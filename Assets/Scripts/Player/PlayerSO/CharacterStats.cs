using UnityEngine;

[CreateAssetMenu(fileName = "CharacterStats", menuName = "Characters/CharacterStats")]
public class CharacterStats : ScriptableObject
{
    [System.Serializable]
    public struct CharacterData
    {
        public string characterName;
        public float maxHealth;
        public float stamina;
        public float movementSpeed;
        public float attacksPerSecond;
        public float damageMin;
        public float damageMax;
        public float specialAttackCost;
        public float specialAttackDamage;
        public float mudSlowFactor;
        [Header("Spinosaurus Specifics")]
        public float neckMinWidth; // Added for Spinosaurus (e.g., 0.075)
        public float neckMaxWidth; // Added for Spinosaurus (e.g., 0.45)
        public float chompSpeed; // Added for Spinosaurus (e.g., 8)
        public float dragSpeed; // Added for Spinosaurus (e.g., 5)
    }

    public CharacterData[] characters;
}