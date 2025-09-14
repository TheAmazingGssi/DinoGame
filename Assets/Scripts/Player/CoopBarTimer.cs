using UnityEngine;

public class CoopBarTimer : MonoBehaviour
{
    public static CoopBarTimer Instance;
    public float MaxFill { get; private set; } = 60;
    public float CurrentFill { get; private set; } = 0;

    public int PlayersTryingToUlt = 0;

    private void Awake()
    {
        if(Instance)
            Destroy(Instance);

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
}
