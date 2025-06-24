using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerEntity : MonoBehaviour
{
    static public List<PlayerEntity> PlayerList = new List<PlayerEntity>();

    [Header("Prefab Refrences")]
    [SerializeField] GameObject CharacterSelectorObject;
    [SerializeField] GameObject TerryPlayerPrefab;
    [SerializeField] GameObject SpencerPlayerPrefab;
    [SerializeField] GameObject ParisPlayerPrefab;
    [SerializeField] GameObject AndrewPlayerPrefab;
    [SerializeField] GameObject MultiplayerUIControllerObject;
    [SerializeField] PlayerInput playerInput;

    CharacterSelect selector;
    MainPlayerController controller;
    MultiplayerUIController uiController;

    //public Color PlayerColor;
    public CharacterType CharacterType;

    public UnityEvent<InputAction.CallbackContext> Move = new UnityEvent<InputAction.CallbackContext>();
    public UnityEvent<InputAction.CallbackContext> Attack = new UnityEvent<InputAction.CallbackContext>();
    public UnityEvent<InputAction.CallbackContext> Special = new UnityEvent<InputAction.CallbackContext>();
    public UnityEvent<InputAction.CallbackContext> Block = new UnityEvent<InputAction.CallbackContext>();
    public UnityEvent<InputAction.CallbackContext> Revive = new UnityEvent<InputAction.CallbackContext>();
    public UnityEvent<InputAction.CallbackContext> Confirmation = new UnityEvent<InputAction.CallbackContext>();
    public UnityEvent<InputAction.CallbackContext> Cancel = new UnityEvent<InputAction.CallbackContext>();
    public UnityEvent<InputAction.CallbackContext> Pause = new UnityEvent<InputAction.CallbackContext>();

    public Transform GetCharactersTransform { get
        {
            if (controller)
                return controller.transform;
            else
                return null;
        } }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        PlayerList.Add(this);
    }

    [HideInInspector] public void InvokeMove(InputAction.CallbackContext inputContext) => Move.Invoke(inputContext);
    [HideInInspector] public void InvokeAttack(InputAction.CallbackContext inputContext) => Attack.Invoke(inputContext);
    [HideInInspector] public void InvokeSpecial(InputAction.CallbackContext inputContext) => Special.Invoke(inputContext);
    [HideInInspector] public void InvokeBlock(InputAction.CallbackContext inputContext) => Block.Invoke(inputContext);
    [HideInInspector] public void InvokeRevive(InputAction.CallbackContext inputContext) => Revive.Invoke(inputContext);
    [HideInInspector] public void InvokeConfirmation(InputAction.CallbackContext inputContext) => Confirmation.Invoke(inputContext);
    [HideInInspector] public void InvokeCancel(InputAction.CallbackContext inputContext) => Cancel.Invoke(inputContext);
    [HideInInspector] public void InvokePause(InputAction.CallbackContext inputContext) => Pause.Invoke(inputContext);

    public void DeviceDisconnected()
    {
        PlayerList.Remove(this);
        Destroy(gameObject);
    }

    private void SetCharacterInformation()
    {
        CharacterType = selector.SelectedCharacter;
    }

    //Spawners
    public CharacterSelect SpawnCharacterSelector()
    {
        selector = Instantiate(CharacterSelectorObject).GetComponent<CharacterSelect>();
        //Set Events
        Move.AddListener(selector.OnNavigate);
        Confirmation.AddListener(selector.OnXPressed);
        Cancel.AddListener(selector.OnCancel);
        selector.FinalizeSelection.AddListener(SetCharacterInformation);
        return selector;
    }
    public MainPlayerController SpawnPlayerController(Transform transform)
    {
        controller = Instantiate(PickPlayerPrefab(CharacterType), transform.position, transform.rotation).GetComponent<MainPlayerController>();
        
        //Set Events
        Move.AddListener(controller.Move);
        Attack.AddListener(controller.Attack);
        Block.AddListener(controller.Block);
        Special.AddListener(controller.SpecialStarted);
        Revive.AddListener(controller.Revive);

        return controller;
    }
    private GameObject PickPlayerPrefab(CharacterType character)
    {
        if (character == CharacterType.Triceratops) return TerryPlayerPrefab;
        else if (character == CharacterType.Spinosaurus) return SpencerPlayerPrefab;
        else if (character == CharacterType.Parasaurolophus) return ParisPlayerPrefab;
        else if (character == CharacterType.Therizinosaurus) return AndrewPlayerPrefab;
        else return null;
    }
    public MultiplayerUIController SpawnUIController(MultiplayerButton defaultButton)
    {
        if (!uiController)
        {
            uiController = Instantiate(MultiplayerUIControllerObject).GetComponent<MultiplayerUIController>();
            Move.AddListener(uiController.OnNavigate);
            Confirmation.AddListener(uiController.OnConrfimPressed);
        }
        uiController.SetUp(CharacterType, defaultButton);

        return uiController;
    }
}
