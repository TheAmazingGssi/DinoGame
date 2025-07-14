using UnityEngine;

[CreateAssetMenu(fileName = "CharacterMapping", menuName = "ScriptableObjects/CharacterMapping", order = 3)]
public class CharacterMapping : ScriptableObject
{
    [System.Serializable]
    public struct CharacterEntry
    {
        [Tooltip("Character type (e.g., Triceratops)")]
        public CharacterType characterType;
        [Tooltip("Script component for this character")]
        public MonoScript characterScript;
    }

    [SerializeField] private CharacterEntry[] characterEntries;

    public System.Type GetCharacterScript(CharacterType type)
    {
        foreach (var entry in characterEntries)
        {
            if (entry.characterType == type)
                return entry.characterScript?.GetClass();
        }
        Debug.LogWarning($"No script found for CharacterType {type}");
        return null;
    }

    private void OnEnable()
    {
        if (characterEntries == null || characterEntries.Length == 0)
        {
            Debug.LogWarning($"{name} has no character mappings! Initializing empty array.");
            characterEntries = new CharacterEntry[0];
        }
    }
}