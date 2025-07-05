using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //private List<VoteEffectBase> effects = new List<VoteEffectBase>();

    [SerializeField] VotingSystem stagesVote;
    [SerializeField] Vote vote;

    public static GameManager Instance;
    private int enemiesOnStage = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!Instance && Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void IncrementDeathCount()
    {
        enemiesOnStage--;
        if(enemiesOnStage <= 0)
        {
            StartVote();
        }
    }
    public void SetWaveSize(int amount)
    {
        enemiesOnStage += amount;
    }

    public void StartVote()
    {
        stagesVote.StartVote(vote);
    }
}
