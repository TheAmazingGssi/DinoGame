using UnityEngine;

[CreateAssetMenu(fileName = "CharacterStats", menuName = "ScriptableObjects/CharacterStats", order = 1)]
public class CharacterStats : ScriptableObject
{
    [System.Serializable]
    public class CharacterData
    {
        [Tooltip("Character type (e.g., Triceratops)")]
        public CharacterType characterType;
        [Tooltip("Name of the character")]
        public string characterName;
        [Tooltip("Maximum health points")]
        public float maxHealth;
        [Tooltip("Movement speed in units per second")]
        public float movementSpeed;
        [Tooltip("Minimum attack damage")]
        public float damageMin;
        [Tooltip("Maximum attack damage")]
        public float damageMax;
        [Tooltip("Attacks per second")]
        public float attacksPerSecond;
        [Tooltip("Damage dealt by special attack")]
        public float specialAttackDamage;
        [Tooltip("Stamina cost for special attack")]
        public float specialAttackCost;
        [Tooltip("Maximum stamina")]
        public float stamina;
        // Spinosaurus-specific fields
        [Tooltip("Minimum neck width for Spinosaurus special")]
        public float neckMinWidth = 0.075f;
        [Tooltip("Maximum neck width for Spinosaurus special")]
        public float neckMaxWidth = 0.45f;
        [Tooltip("Speed of neck extension/retraction for Spinosaurus")]
        public float chompSpeed = 8f;
        [Tooltip("Speed of pulling enemy during Spinosaurus special")]
        public float dragSpeed = 5f;
    }

    [SerializeField] public CharacterData[] characters;

    public CharacterData GetCharacterData(CharacterType type)
    {
        foreach (var character in characters)
        {
            if (character.characterType == type)
                return character;
        }
        Debug.LogWarning($"No stats found for CharacterType {type}");
        return new CharacterData();
    }

    private void OnEnable()
    {
        if (characters == null || characters.Length == 0)
        {
            Debug.LogWarning($"{name} has no character data! Initializing with empty array.");
            characters = new CharacterData[0];
        }
    }
}