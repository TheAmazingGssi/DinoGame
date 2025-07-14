using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(PlayerCombatManager), typeof(AnimationController))]
public class MainPlayerController : MonoBehaviour
{
    [Header("Character Settings")]
    [SerializeField] private CharacterStats characterStats;
    [SerializeField] private CharacterMapping characterMapping;
    [SerializeField] private CharacterType characterType;
    [SerializeField] private bool facingRight = true;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Required Components")]
    [SerializeField] private PlayerTransformData playerTransform;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private GameObject rightMeleeColliderGO;
    [SerializeField] private GameObject leftMeleeColliderGO;
    [SerializeField] private GameObject specialColliderGO;
    [SerializeField] private SoundPlayer soundPlayer;

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
    private bool isBlocking;
    private bool isEmoting;
    private bool isFallen;
    private bool isPerformingSpecialMovement;
    private Vector2 moveInput;
    private Vector2 currentVelocity;
    private CharacterStats.CharacterData stats;
    private CharacterBase characterScript;
    private AnimationController animController;
    private PlayerCombatManager combatManager;
    private KnockbackManager knockbackManager;
    private MeleeDamage rightMeleeDamage;
    private MeleeDamage leftMeleeDamage;
    private Animator animator;
    private AudioSource audioSource;
    private int score;
    private static int activePlayers;
    private static int fallenPlayers;

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
        canAttack = canSpecial = true;
        fallenPlayers--;
        combatManager.RestoreHealthByPercent(100f);
        animController.SetRevived();
        StartCoroutine(ResetRevive());
        Debug.Log($"{stats.characterName} revived");
    }

    public void EnterFallenState()
    {
        isFallen = true;
        canAttack = canSpecial = false;
        isBlocking = isEmoting = false;
        rb.linearVelocity = Vector2.zero;
        animController.SetDowned(true);
        animController.SetBlocking(false);
        animController.SetEmote(false);
        AddScore(-15);
        fallenPlayers++;
        Debug.Log($"{stats.characterName} has fallen, score: {score}");
    }

    private void Awake()
    {
        // Cache components
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        spriteRenderer = spriteRenderer ?? GetComponent<SpriteRenderer>();
        animController = GetComponent<AnimationController>();
        combatManager = GetComponent<PlayerCombatManager>();
        knockbackManager = GetComponent<KnockbackManager>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        if (playerTransform != null)
            playerTransform.PlayerTransform = transform;

        rightMeleeDamage = rightMeleeColliderGO?.GetComponent<MeleeDamage>();
        leftMeleeDamage = leftMeleeColliderGO?.GetComponent<MeleeDamage>();

        // Initialize stats and character
        if (characterStats == null || characterMapping == null)
        {
            Debug.LogError("CharacterStats or CharacterMapping not assigned!");
            return;
        }

        stats = characterStats.characters[(int)characterType];
        combatManager.Initialize(stats.maxHealth, stats.stamina, this, animator);
        combatManager.OnDeath += (_) => EnterFallenState();

        // Add character-specific script
        System.Type characterScriptType = characterMapping.GetCharacterScript(characterType);
        if (characterScriptType != null)
        {
            characterScript = gameObject.AddComponent(characterScriptType) as CharacterBase;
            characterScript?.Initialize(stats, rightMeleeColliderGO, leftMeleeColliderGO, specialColliderGO, facingRight, enableDuration, disableDelay);
        }
        else
        {
            Debug.LogError($"Failed to add character script for {characterType}");
        }

        animController.characterType = characterType;
        activePlayers++;
        combatManager.OnDeath += PlayDeathSound;
        Debug.Log($"Loaded stats for {stats.characterName}");

        // Register with PlayerManager
        PlayerManager.RegisterPlayer(this);
    }

    private void OnDisable()
    {
        activePlayers--;
        PlayerManager.UnregisterPlayer(this);
    }

    private void Update()
    {
        if (!isFallen)
        {
            combatManager.RegenerateStamina(Time.deltaTime);
            animController.SetMoveSpeed(moveInput.magnitude);
            spriteRenderer.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100);
        }
    }

    private void FixedUpdate()
    {
        if (!isFallen)
            HandleMovement();
    }

    private void HandleMovement()
    {
        if (knockbackManager.IsKnockedBack || isPerformingSpecialMovement || isBlocking || isEmoting)
            return;

        float effectiveSpeed = stats.movementSpeed;
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

        animController.SetMoveSpeed(moveInput.magnitude);
    }

    public void Move(InputAction.CallbackContext context)
    {
        if (isFallen)
        {
            moveInput = Vector2.zero;
            return;
        }
        moveInput = context.ReadValue<Vector2>();
    }

    public void ApplyDamageBoost(float percentage)
    {
        float multiplier = 1 + percentage / 100f;
        stats.damageMin *= multiplier;
        stats.damageMax *= multiplier;
        Debug.Log($"New damage range: {stats.damageMin} - {stats.damageMax}");
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (!context.performed || isFallen || isBlocking || !canAttack)
            return;

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

    public void SpecialStarted(InputAction.CallbackContext context)
    {
        if (!context.performed || isFallen || !canSpecial || !combatManager.DeductStamina(stats.specialAttackCost))
            return;

        canSpecial = false;
        animController.TriggerSpecial();
        StartCoroutine(PerformSpecialAttackCoroutine());
        soundPlayer.PlaySound(1);
    }

    public void Block(InputAction.CallbackContext context)
    {
        if (isFallen)
            return;

        if (context.started || context.performed)
        {
            isBlocking = true;
            CanBeDamaged = false;
            animController.SetBlocking(true);
            Debug.Log($"{stats.characterName} started blocking");
        }
        else if (context.canceled)
        {
            isBlocking = false;
            CanBeDamaged = true;
            animController.SetBlocking(false);
            Debug.Log($"{stats.characterName} stopped blocking");
        }
    }

    public void Revive(InputAction.CallbackContext context)
    {
        if (!context.performed || isFallen)
            return;

        MainPlayerController target = PlayerManager.FindNearestFallenPlayer(transform.position, reviveRange);
        if (target != null)
        {
            animController.SetRevived();
            RevivePrompt prompt = gameObject.AddComponent<RevivePrompt>();
            prompt.StartRevive(this, target);
        }
    }

    public void Emote(InputAction.CallbackContext context)
    {
        if (!context.performed && !context.canceled || isFallen)
            return;

        if (context.started || context.performed)
        {
            isEmoting = true;
            animController.SetEmote(true);
            if (emoteSound != null)
                audioSource.PlayOneShot(emoteSound);
            Debug.Log($"{stats.characterName} started emoting");
        }
        else if (context.canceled)
        {
            isEmoting = false;
            animController.SetEmote(false);
            Debug.Log($"{stats.characterName} stopped emoting");
        }
    }

    private IEnumerator ResetAttackCooldown()
    {
        yield return new WaitForSeconds(1f / stats.attacksPerSecond);
        canAttack = true;
    }

    private IEnumerator PerformSpecialAttackCoroutine()
    {
        isPerformingSpecialMovement = true;
        yield return characterScript.PerformSpecial(dmg =>
        {
            MeleeDamage activeMeleeDamage = characterType == CharacterType.Parasaurolophus
                ? ((Parasaurolophus)characterScript).SpecialMeleeDamage
                : facingRight ? rightMeleeDamage : leftMeleeDamage;

            activeMeleeDamage?.ApplyDamage(dmg, true, transform, this, characterType == CharacterType.Spinosaurus);
        });

        isPerformingSpecialMovement = false;
        yield return new WaitForSeconds(stats.specialAttackCost / 15f);
        canSpecial = true;
    }

    private IEnumerator ResetRevive()
    {
        yield return new WaitForSeconds(0.1f);
        animController.SetDowned(false);
    }

    private void PlayDeathSound(CombatManager combatManager)
    {
        soundPlayer.PlaySound(2);
    }
}