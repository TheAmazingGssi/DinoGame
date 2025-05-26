using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterStats", menuName = "ScriptableObjects/CharacterStats", order = 1)]
public class CharacterStats : ScriptableObject
{
    [Serializable]
    public struct CharacterData
    {
        public string characterName;
        public float health;
        public float movementSpeed;
        public float stamina;
        [HideInInspector] public float currentStamina; // Runtime
        public float damageMin;
        public float damageMax;
        public int attackSequenceCount;
        public float attacksPerSecond;
        public string specialAttackName;
        public float specialAttackDamage;
        [TextArea] public string specialAttackDescription;
        public float specialAttackCost;
    }

    public List<CharacterData> characters = new List<CharacterData>
    {
        new CharacterData
        {
            characterName = "Triceratops (Terry)",
            health = 100f,
            movementSpeed = 4f,
            stamina = 50f,
            currentStamina = 50f,
            damageMin = 15f,
            damageMax = 20f,
            attackSequenceCount = 2,
            attacksPerSecond = 1.5f,
            specialAttackName = "Forward Bash",
            specialAttackDamage = 20f,
            specialAttackDescription = "Bash forward a strong attack that knocks enemies back you bash into.",
            specialAttackCost = 15f
        },
        new CharacterData
        {
            characterName = "Spinosaurus (Spencer)",
            health = 100f,
            movementSpeed = 4f,
            stamina = 50f,
            currentStamina = 50f,
            damageMin = 10f,
            damageMax = 10f,
            attackSequenceCount = 3,
            attacksPerSecond = 1f,
            specialAttackName = "Chomp",
            specialAttackDamage = 20f,
            specialAttackDescription = "Bite an enemy from afar and grab it.",
            specialAttackCost = 15f
        },
        new CharacterData
        {
            characterName = "Parasaurolophus (Paris)",
            health = 100f,
            movementSpeed = 4f,
            stamina = 50f,
            currentStamina = 50f,
            damageMin = 15f,
            damageMax = 20f,
            attackSequenceCount = 3,
            attacksPerSecond = 1f,
            specialAttackName = "Roar Attack",
            specialAttackDamage = 20f,
            specialAttackDescription = "Roar in front of you for 20 damage, knocking back enemies hit.",
            specialAttackCost = 15f
        },
        new CharacterData
        {
            characterName = "Therizinosaurus (Andrew)",
            health = 100f,
            movementSpeed = 4f,
            stamina = 50f,
            currentStamina = 50f,
            damageMin = 5f,
            damageMax = 7f,
            attackSequenceCount = 2,
            attacksPerSecond = 3f,
            specialAttackName = "Claw Attack",
            specialAttackDamage = 20f,
            specialAttackDescription = "Slash enemies in front of you 4 times for 5 damage, knocking back enemies you hit.",
            specialAttackCost = 15f
        }
    };
}