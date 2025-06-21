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

    [Header("Attack Variables")]
    [SerializeField] private GameObject rightMeleeColliderGO;
    [SerializeField] private GameObject leftMeleeColliderGO;
    [SerializeField] private float enableDuration = 0.2f;
    [SerializeField] private float disableDelay = 0.5f;

    [Header("Block Variables")]
    [SerializeField] private float blockDuration = 0.5f;
    [SerializeField] private float blockMoveSpeedMultiplier = 0.5f;

    [Header("Revive Variables")]
    [SerializeField] private float reviveRange = 2f;
    [SerializeField] private float revivePromptDuration = 4f;

    [Header("Stamina Settings")]
    [SerializeField] private float staminaRegenRate = 10f;

    private float lastAttackTime;
    private float lastSpecialTime;
    private float lastBlockTime;
    private bool canAttack = true;
    private bool canSpecial = true;
    private bool canBlock = true;
    private bool isBlocking = false;
    private bool isFallen = false;
    private bool isPerformingSpecialMovement = false;
    private Vector2 moveInput;
    private Vector2 currentVelocity;
    private CharacterStats.CharacterData stats;
    private CharacterBase characterScript;
    private PlayerCombatManager combatManager;
    private int score;
    private static int activePlayers = 0;
    private static int fallenPlayers = 0;

    public static bool CanBeDamaged = true;

    public int GetScore() => score;
    public bool IsFallen() => isFallen;

    public void AddScore(int points)
    {
        score += points;
        Debug.Log($"Score updated: {score}");
    }

    public void Revive()
    {
        isFallen = false;
        canAttack = true;
        canSpecial = true;
        canBlock = true;
        fallenPlayers--;
        GetComponent<AnimationController>().SetRevived(true);
        StartCoroutine(ResetRevive());
        Debug.Log($"{stats.characterName} revived");
    }

    public void EnterFallenState()
    {
        isFallen = true;
        canAttack = false;
        canSpecial = false;
        canBlock = false;
        rb.linearVelocity = Vector2.zero;
        GetComponent<AnimationController>().TriggerDowned();
        AddScore(-15);
        fallenPlayers++;
        Debug.Log($"{stats.characterName} has fallen, score: {score}");
    }

    private IEnumerator ResetRevive()
    {
        yield return new WaitForSeconds(0.1f);
        GetComponent<AnimationController>().SetRevived(false);
    }

    private void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();

        playerTransform.PlayerTransform = transform;

        if (characterStats != null)
        {
            stats = characterStats.characters[(int)characterType];
            stats.currentStamina = stats.stamina;
            Debug.Log($"Loaded stats for {stats.characterName}");
        }
        else Debug.LogError("CharacterStats not assigned in Inspector!");

        combatManager = GetComponent<PlayerCombatManager>();
        combatManager.OnDeath += (cm) => EnterFallenState();

        switch (characterType)
        {
            case CharacterType.Triceratops: characterScript = gameObject.AddComponent<Triceratops>(); break;
            case CharacterType.Spinosaurus: characterScript = gameObject.AddComponent<Spinosaurus>(); break;
            case CharacterType.Parasaurolophus: characterScript = gameObject.AddComponent<Parasaurolophus>(); break;
            case CharacterType.Therizinosaurus: characterScript = gameObject.AddComponent<Therizinosaurus>(); break;
        }
        characterScript.Initialize(stats, rightMeleeColliderGO, leftMeleeColliderGO, facingRight, enableDuration, disableDelay);

        SetupAnimationController();
        activePlayers++;
    }

    private void OnEnable() { }
    private void OnDisable() { activePlayers--; }

    private void Update()
    {
        if (!isFallen)
        {
            stats.currentStamina = Mathf.Min(stats.stamina, stats.currentStamina + staminaRegenRate * Time.deltaTime);
        }
    }

    private void FixedUpdate()
    {
        if (!isFallen) HandleMovement();
    }

    private void HandleMovement()
    {
        KnockbackManager knockbackManager = GetComponent<KnockbackManager>();
        if ((knockbackManager != null && knockbackManager.IsKnockedBack) || isPerformingSpecialMovement) return;
        
        float effectiveMoveSpeed = isBlocking ? stats.movementSpeed * blockMoveSpeedMultiplier : stats.movementSpeed;
        Vector2 targetVelocity = moveInput.normalized * effectiveMoveSpeed;
        currentVelocity = Vector2.Lerp(currentVelocity, targetVelocity, stats.attacksPerSecond * Time.fixedDeltaTime);
        rb.linearVelocity = currentVelocity;

        if (Mathf.Abs(moveInput.x) > 0.01f)
        {
            bool shouldFaceRight = moveInput.x > 0;
            if (shouldFaceRight != facingRight)
            {
                facingRight = shouldFaceRight;
                spriteRenderer.flipX = !facingRight;
            }
        }
    }

    private void SetupAnimationController()
    {
        AnimationController animController = GetComponent<AnimationController>();
        animController.characterType = characterType;
    }

    // Event handlers for PlayerEntity events
    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (canAttack && !isBlocking && !isFallen)
        {
            canAttack = false;
            lastAttackTime = Time.time;
            float damage = characterType == CharacterType.Spinosaurus ? stats.damageMin : Random.Range(stats.damageMin, stats.damageMax);
            GetComponent<AnimationController>().TriggerAttack();
            StartCoroutine(characterScript.PerformAttack(damage, stats.attackSequenceCount, (dmg) =>
            {
                GameObject activeCollider = facingRight ? rightMeleeColliderGO : leftMeleeColliderGO;
                MeleeDamage meleeDamage = activeCollider.GetComponent<MeleeDamage>();
                if (meleeDamage != null) meleeDamage.ApplyDamage(dmg, false, transform, this);
            }));
            StartCoroutine(ResetAttackCooldown());
        }
    }

    public void SpecialStarted(InputAction.CallbackContext context)
    {
        if (!isFallen) GetComponent<AnimationController>().TriggerSpecial();
    }

    public void Block(InputAction.CallbackContext context)
    {
        if (canBlock && !isBlocking && !isFallen)
        {
            canBlock = false;
            lastBlockTime = Time.time;
            GetComponent<AnimationController>().SetBlocking(true);
            StartCoroutine(BlockDamageWindow());
        }
    }

    public void Revive(InputAction.CallbackContext context)
    {
        if (!isFallen)
        {
            MainPlayerController target = FindNearestFallenPlayer();
            if (target != null)
            {
                GetComponent<AnimationController>().TriggerReviveFront();
                RevivePrompt prompt = gameObject.AddComponent<RevivePrompt>();
                prompt.StartRevive(this, target);
            }
        }
    }

    private IEnumerator ResetAttackCooldown()
    {
        yield return new WaitForSeconds(1f / stats.attacksPerSecond * stats.attackSequenceCount);
        canAttack = true;
    }

    private IEnumerator PerformSpecialAttackCoroutine()
    {
        yield return StartCoroutine(characterScript.PerformSpecial((dmg) =>
        {
            if (characterType == CharacterType.Parasaurolophus)
            {
                rightMeleeColliderGO.GetComponent<MeleeDamage>()?.ApplyDamage(dmg, true, transform, this);
                leftMeleeColliderGO.GetComponent<MeleeDamage>()?.ApplyDamage(dmg, true, transform, this);
            }
            else
            {
                GameObject activeCollider = facingRight ? rightMeleeColliderGO : leftMeleeColliderGO;
                MeleeDamage meleeDamage = activeCollider.GetComponent<MeleeDamage>();
                if (meleeDamage != null) meleeDamage.ApplyDamage(dmg, true, transform, this, characterType == CharacterType.Spinosaurus);
            }
        }));
        
        isPerformingSpecialMovement = false;
        yield return new WaitForSeconds(stats.specialAttackCost / 15f);
        canSpecial = true;
    }

    private IEnumerator BlockDamageWindow()
    {
        isBlocking = true;
        CanBeDamaged = false;
        yield return new WaitForSeconds(blockDuration);
        CanBeDamaged = true;
        isBlocking = false;
        GetComponent<AnimationController>().SetBlocking(false);
        
        float elapsedTime = Time.time - lastBlockTime;
        if (elapsedTime < stats.stamina / 25f) yield return new WaitForSeconds(stats.stamina / 25f - elapsedTime);
        canBlock = true;
    }

    private MainPlayerController FindNearestFallenPlayer()
    {
        MainPlayerController[] players = FindObjectsOfType<MainPlayerController>();
        MainPlayerController nearest = null;
        float minDistance = reviveRange;
        
        foreach (var player in players)
        {
            if (player != this && player.IsFallen())
            {
                float distance = Vector2.Distance(transform.position, player.transform.position);
                if (distance <= minDistance) { nearest = player; minDistance = distance; }
            }
        }
        return nearest;
    }
}