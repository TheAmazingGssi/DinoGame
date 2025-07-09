using UnityEngine;

[CreateAssetMenu(fileName = "CharacterStats", menuName = "AstroSaurs/CharacterStats")]
public class CharacterStats : ScriptableObject
{
    [System.Serializable]
    public struct CharacterData
    {
        public string characterName;
        public float maxHealth; // Player's health; depletes to trigger downed state until revived
        public float movementSpeed; // Player's movement speed
        public float damageMin; // Lower margin for normal attack damage range
        public float damageMax; // Upper margin for normal attack damage range
        public float attacksPerSecond; // Normal attack speed (attacks per second)
        public float specialAttackDamage; // Damage dealt by special attack
        public float specialAttackCost; // Stamina cost for special attack
        public float stamina; // Maximum stamina for special attacks
        public float SPspecialAttackRange; // Spinosaurus: Neck extension distance
        public float SPchompSpeed; // Spinosaurus: Neck extension/retraction speed
        public float SPmeleeRange; // Spinosaurus: Distance to pull enemy
        public float SPdragSpeed; // Spinosaurus: Enemy pull speed
    }
    public CharacterData[] characters;
}