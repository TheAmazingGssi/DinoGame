// using UnityEngine;
// using UnityEngine.InputSystem;
//
// public class InputHandler : MonoBehaviour
// {
//     private PlayerInputActions inputActions;
//     private AnimationController animationController;
//     private MainPlayerController playerController;
//
//     public delegate void MoveAction(Vector2 direction);
//     public event MoveAction OnMove;
//     public event System.Action OnAttack;
//     public event System.Action OnSpecialStart;
//     public event System.Action OnSpecialPerformed;
//     public event System.Action OnSpecialCanceled;
//     public event System.Action OnBlock;
//     public event System.Action OnRevive;
//     //public event System.Action OnChoose;
//     public event System.Action OnPause;
//
//     public float specialHoldStartTime { get; private set; }
//
//     private void Awake()
//     {
//         animationController = GetComponent<AnimationController>();
//         playerController = GetComponent<MainPlayerController>();
//         inputActions = new PlayerInputActions();
//     }
//
//     private void OnEnable() => inputActions.Player.Enable();
//     private void OnDisable() => inputActions.Player.Disable();
//
//     private void Start()
//     {
//         inputActions.Player.Move.performed += ctx => OnMove?.Invoke(ctx.ReadValue<Vector2>());
//         inputActions.Player.Move.canceled += ctx => OnMove?.Invoke(Vector2.zero);
//         inputActions.Player.Attack.performed += ctx => OnAttack?.Invoke();
//         inputActions.Player.Special.started += ctx => 
//         {
//             if (!playerController.IsFallen()) 
//             {
//                 specialHoldStartTime = Time.time;
//                 OnSpecialStart?.Invoke();
//             }
//         };
//         inputActions.Player.Special.performed += ctx => OnSpecialPerformed?.Invoke();
//         inputActions.Player.Special.canceled += ctx => 
//         {
//             if (!playerController.IsFallen()) OnSpecialCanceled?.Invoke();
//         };
//         inputActions.Player.Block.performed += ctx => OnBlock?.Invoke();
//         inputActions.Player.Revive.performed += ctx => OnRevive?.Invoke();
//         //inputActions.Player.Choose.performed += ctx => OnChoose?.Invoke();
//         inputActions.Player.Pause.performed += ctx => OnPause?.Invoke();
//     }
//
//     public void UpdateMovement(Vector2 direction)
//     {
//         animationController.SetMoving(direction.magnitude > 0.1f);
//     }
// }