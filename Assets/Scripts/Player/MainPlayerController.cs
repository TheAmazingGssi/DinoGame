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

    [Header("Attack Variables")]
    [SerializeField] private float enableDuration = 0.2f;
    [SerializeField] private float disableDelay = 0.5f;

    [Header("Block Variables")]
    [SerializeField] private float blockDuration = 0.5f;
    [SerializeField] private float blockMoveSpeedMultiplier = 0.5f;

    [Header("Revive Variables")]
    [SerializeField] private float reviveRange = 2f;

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
    private float currentStamina; // Runtime stamina
    private CharacterBase characterScript;
    private AnimationController animController;
    private PlayerCombatManager combatManager;
    private KnockbackManager knockbackManager;
    private MeleeDamage rightMeleeDamage;
    private MeleeDamage leftMeleeDamage;
    private Animator animator;
    private static int activePlayers = 0;
    private static int fallenPlayers = 0;
    private int score;

    public static bool CanBeDamaged = true;

    public int GetScore() => score;
    public bool IsFallen() => isFallen;

    public void AddScore(int points)
    {
        score += points;
        Debug.Log($"Score updated: {score}");
    }

    public void Revived()
    {
        isFallen = false;
        canAttack = true;
        canSpecial = true;
        canBlock = true;
        fallenPlayers--;
        currentStamina = stats.stamina; // Reset stamina
        animController.SetRevived(true);
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
        animController.SetDowned(true);
        AddScore(-15);
        fallenPlayers++;
        Debug.Log($"{stats.characterName} has fallen, score: {score}");
    }

    private IEnumerator ResetRevive()
    {
        yield return new WaitForSeconds(0.1f);
        animController.SetRevived(false);
        animController.SetDowned(false);
    }

    private void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (playerTransform != null)
            playerTransform.PlayerTransform = transform;

        animController = GetComponent<AnimationController>();
        combatManager = GetComponent<PlayerCombatManager>();
        knockbackManager = GetComponent<KnockbackManager>();
        animator = GetComponent<Animator>();
        rightMeleeDamage = rightMeleeColliderGO != null ? rightMeleeColliderGO.GetComponent<MeleeDamage>() : null;
        leftMeleeDamage = leftMeleeColliderGO != null ? leftMeleeColliderGO.GetComponent<MeleeDamage>() : null;

        if (characterStats != null)
        {
            stats = characterStats.characters[(int)characterType];
            currentStamina = stats.stamina; // Initialize runtime stamina
            combatManager.Initialize(stats.maxHealth, this, animator);
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

        characterScript.Initialize(stats, rightMeleeColliderGO, leftMeleeColliderGO, facingRight, enableDuration, disableDelay);
        animController.characterType = characterType;

        activePlayers++;
    }

    private void OnEnable() { }
    private void OnDisable() { activePlayers--; }

    private void Update()
    {
        if (!isFallen)
        {
            currentStamina = Mathf.Min(stats.stamina, currentStamina + staminaRegenRate * Time.deltaTime);
            animController.SetMoveSpeed(moveInput.magnitude);
            spriteRenderer.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100); // Castle Crashers depth
        }
    }

    private void FixedUpdate()
    {
        if (!isFallen) HandleMovement();
    }

    private void HandleMovement()
    {
        if (knockbackManager != null && knockbackManager.IsKnockedBack || isPerformingSpecialMovement) return;

        float effectiveSpeed = isBlocking
            ? stats.movementSpeed * blockMoveSpeedMultiplier
            : stats.movementSpeed;

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
            animController.SetAnimationSpeed(isMoving > 0.05f ? 1.15f : 1f);
        }
    }

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
            animController.TriggerAttack();

            StartCoroutine(characterScript.PerformAttack(damage, stats.attackSequenceCount, (dmg) =>
            {
                MeleeDamage activeMeleeDamage = facingRight ? rightMeleeDamage : leftMeleeDamage;
                activeMeleeDamage?.ApplyDamage(dmg, false, transform, this);
            }));

            StartCoroutine(ResetAttackCooldown());
        }
    }

    public void SpecialStarted(InputAction.CallbackContext context)
    {
        if (!isFallen && canSpecial && currentStamina >= stats.specialAttackCost)
        {
            canSpecial = false;
            currentStamina -= stats.specialAttackCost;
            animController.TriggerSpecial();
            StartCoroutine(PerformSpecialAttackCoroutine());
        }
    }

    public void Block(InputAction.CallbackContext context)
    {
        if (canBlock && !isBlocking && !isFallen)
        {
            canBlock = false;
            lastBlockTime = Time.time;
            animController.SetBlocking(true);
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
                animController.SetRevived(true);
                RevivePrompt prompt = gameObject.AddComponent<RevivePrompt>();
                prompt.StartRevive(this, target);
            }
        }
    }

    public void Emote(InputAction.CallbackContext context)
    {
        if (!isFallen && !isBlocking)
        {
            animController.TriggerEmote();
        }
    }

    private IEnumerator ResetAttackCooldown()
    {
        yield return new WaitForSeconds(1f / stats.attacksPerSecond * stats.attackSequenceCount);
        canAttack = true;
    }

    private IEnumerator PerformSpecialAttackCoroutine()
    {
        isPerformingSpecialMovement = true;
        yield return characterScript.PerformSpecial((dmg) =>
        {
            MeleeDamage activeMeleeDamage = facingRight ? rightMeleeDamage : leftMeleeDamage;
            if (characterType == CharacterType.Parasaurolophus)
            {
                activeMeleeDamage = ((Parasaurolophus)characterScript).SpecialMeleeDamage;
                activeMeleeDamage?.ApplyDamage(dmg, true, transform, this);
            }
            else
            {
                activeMeleeDamage?.ApplyDamage(dmg, true, transform, this, characterType == CharacterType.Spinosaurus);
            }
        });

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
        animController.SetBlocking(false);

        float elapsed = Time.time - lastBlockTime;
        if (elapsed < stats.stamina / 25f)
            yield return new WaitForSeconds(stats.stamina / 25f - elapsed);

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
}