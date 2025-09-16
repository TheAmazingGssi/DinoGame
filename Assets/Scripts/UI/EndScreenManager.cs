using UnityEngine;
using UnityEngine.InputSystem;

public class EndScreenManager : MonoBehaviour
{
    [SerializeField] GameObject earth;
    [SerializeField] GameObject mars;
    [SerializeField] GameObject moon;

    [SerializeField] GameObject terry;
    [SerializeField] GameObject spencer;
    [SerializeField] GameObject paris;
    [SerializeField] GameObject andrew;


    private void Start()
    {
        VoteEffectManager.Instance.ApplyStoredEffects(-1, -1);

        terry.SetActive(false);
        spencer.SetActive(false);
        paris.SetActive(false);
        andrew.SetActive(false);

        switch (VoteEffectManager.Instance.Ending)
        {
            case Ending.Mars:
                earth.SetActive(false);
                moon.SetActive(false);
                mars.SetActive(true);
                break;
            case Ending.Moon:
                earth.SetActive(false);
                moon.SetActive(true);
                mars.SetActive(false);
                break;
            case Ending.Space:
                earth.SetActive(false);
                moon.SetActive(false);
                mars.SetActive(false);
                break;
        }
    
        foreach (PlayerEntity player in PlayerEntity.PlayerList)
        {
            switch(player.CharacterType)
            {
                case CharacterType.Triceratops:
                    terry.SetActive(true);
                    break;
                case CharacterType.Parasaurolophus:
                    paris.SetActive(true);
                    break;
                case CharacterType.Spinosaurus: 
                    spencer.SetActive(true);
                    break;
                case CharacterType.Therizinosaurus:
                    andrew.SetActive(true);
                    break;
            }
        }

    }
}
