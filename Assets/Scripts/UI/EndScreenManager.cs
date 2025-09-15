using UnityEngine;

public class EndScreenManager : MonoBehaviour
{
    [SerializeField] GameObject earth;
    [SerializeField] GameObject mars;
    [SerializeField] GameObject moon;


    private void Start()
    {
        VoteEffectManager.Instance.ApplyStoredEffects(-1, -1);

        switch(VoteEffectManager.Instance.Ending)
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
    }
}
