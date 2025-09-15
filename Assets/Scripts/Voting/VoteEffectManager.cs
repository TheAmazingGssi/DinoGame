using System.Collections.Generic;
using UnityEngine;

public enum Ending
{
    Space,
    Moon,
    Mars
}

public class VoteEffectManager : MonoBehaviour
{
    public static VoteEffectManager Instance;
    private Dictionary<Vote, int> nextLevelEffects = new Dictionary<Vote, int>();
    private Dictionary<Vote, int> finaleLevelEffects = new Dictionary<Vote, int>();
    private int lastLevel = -1;

    public Ending Ending;

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
        {
            nextLevelEffects.Add(vote, choice);
        }
        lastLevel = GameManager.Instance.LevelNumber;
    }

    public void StoreFinaleLevelEffect(Vote vote, int choice)
    {
        if (!finaleLevelEffects.ContainsKey(vote))
        {
            finaleLevelEffects.Add(vote, choice);
        }
    }

    public void ApplyStoredEffects(int currentLevel, int finaleLevel)
    {
        if (currentLevel == finaleLevel)
        {
            foreach (var pair in finaleLevelEffects)
            {
                pair.Key.ApplyEffects(pair.Value);
            }
            finaleLevelEffects.Clear();
        }

        if (lastLevel != -1 && currentLevel == lastLevel + 1)
        {
            foreach (var pair in nextLevelEffects)
            {
                pair.Key.ApplyEffects(pair.Value);
            }
            nextLevelEffects.Clear();
            lastLevel = -1;
        }

        if(currentLevel == -1)
        {
            foreach (var pair in finaleLevelEffects)
            {
                pair.Key.ApplyEffects(pair.Value);
            }
        }

    }

    public void ClearAll()
    {
        nextLevelEffects.Clear();
        finaleLevelEffects.Clear();
        lastLevel = -1;
    }
}