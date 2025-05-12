using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerEntity : MonoBehaviour
{
    [SerializeField] GameObject CharacterSelector;
    [SerializeField] PlayerInput playerInput;

    private UnityEvent<InputAction.CallbackContext> Move = new UnityEvent<InputAction.CallbackContext>();
    private UnityEvent Attack = new UnityEvent();
    private UnityEvent Special = new UnityEvent();
    private UnityEvent Emote = new UnityEvent();
    private UnityEvent Block = new UnityEvent();
    private UnityEvent Revive = new UnityEvent();

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
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
        Destroy(gameObject);
    }

    public CharacterSelect SpawnCharacterSelector()
    {
        CharacterSelect selector = Instantiate(CharacterSelector).GetComponent<CharacterSelect>();
        selector.PlayerInput = playerInput;
        Move.AddListener(selector.OnNavigate);
        Emote.AddListener(selector.OnXPressed);
        
        

        return selector;
    }
}
