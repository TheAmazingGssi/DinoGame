using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerEntity : MonoBehaviour
{
    static public List<PlayerEntity> PlayerList = new List<PlayerEntity>();

    [Header("Prefab Refrences")]
    [SerializeField] GameObject CharacterSelectorObject;
    [SerializeField] GameObject PlayerGameObject;
    [SerializeField] PlayerInput playerInput;

    CharacterSelect selector;
    PlayerController controller;

    public Color PlayerColor;

    private UnityEvent<InputAction.CallbackContext> Move = new UnityEvent<InputAction.CallbackContext>();
    private UnityEvent<InputAction.CallbackContext> Attack = new UnityEvent<InputAction.CallbackContext>();
    private UnityEvent<InputAction.CallbackContext> Special = new UnityEvent<InputAction.CallbackContext>();
    private UnityEvent<InputAction.CallbackContext> Emote = new UnityEvent<InputAction.CallbackContext>();
    private UnityEvent<InputAction.CallbackContext> Block = new UnityEvent<InputAction.CallbackContext>();
    private UnityEvent<InputAction.CallbackContext> Revive = new UnityEvent<InputAction.CallbackContext>();

    public Transform GetCharactersTransform { get
        {
            if (controller)
                return controller.transform;
            else
                return null;
        } }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        PlayerList.Add(this);
    }

    public void InvokeMove(InputAction.CallbackContext inputContext)
    {
        Move.Invoke(inputContext);
    }
    public void InvokeAttack(InputAction.CallbackContext inputContext) => Attack.Invoke(inputContext);
    public void InvokeSpecial(InputAction.CallbackContext inputContext) => Special.Invoke(inputContext);
    public void InvokeEmote(InputAction.CallbackContext inputContext) => Emote.Invoke(inputContext);
    public void InvokeBlock(InputAction.CallbackContext inputContext) => Block.Invoke(inputContext);
    public void InvokeRevive(InputAction.CallbackContext inputContext) => Revive.Invoke(inputContext);

    public void DeviceDisconnected()
    {
        PlayerList.Remove(this);
        Destroy(gameObject);
    }

    private void SetCharacterInformation()
    {
        if(selector)
        {
            PlayerColor = selector.Color;
        }
    }

    public CharacterSelect SpawnCharacterSelector()
    {
        selector = Instantiate(CharacterSelectorObject).GetComponent<CharacterSelect>();
        //Set script refrences
        selector.PlayerInput = playerInput;
        //Set Events
        Move.AddListener(selector.OnNavigate);
        Emote.AddListener(selector.OnXPressed);
        selector.FinalizeSelection.AddListener(SetCharacterInformation);
        return selector;
    }

    public PlayerController SpawnPlayerController(Transform transform)
    {
        controller = Instantiate(PlayerGameObject, transform.position, transform.rotation).GetComponent<PlayerController>();
        
        //Set Events
        Move.AddListener(controller.OnMove);
        Attack.AddListener(controller.OnAttack);
        Emote.AddListener(controller.OnJump);
        Block.AddListener(controller.OnBlock);
        Special.AddListener(controller.OnSpecial);

        //Set Player
        controller.GetComponentInChildren<SpriteRenderer>().color = PlayerColor;

        return controller;
    }
}
