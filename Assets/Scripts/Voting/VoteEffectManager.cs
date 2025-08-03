using System.Collections.Generic;
using UnityEngine;

public class VoteEffectManager : MonoBehaviour
{
    public static VoteEffectManager Instance;

    private Dictionary<Vote, int> nextLevelEffects = new Dictionary<Vote, int>();
    private Dictionary<Vote, int> finaleLevelEffects = new Dictionary<Vote, int>();

    public int StoredLevelNumber { get; private set; } = 1;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void StoreNextLevelEffect(Vote vote, int choice)
    {
        if (!nextLevelEffects.ContainsKey(vote))
            nextLevelEffects.Add(vote, choice);
    }

    public void StoreFinaleLevelEffect(Vote vote, int choice)
    {
        if (!finaleLevelEffects.ContainsKey(vote))
            finaleLevelEffects.Add(vote, choice);
    }

    public void ApplyStoredEffects(int currentLevel, int finaleLevel)
    {
        if (currentLevel == finaleLevel)
        {
            foreach (var pair in finaleLevelEffects)
                pair.Key.ApplyEffects(pair.Value);
            finaleLevelEffects.Clear();
        }

        if (currentLevel > StoredLevelNumber)
        {
            foreach (var pair in nextLevelEffects)
                pair.Key.ApplyEffects(pair.Value);
            nextLevelEffects.Clear();
        }

        StoredLevelNumber = currentLevel;
    }

    public void ClearAll()
    {
        nextLevelEffects.Clear();
        finaleLevelEffects.Clear();
    }
}
