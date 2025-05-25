using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerCombatManager))]
public class MainPlayerController : MonoBehaviour
{
    public enum CharacterType
    {
        Triceratops,
        Spinosaurus,
        Parasaurolophus,
        Therizinosaurus
    }

    [Header("Character Settings")]
    [SerializeField] private CharacterStats characterStats;
    [SerializeField] private CharacterType characterType;
    [SerializeField] private bool facingRight = true;
    //[SerializeField] private GameOverUI gameOverUI;
    [SerializeField] private Animator animator;
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
    private Vector2 moveInput;
    private Vector2 currentVelocity;
    private PlayerInputActions inputActions;
    private float specialHoldStartTime;
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
        Debug.Log($"{stats.characterName} revived");
    }

    public void EnterFallenState()
    {
        isFallen = true;
        canAttack = false;
        canSpecial = false;
        canBlock = false;
        rb.linearVelocity = Vector2.zero;
        AddScore(-15);
        fallenPlayers++;
        Debug.Log($"{stats.characterName} has fallen, score: {score}");

        if (activePlayers == 1)
        {
           // gameOverUI.ShowGameOverPopup();
        }
        else if (activePlayers > 1 && activePlayers == fallenPlayers)
        {
           // gameOverUI.ShowGameOverPopup();
        }
    }

    private void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
        
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        playerTransform.PlayerTransform = transform;

        inputActions = new PlayerInputActions();
        inputActions.Player.Move.performed += Move;
        inputActions.Player.Move.canceled += Move;
        inputActions.Player.Attack.performed += Attack;
        inputActions.Player.Special.started += SpecialStarted;
        inputActions.Player.Special.performed += SpecialPerformed;
        inputActions.Player.Special.canceled += SpecialCanceled;
        inputActions.Player.Block.performed += Block;
        inputActions.Player.Revive.performed += Revive;

        if (characterStats != null)
        {
            stats = characterStats.characters[(int)characterType];
            stats.currentStamina = stats.stamina;
            Debug.Log($"Loaded stats for {stats.characterName}");
        }
        else
        {
            Debug.LogError("CharacterStats not assigned in Inspector!");
        }

        combatManager = GetComponent<PlayerCombatManager>();
        combatManager.Initialize(stats.health, this, animator);
        combatManager.OnDeath += (cm) => EnterFallenState();

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

        activePlayers++;
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
        activePlayers--;
    }

    private void Update()
    {
        if (!isFallen)
        {
            stats.currentStamina = Mathf.Min(stats.stamina, stats.currentStamina + staminaRegenRate * Time.deltaTime);
        }
    }

    private void FixedUpdate()
    {
        if (!isFallen)
        {
            HandleMovement();
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        if (!isFallen)
        {
            moveInput = context.ReadValue<Vector2>();
            Debug.Log("Move Input: " + moveInput);
        }
    }

    private void HandleMovement()
    {
        float effectiveMoveSpeed = isBlocking ? stats.movementSpeed * blockMoveSpeedMultiplier : stats.movementSpeed;
        Vector2 targetVelocity = moveInput.normalized * effectiveMoveSpeed;
        currentVelocity = Vector2.Lerp(currentVelocity, targetVelocity, stats.attacksPerSecond * Time.fixedDeltaTime);
        rb.linearVelocity = currentVelocity;

        // Flip sprite based on movement direction
        if (Mathf.Abs(moveInput.x) > 0.01f)
        {
            bool shouldFaceRight = moveInput.x > 0;
            if (shouldFaceRight != facingRight)
            {
                facingRight = shouldFaceRight;
                spriteRenderer.flipX = !facingRight;
                Debug.Log($"Flipped sprite to face {(facingRight ? "right" : "left")}");
            }
        }

        Debug.Log("Target Velocity: " + targetVelocity + ", Current Velocity: " + currentVelocity + ", RB Velocity: " + rb.linearVelocity);
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (canAttack && !isBlocking && !isFallen)
        {
            canAttack = false;
            lastAttackTime = Time.time;
            float damage = Random.Range(stats.damageMin, stats.damageMax);
            StartCoroutine(characterScript.PerformAttack(damage, stats.attackSequenceCount, (dmg) =>
            {
                GameObject activeCollider = facingRight ? rightMeleeColliderGO : leftMeleeColliderGO;
                MeleeDamage meleeDamage = activeCollider.GetComponent<MeleeDamage>();
                if (meleeDamage != null)
                    meleeDamage.ApplyDamage(dmg, false, transform, this);
            }));
            Debug.Log($"Attack Performed by {stats.characterName} for {damage} damage");
        }
    }

    private void SpecialStarted(InputAction.CallbackContext context)
    {
        if (!isFallen)
        {
            specialHoldStartTime = Time.time;
            Debug.Log("Special Hold Started");
        }
    }

    private void SpecialPerformed(InputAction.CallbackContext context)
    {
        if (canSpecial && !isBlocking && !isFallen && characterScript.CanPerformSpecial() && (Time.time - specialHoldStartTime >= stats.specialAttackCost / 30f))
        {
            canSpecial = false;
            lastSpecialTime = Time.time;
            characterScript.ConsumeSpecialStamina();
            StartCoroutine(characterScript.PerformSpecial((dmg) =>
            {
                GameObject activeCollider = facingRight ? rightMeleeColliderGO : leftMeleeColliderGO;
                if (characterType == CharacterType.Parasaurolophus)
                {
                    rightMeleeColliderGO.GetComponent<MeleeDamage>()?.ApplyDamage(dmg, true, transform, this);
                    leftMeleeColliderGO.GetComponent<MeleeDamage>()?.ApplyDamage(dmg, true, transform, this);
                }
                else
                {
                    MeleeDamage meleeDamage = activeCollider.GetComponent<MeleeDamage>();
                    if (meleeDamage != null)
                        meleeDamage.ApplyDamage(dmg, true, transform, this, characterType == CharacterType.Spinosaurus);
                }
            }));
            Debug.Log($"Special Attack Performed: {stats.specialAttackName}, Stamina: {stats.currentStamina}");
        }
    }

    private void SpecialCanceled(InputAction.CallbackContext context)
    {
        if (!isFallen)
        {
            Debug.Log("Special Hold Canceled");
        }
    }

    public void Block(InputAction.CallbackContext context)
    {
        if (canBlock && !isBlocking && !isFallen)
        {
            canBlock = false;
            lastBlockTime = Time.time;
            StartCoroutine(BlockDamageWindow());
            Debug.Log("Block Performed");
        }
    }

    public void Revive(InputAction.CallbackContext context)
    {
        if (!isFallen)
        {
            MainPlayerController target = FindNearestFallenPlayer();
            if (target != null)
            {
                //RevivePrompt prompt = gameObject.AddComponent<RevivePrompt>();
                //prompt.StartRevive(this, target);
                Debug.Log($"Started revive prompt for {target.stats.characterName}");
            }
        }
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
                if (distance <= minDistance)
                {
                    nearest = player;
                    minDistance = distance;
                }
            }
        }
        return nearest;
    }

    private IEnumerator BlockDamageWindow()
    {
        isBlocking = true;
        CanBeDamaged = false;
        yield return new WaitForSeconds(blockDuration);
        CanBeDamaged = true;
        isBlocking = false;
        
        float elapsedTime = Time.time - lastBlockTime;
        if (elapsedTime < stats.stamina / 25f)
            yield return new WaitForSeconds(stats.stamina / 25f - elapsedTime);
        canBlock = true;
    }
}