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
    }

    public CharacterData[] characters;
}