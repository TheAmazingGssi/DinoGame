using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

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
    // ---------- Inspector: Serialized References & Settings ----------

    [Header("Character ▸ Identity & Visuals")]
    [SerializeField] private CharacterStats characterStats;
    [SerializeField] private CharacterType characterType;
    [SerializeField] private bool facingRight = true;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] public Transform worldPosition;

    [Header("Components ▸ Core (Required)")]
    [SerializeField] public AnimationController animController;
    [SerializeField] private PlayerTransformData playerTransform;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private SoundPlayer soundPlayer;

    [Header("Components ▸ Combat")]
    [SerializeField] private PlayerCombatManager combatManager;
    [SerializeField] private KnockbackManager knockbackManager;
    [SerializeField] private GameObject rightMeleeColliderGO;
    [SerializeField] private GameObject leftMeleeColliderGO;
    [SerializeField] private MeleeDamage rightMeleeDamage;
    [SerializeField] private MeleeDamage leftMeleeDamage;

    [Header("Components ▸ UI & Effects")]
    [SerializeField] private GameObject crown;
    [SerializeField] private GameObject BlockBubble; // keeping your original casing
    [SerializeField] private ReviveMiniGame reviveMiniGame;
    [SerializeField] private CoopAttack coopAttack;

    [Header("Gameplay ▸ Attack")]
    [SerializeField] private float enableDuration = 0.2f;
    [SerializeField] private float disableDelay = 0.5f;

    [Header("Gameplay ▸ Revive")]
    [SerializeField] private float reviveRange = 2f;

    [Header("Gameplay ▸ Status Effects")]
    [SerializeField] private float freezeLength = 3f;

    [Header("Audio ▸ Emotes")]
    [SerializeField] private AudioClip emoteSound;


    // ---------- Runtime: State (not serialized) ----------

    #region Runtime State
    public bool isBlocking { get; private set; } = false;
    public bool blockHeld { get; private set; } = false;

    private MainPlayerController currentReviveTarget;
    private float lastAttackTime;
    private float lastSpecialTime;
    private bool blockRepressRequired; 
    private bool canAttack = true;
    private bool canSpecial = true;
    private bool isAttacking = false;
    private bool isEmoting = false;
    private bool isFallen = false;
    private bool isFrozen = false;
    private bool isMudSlowed = false;
    private bool isPerformingSpecialMovement = false;
    private bool isEndOfLevel = false;

    private Vector2 moveInput;
    private bool emoteHeld = false;
    private Vector2 currentVelocity;

    private CharacterStats.CharacterData stats;
    private CharacterBase characterScript;

    private static int activePlayers = 0; // todo: remove if not needed
    private static int fallenPlayers = 0; // todo: remove if not needed
    private int score;
    #endregion


    // ---------- Properties & API ----------

    public PlayerCombatManager CombatManager => combatManager;
    public CharacterType CharacterType => characterType;

    // todo: new block system
    public bool IsBlocking => isBlocking;
    public bool IsFacingRight => facingRight;

    public static bool CanBeDamaged = true;

