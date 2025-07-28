using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public enum CharacterType
{
    Triceratops,
    Spinosaurus,
    Parasaurolophus,
    Therizinosaurus
}

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerCombatManager))]
[RequireComponent(typeof(AnimationController))]
public class MainPlayerController : MonoBehaviour
{
    [Header("Character Settings")]
    [SerializeField] private CharacterStats characterStats;
    [SerializeField] private CharacterType characterType;
    [SerializeField] private bool facingRight = true;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Required Components")]
    [SerializeField] private PlayerTransformData playerTransform;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private GameObject rightMeleeColliderGO;
    [SerializeField] private GameObject leftMeleeColliderGO;
    [SerializeField] private SoundPlayer soundPlayer;
    [SerializeField] private AnimationController animController;
    [SerializeField] private PlayerCombatManager combatManager;
    [SerializeField] private KnockbackManager knockbackManager;
    [SerializeField] private MeleeDamage rightMeleeDamage;
    [SerializeField] private MeleeDamage leftMeleeDamage;
    [SerializeField] private Animator animator;
    [SerializeField] private AudioSource audioSource;

    [Header("Attack Variables")]
    [SerializeField] private float enableDuration = 0.2f;
    [SerializeField] private float disableDelay = 0.5f;

    [Header("Revive Variables")]
    [SerializeField] private float reviveRange = 2f;

    [Header("Emote Variables")]
    [SerializeField] private AudioClip emoteSound;

    private float lastAttackTime;
    private float lastSpecialTime;
    private bool canAttack = true;
    private bool canSpecial = true;
    private bool isAttacking = false;
    private bool isBlocking = false;
    private bool isEmoting = false;
    private bool isFallen = false;
    private bool isMudSlowed = false;
    private bool isPerformingSpecialMovement = false;
    private bool isEndOfLevel = false;
    private Vector2 moveInput;
    private bool emoteHeld = false;
    private bool blockHeld = false;
    private Vector2 currentVelocity;
    private CharacterStats.CharacterData stats;
    private CharacterBase characterScript;
    private static int activePlayers = 0;
    private static int fallenPlayers = 0;
    private int score;

    public PlayerCombatManager CombatManager => combatManager;

    public static bool CanBeDamaged = true;

    public int GetScore() => score;
    public bool IsFallen() => isFallen;

    public void AddScore(int points)
    {
        score += points;
    }

    public void Revived()
    {
        isFallen = false;
        canAttack = true;
        canSpecial = true;
        fallenPlayers--;
        combatManager.RestoreHealthByPercent(100f);
        animController.SetRevived();
        StartCoroutine(ResetRevive());
        Debug.Log($"{stats.characterName} revived");
    }

    public void EnterFallenState()
    {
        isFallen = true;
        canAttack = false;
        canSpecial = false;
        rb.linearVelocity = Vector2.zero;
        animController.SetDowned(true);
        AddScore(-15);
        fallenPlayers++;
        Debug.Log($"{stats.characterName} has fallen, score: {score}");
    }

    private IEnumerator ResetRevive()
    {
        yield return new WaitForSeconds(0.1f);
        animController.SetDowned(false);
    }

    private void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (playerTransform != null) playerTransform.PlayerTransform = transform;
        
        rightMeleeDamage = rightMeleeColliderGO != null ? rightMeleeColliderGO.GetComponent<MeleeDamage>() : null;
        leftMeleeDamage = leftMeleeColliderGO != null ? leftMeleeColliderGO.GetComponent<MeleeDamage>() : null;

        if (characterStats != null)
        {
            stats = characterStats.characters[(int)characterType];
            combatManager.Initialize(stats.maxHealth, stats.stamina, this, animator);
            combatManager.OnDeath += (_) => EnterFallenState();
            Debug.Log($"Loaded stats for {stats.characterName}");
        }
        else Debug.LogError("CharacterStats not assigned in Inspector!");

