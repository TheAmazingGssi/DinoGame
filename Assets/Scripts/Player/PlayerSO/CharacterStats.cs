using UnityEngine;

[CreateAssetMenu(fileName = "CharacterStats", menuName = "ScriptableObjects/Character Stats", order = 1)]
public class CharacterStats : ScriptableObject
{
    [System.Serializable]
    public struct CharacterData
    {
        public string characterName;
        public float maxHealth;
        public float movementSpeed;
        public float stamina;
        public float currentStamina;
        public float damageMin;
        public float damageMax;
        public float attacksPerSecond;
        public int attackSequenceCount;
        public float specialAttackDamage;
        public float specialAttackCost;
        public string specialAttackName;
    }

    public CharacterData[] characters = new CharacterData[4];

    private void OnEnable()
    {
        // Initialize only if characters array is empty or unconfigured
        if (characters[0].characterName == "")
        {
            // Triceratops (Terry)
            characters[0] = new CharacterData
            {
                characterName = "Terry",
                maxHealth = 100f,
                movementSpeed = 4f,
                stamina = 50f,
                currentStamina = 50f,
                damageMin = 15f,
                damageMax = 20f,
                attacksPerSecond = 1.5f,
                attackSequenceCount = 2,
                specialAttackDamage = 20f,
                specialAttackCost = 15f,
                specialAttackName = "Forward Bash"
            };
        }

        if (characters[1].characterName == "")
        {
            // Spinosaurus (Spencer)
            characters[1] = new CharacterData
            {
                characterName = "Spencer",
                maxHealth = 100f,
                movementSpeed = 4f,
                stamina = 50f,
                currentStamina = 50f,
                damageMin = 10f,
                damageMax = 10f,
                attacksPerSecond = 1f,
                attackSequenceCount = 3,
                specialAttackDamage = 30f,
                specialAttackCost = 15f,
                specialAttackName = "Chomp"
            };
        }

        if (characters[2].characterName == "")
        {
            // Parasaurolophus (Paris)
            characters[2] = new CharacterData
            {
                characterName = "Paris",
                maxHealth = 100f,
                movementSpeed = 4f,
                stamina = 50f,
                currentStamina = 50f,
                damageMin = 15f,
                damageMax = 20f,
                attacksPerSecond = 1f,
                attackSequenceCount = 3,
                specialAttackDamage = 20f,
                specialAttackCost = 30f,
                specialAttackName = "Roar"
            };
        }

        if (characters[3].characterName == "")
        {
            // Therizinosaurus (Andrew)
            characters[3] = new CharacterData
            {
                characterName = "Andrew",
                maxHealth = 100f,
                movementSpeed = 4f,
                stamina = 50f,
                currentStamina = 50f,
                damageMin = 5f,
                damageMax = 7f,
                attacksPerSecond = 3f,
                attackSequenceCount = 2,
                specialAttackDamage = 20f, // Total for 4x5
                specialAttackCost = 15f,
                specialAttackName = "Claw Attack"
            };
        }
    }
}