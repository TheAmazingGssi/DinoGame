using UnityEngine;

public class CoopBarTimer : MonoBehaviour
{
    [HideInInspector] public static CoopBarTimer Instance { get; private set; }
    [field: SerializeField] public float MaxFill { get; private set; } = 5;
    public float CurrentFill { get; private set; } = 0;

    [HideInInspector] public int PlayersTryingToUlt = 0;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }


    private void Update()
    {
        CurrentFill += Time.deltaTime;

        if(PlayersTryingToUlt == PlayerEntity.PlayerList.Count && CurrentFill >= MaxFill)
        {
            Debug.Log("Do Ult");
            CurrentFill = -10;
            PlayerEntity.PlayerList[0].MainPlayerController.StartCoopActualAttack();
        }
    }
    public void ResetFill()
    {
        CurrentFill = 0;
    }
}