        switch (characterType)
        {
            case CharacterType.Triceratops:
                characterScript = gameObject.AddComponent<Triceratops>();
                break;
            case CharacterType.Spinosaurus:
                characterScript = gameObject.AddComponent<Spinosaurus>();
                break;
            case CharacterType.Parasaurolophus:
                characterScript = gameObject.AddComponent<Parasaurolophus>();
                break;
            case CharacterType.Therizinosaurus:
                characterScript = gameObject.AddComponent<Therizinosaurus>();
                break;
        }

        characterScript.Initialize(stats, animController, rightMeleeColliderGO, leftMeleeColliderGO, facingRight, enableDuration, disableDelay);
        animController.characterType = characterType;

        activePlayers++;

        combatManager.OnDeath += PlayDeathSound;
    }

    private void OnEnable() { GameManager.OnLevelEnd += OnLevelEnd; }
    private void OnDisable() { activePlayers--; GameManager.OnLevelEnd -= OnLevelEnd; }

    private void Update()
    {
        if (!isFallen)
        {
            combatManager.RegenerateStamina(Time.deltaTime);
            animController.SetMoveSpeed(moveInput.magnitude);
            spriteRenderer.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100);

            // Stop movement during emote, block, or special (except Triceratops)
            if (isEmoting || isBlocking || (isPerformingSpecialMovement && characterType != CharacterType.Triceratops))
            {
                rb.linearVelocity = Vector2.zero;
            }
        }

        // Block handling
        if (blockHeld && !isBlocking && !isEmoting && !isFallen)
        {
            isBlocking = true;
            animController.SetBlocking(true);
            CanBeDamaged = false;
        }
        else if (!blockHeld && isBlocking)
        {
            isBlocking = false;
            animController.SetBlocking(false);
            CanBeDamaged = true;
        }

        // Emote handling
        if (emoteHeld && !isFallen && !isBlocking)
        {
            isEmoting = true;
            animController.SetEmoting(true);
            if (emoteSound != null && audioSource != null)
                audioSource.PlayOneShot(emoteSound);
            Debug.Log($"{stats.characterName} started emoting");
        }
        else if (!emoteHeld && isEmoting)
        {
            isEmoting = false;
            animController.SetEmoting(false);
            Debug.Log($"{stats.characterName} stopped emoting");
        }
    }

    private void FixedUpdate()
    {
        if (!isAttacking && !isFallen && !isEmoting && !isBlocking && (!isPerformingSpecialMovement || characterType == CharacterType.Triceratops))
        {
            HandleMovement();
        }
        else 
        {
            if(!(characterType == CharacterType.Triceratops && isPerformingSpecialMovement))
                rb.linearVelocity = Vector2.zero;
        }
    }

    private void HandleMovement()
    {
        if (knockbackManager != null && knockbackManager.IsKnockedBack || isEndOfLevel) return;

        float effectiveSpeed = stats.movementSpeed;
        if (isMudSlowed) { effectiveSpeed *= stats.mudSlowFactor; }
        currentVelocity = moveInput.normalized * effectiveSpeed;
        rb.linearVelocity = currentVelocity;

        if (Mathf.Abs(moveInput.x) > 0.01f)
        {
            bool shouldFaceRight = moveInput.x > 0;
            
            if (shouldFaceRight != facingRight)
            {
                facingRight = shouldFaceRight;
                spriteRenderer.flipX = !facingRight;
                characterScript.facingRight = facingRight;
            }
        }

        if (animController != null)
        {
            float isMoving = moveInput.magnitude;
            animController.SetMoveSpeed(isMoving);
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void ApplyDamageBoost(float percentage)
    {
        float multiplier = 1 + percentage / 100f;
        stats.damageMin *= multiplier;
        stats.damageMax *= multiplier;

        Debug.Log($"New range: {stats.damageMin} - {stats.damageMax}");
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (context.performed && canAttack && !isBlocking && !isEmoting && !isFallen)
        {
            canAttack = false;
            lastAttackTime = Time.time;

            float damage = characterType == CharacterType.Spinosaurus ? stats.damageMin : Random.Range(stats.damageMin, stats.damageMax);
            animController.TriggerAttack();

            StartCoroutine(characterScript.PerformAttack(damage, (dmg) =>
            {
                MeleeDamage activeMeleeDamage = facingRight ? rightMeleeDamage : leftMeleeDamage;
                activeMeleeDamage?.ApplyDamage(dmg, false, transform, this);
            }));
            soundPlayer.PlaySound(0);

            StartCoroutine(ResetAttackCooldown());
        }
    }

    public void SpecialStarted(InputAction.CallbackContext context)
    {
        if (context.performed && !isFallen && canSpecial && !isEmoting && combatManager.DeductStamina(stats.specialAttackCost))
        {
            canSpecial = false;
            animController.TriggerSpecial();
            StartCoroutine(PerformSpecialAttackCoroutine());
            soundPlayer.PlaySound(1);
        }
    }

    public void Block(InputAction.CallbackContext context)
    {
        bool previousInput = blockHeld;
        
        if (context.ReadValue<float>() == 0)
            blockHeld = false;
        else
            blockHeld = true;

        if (blockHeld != previousInput && blockHeld)
            animator.SetTrigger("BlockStart");
    }

    public void Revive(InputAction.CallbackContext context)
    {
        if (context.performed && !isFallen)
        {
            MainPlayerController target = FindNearestFallenPlayer();
            if (target != null)
            {
                animController.SetRevived();
                RevivePrompt prompt = gameObject.AddComponent<RevivePrompt>();
                prompt.StartRevive(this, target);
            }
        }
    }

    public void Emote(InputAction.CallbackContext context)
    {
        bool previousInput = emoteHeld;
        if (context.ReadValue<float>() == 0)
            emoteHeld = false;
        else
            emoteHeld = true;

        if (emoteHeld != previousInput && emoteHeld)
            animator.SetTrigger("Emote");
    }

    private IEnumerator ResetAttackCooldown()
    {
        yield return new WaitForSeconds(1f / stats.attacksPerSecond);
        canAttack = true;
    }

    private IEnumerator PerformSpecialAttackCoroutine()
    {
        isPerformingSpecialMovement = true;
        ToggleIsAttacking();
        //----------------------------------------------------------------------------------------------------remove???????????????
        yield return characterScript.PerformSpecial((dmg) => { /* MeleeDamage activeMeleeDamage = facingRight ? rightMeleeDamage : leftMeleeDamage;

            if (characterType == CharacterType.Parasaurolophus)
            {
                //activeMeleeDamage = ((Parasaurolophus)characterScript).SpecialMeleeDamage;
                //activeMeleeDamage?.ApplyDamage(dmg, true, transform, this);
            }
            else
            {
                //activeMeleeDamage?.ApplyDamage(dmg, true, transform, this, characterType == CharacterType.Spinosaurus);
            }*/});

        ToggleIsAttacking();
        isPerformingSpecialMovement = false;
        canSpecial = true;
    }

    private MainPlayerController FindNearestFallenPlayer()
    {
        MainPlayerController[] players = FindObjectsByType<MainPlayerController>(FindObjectsSortMode.None);
        MainPlayerController nearest = null;
        float minDistance = reviveRange;

        foreach (var player in players)
        {
            if (player != this && player.IsFallen())
            {
                float dist = Vector2.Distance(transform.position, player.transform.position);
                if (dist <= minDistance)
                {
                    nearest = player;
                    minDistance = dist;
                }
            }
        }

        return nearest;
    }

    private void OnLevelEnd(MainPlayerController controller)
    {
        isEndOfLevel = true;
        if (controller == this)
        {
            emoteHeld = true;
            animator.SetTrigger("Emote");
        }
    }

    private void PlayDeathSound(CombatManager combatManager)
    {
        soundPlayer.PlaySound(2);
    }
    
    public void ToggleMudSlowEffect()
    {
        isMudSlowed = !isMudSlowed;
        Debug.Log($"{stats.characterName} mud slow active: {isMudSlowed}");
    }
    
    public void ToggleIsAttacking()
    {
        isAttacking = !isAttacking;
        Debug.Log($"{stats.characterName} is attacking: {isAttacking}");
    }
}