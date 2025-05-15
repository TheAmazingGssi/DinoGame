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
    private UnityEvent Attack = new UnityEvent();
    private UnityEvent Special = new UnityEvent();
    private UnityEvent Emote = new UnityEvent();
    private UnityEvent Block = new UnityEvent();
    private UnityEvent Revive = new UnityEvent();

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        PlayerList.Add(this);
    }

    public void InvokeMove(InputAction.CallbackContext inputContext)
    {
        Move.Invoke(inputContext);
    }
    public void InvokeAttack() => Attack.Invoke();
    public void InvokeSpecial() => Special.Invoke();
    public void InvokeEmote() => Emote.Invoke();
    public void InvokeBlock() => Block.Invoke();
    public void InvokeRevive() => Revive.Invoke();

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
