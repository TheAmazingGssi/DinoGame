/*using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer), typeof(Animator))]
[RequireComponent(typeof(PlayerCombatManager), typeof(PlayerMovement), typeof(PlayerAttack))]
[RequireComponent(typeof(PlayerStamina), typeof(PlayerRevive), typeof(PlayerScore), typeof(PlayerState))]
public class AltMainPlayerController : MonoBehaviour
{
    public enum CharacterType { Triceratops, Spinosaurus, Parasaurolophus, Therizinosaurus }

    [SerializeField] private CharacterStats characterStats;
    [SerializeField] private CharacterType characterType;
    [SerializeField] private PlayerTransformData playerTransform;
    [SerializeField] private GameObject rightMeleeColliderGO;
    [SerializeField] private GameObject leftMeleeColliderGO;

    private PlayerInputActions inputActions;
    private CharacterBase characterScript;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
        inputActions.Player.Move.performed += ctx => GetComponent<PlayerMovement>().Move(ctx);
        inputActions.Player.Move.canceled += ctx => GetComponent<PlayerMovement>().Move(ctx);
        inputActions.Player.Attack.performed += ctx => GetComponent<PlayerAttack>().Attack();
        inputActions.Player.Special.started += ctx => GetComponent<PlayerAttack>().SpecialStarted();
        inputActions.Player.Special.performed += ctx => GetComponent<PlayerAttack>().SpecialPerformed();
        inputActions.Player.Special.canceled += ctx => GetComponent<PlayerAttack>().SpecialCanceled();
        inputActions.Player.Block.performed += ctx => GetComponent<PlayerMovement>().Block();
        inputActions.Player.Revive.performed += ctx => GetComponent<PlayerRevive>().Revive();

        if (characterStats == null) Debug.LogError("CharacterStats not assigned!");
        var stats = characterStats.characters[(int)characterType];
        playerTransform.PlayerTransform = transform;

        switch (characterType)
        {
            case CharacterType.Triceratops: characterScript = gameObject.AddComponent<Triceratops>(); break;
            case CharacterType.Spinosaurus: characterScript = gameObject.AddComponent<Spinosaurus>(); break;
            case CharacterType.Parasaurolophus: characterScript = gameObject.AddComponent<Parasaurolophus>(); break;
            case CharacterType.Therizinosaurus: characterScript = gameObject.AddComponent<Therizinosaurus>(); break;
        }
        characterScript.Initialize(stats, rightMeleeColliderGO, leftMeleeColliderGO, true, 0.2f, 0.5f);

        GetComponent<PlayerCombatManager>().Initialize(stats.health, this, GetComponent<Animator>());
        GetComponent<PlayerStamina>().Initialize(stats);
        GetComponent<PlayerAttack>().Initialize(characterScript, characterType);
    }

    private void OnEnable() => inputActions.Player.Enable();
    private void OnDisable() => inputActions.Player.Disable();
}
*/