public int GetScore() => score;
public bool IsFallen() => isFallen;


    public void AddScore(int points)
    {
        score += points;
        GameManager.Instance.HandleScoreChange();
    }

    public void Revived()
    {
        isFallen = false;
        canAttack = true;
        canSpecial = true;
        fallenPlayers--;
        combatManager.RestoreHealthByPercent(100f);
        StartCoroutine(ResetRevive());
        soundPlayer.PlaySound(3);
        animController.TriggerHalo();
        Debug.Log($"{stats.characterName} revived");
    }

    public void EnterFallenState()
    {
        isFallen = true;
        canAttack = false;
        canSpecial = false;
        rb.linearVelocity = Vector2.zero;
        animController.SetDowned(true);
        reviveMiniGame?.ShowOnDowned();
        AddScore(-15);
        fallenPlayers++;
        Debug.Log($"{stats.characterName} has fallen, score: {score}");
    }

    private IEnumerator ResetRevive()
    {
        yield return new WaitForSeconds(0.1f);
        animController.SetDowned(false);
        reviveMiniGame?.HideOnRevived();
    }

    private void Awake()
    {
        if (rb == null) 
            rb = GetComponent<Rigidbody2D>();
        
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (playerTransform != null) 
            playerTransform.PlayerTransform = transform;
        
        if (!reviveMiniGame) 
            reviveMiniGame = GetComponent<ReviveMiniGame>();

        if (!coopAttack)
            coopAttack = FindFirstObjectByType<CoopAttack>();
        
        rightMeleeDamage = rightMeleeColliderGO != null ? rightMeleeColliderGO.GetComponent<MeleeDamage>() : null;
        leftMeleeDamage = leftMeleeColliderGO != null ? leftMeleeColliderGO.GetComponent<MeleeDamage>() : null;

        if (characterStats != null)
        {
            stats = characterStats.characters[(int)characterType];
            
            combatManager.Initialize(stats.maxHealth, stats.stamina, this, animator,
                stats.blockStaminaMax, stats.blockCost, stats.blockRegenRate);

            combatManager.OnDeath += (_) => EnterFallenState();
            Debug.Log($"Loaded stats for {stats.characterName}");
        }
        else 
            Debug.LogError("CharacterStats not assigned in Inspector!");

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

        crown.gameObject.SetActive(false);
    }

    private void OnEnable() { GameManager.OnLevelEnd += OnLevelEnd; }
    private void OnDisable() { activePlayers--; GameManager.OnLevelEnd -= OnLevelEnd; }

    private void Update()
    {
        if (isFrozen)
            return;
        
        if (!isFallen)
        {
            combatManager.RegenerateStamina(Time.deltaTime);
            
            combatManager.RegenerateBlockStamina(Time.deltaTime, isBlocking);

            animController.SetMoveSpeed(moveInput.magnitude);
            spriteRenderer.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100);

            // Stop movement during emote, block, or special (except Triceratops)
            if (isEmoting || isBlocking || (isPerformingSpecialMovement && characterType != CharacterType.Triceratops))
            {
                rb.linearVelocity = Vector2.zero;
            }
        }
        
        // Force break when empty/locked and require a release before restarting
        if (isBlocking && (!combatManager.HasBlockStamina() || combatManager.IsBlockLocked))
        {
            ForceStopBlocking();
            blockRepressRequired = true; // gate restart until button is released
        }

        // Clear the gate once the player releases the button
        if (!blockHeld)
            blockRepressRequired = false;

        // Start block (gated, no auto-restart while still held)
        if (blockHeld && !blockRepressRequired && !isBlocking && !isEmoting && !isFallen)
        {
            if (combatManager.HasBlockStamina() && !combatManager.IsBlockLocked)
            {
                isBlocking = true;
                animController.SetBlocking(true);
            }
        }
        else if (!blockHeld && isBlocking) // Stop on release
        {
            ForceStopBlocking();
        }

        // Bubble mirrors true state only
        BlockBubble.SetActive(isBlocking && combatManager.HasBlockStamina() && !combatManager.IsBlockLocked);


        // Emote handling
        if (emoteHeld && !isFallen && !isBlocking)
        {
            isEmoting = true;
            animController.SetEmoting(true);
            if (emoteSound != null && audioSource != null)
                audioSource.PlayOneShot(emoteSound);
        }
        else if (!emoteHeld && isEmoting)
        {
            isEmoting = false;
            animController.SetEmoting(false);
        }
    }

    private void FixedUpdate()
    {
        if (knockbackManager != null && knockbackManager.IsKnockedBack)
            return;
        
        if (!isFrozen)
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
    }

    private void HandleMovement()
    {
        if (knockbackManager != null && knockbackManager.IsKnockedBack)
        {
            animator.SetFloat("Speed", 0f);
            return;
        }

        if (isEndOfLevel) return;

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
        if (context.performed && canAttack && !isBlocking && !isEmoting && !isFallen && !isFrozen)
        {
            canAttack = false;
            lastAttackTime = Time.time;

            float damage = characterType == CharacterType.Spinosaurus ? stats.damageMin : Random.Range(stats.damageMin, stats.damageMax);
            animController.TriggerAttack();

            StartCoroutine(characterScript.PerformAttack(damage, dmg =>
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
        if (context.performed && !isFallen && canSpecial && !isEmoting && !isFrozen
            && combatManager.DeductStamina(stats.specialAttackCost))
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
    
    public void ForceStopBlocking()
    {
        if (!isBlocking) return;
        isBlocking = false;
        animController.SetBlocking(false);
        BlockBubble.SetActive(false);
    }
    
    public void Revive(InputAction.CallbackContext context)
    {
        // Ignore if I'm the fallen one or frozen, etc.
        if (isFallen) 
            return;

        if (context.started)
        {
            // Find & cache the target at the start of the hold
            var fallen = FindNearestFallenPlayer();
            if (fallen == null)
                return;
            
            currentReviveTarget = fallen;

            var mini = fallen.GetComponent<ReviveMiniGame>();
            if (!mini) 
                mini = fallen.gameObject.AddComponent<ReviveMiniGame>(); // safety

            if (!mini.CanBegin(this, reviveRange))
                return;
            
            mini.BeginHold(this);
            fallen.animController.StartHealVfx();
        }
        else if (context.canceled)
        {
            if (currentReviveTarget)
            {
                var mini = currentReviveTarget.GetComponent<ReviveMiniGame>();
                mini?.StopHold(this);
                currentReviveTarget?.animController.StopHealVfx();
                currentReviveTarget = null;
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

    public bool FriendshipAttackFlag { get; private set; } = false;
    public void FriendshipAttack(InputAction.CallbackContext context)
    {
        if(!FriendshipAttackFlag)
            StartCoroutine(RaiseFriendshipFlag());
    }
    
    private IEnumerator RaiseFriendshipFlag()
    {
        yield return new WaitUntil(() => GameManager.Instance != null && CoopBarTimer.Instance != null);

        CoopBarTimer.Instance.PlayersTryingToUlt++;
        FriendshipAttackFlag = true;

        yield return new WaitForSeconds(GameManager.Instance.VotePersistenceDuration);

        FriendshipAttackFlag = false;
        CoopBarTimer.Instance.PlayersTryingToUlt--;
    }
    
    public void StartCoopActualAttack()
    {
        coopAttack.Activate();
    }
    
    private IEnumerator ResetAttackCooldown()
    {
        yield return new WaitForSeconds(1f / stats.attacksPerSecond);
        canAttack = true;
    }

    private IEnumerator PerformSpecialAttackCoroutine()
    {
        isPerformingSpecialMovement = true;
        yield return characterScript.PerformSpecial((dmg) => { });
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
            crown.gameObject.SetActive(true);
            Debug.Log($"{controller.gameObject.name} has the crown");
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
    
    public void ActivateFreezeEffect()
    {
        StartCoroutine(FreezeCoroutine());
    }

    private IEnumerator FreezeCoroutine()
    {
        isFrozen = true;
        animController.SetFrozen(true);
        rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(freezeLength);

        isFrozen = false;
        animController.SetFrozen(false);
    }

    public void ToggleIsAttacking()
    {
        isAttacking = !isAttacking;
        Debug.Log($"{stats.characterName} is attacking: {isAttacking}");
    }
}