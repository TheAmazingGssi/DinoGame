using UnityEngine;

[CreateAssetMenu(fileName = "CharacterStats", menuName = "AstroSaurs/CharacterStats")]
public class CharacterStats : ScriptableObject
{
    [System.Serializable]
    public struct CharacterData
    {
        public string characterName;
        public float maxHealth;
        public float movementSpeed;
        public float damageMin;
        public float damageMax;
        public float attacksPerSecond;
        public int attackSequenceCount;
        public float specialAttackDamage;
        public float specialAttackCost;
        public float stamina;
    }
    public CharacterData[] characters;
}