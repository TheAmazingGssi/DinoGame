using UnityEngine;

[CreateAssetMenu(fileName = "AnimationData", menuName = "ScriptableObjects/AnimationData", order = 2)]
public class AnimationData : ScriptableObject
{
    [System.Serializable]
    public struct DinosaurAnimationSettings
    {
        [Tooltip("Name of the dinosaur (e.g., Triceratops)")]
        public string dinosaurName;

        [Header("State Durations (Seconds)")]
        [Tooltip("Duration of idle animation")]
        public float idleDuration;
        [Tooltip("Duration of walk animation")]
        public float walkDuration;
        [Tooltip("Duration of attack animation")]
        public float attackDuration;
        [Tooltip("Duration of special attack animation")]
        public float specialDuration;
        [Tooltip("Duration of knockback animation")]
        public float knockbackDuration;
        [Tooltip("Duration of revive animation (back)")]
        public float reviveBackDuration;
        [Tooltip("Duration of revive animation (front)")]
        public float reviveFrontDuration;
        [Tooltip("Duration of downed animation")]
        public float downedDuration;

        [Header("Transition Settings")]
        [Tooltip("Duration of animation transitions")]
        public float transitionDuration;
        [Tooltip("Normalized exit time for transitions")]
        public float exitTime;

        [Header("Conditions")]
        [Tooltip("Use IsMoving condition in Animator")]
        public bool useIsMoving;
        [Tooltip("Use IsDowned condition in Animator")]
        public bool useIsDowned;
        [Tooltip("Use IsBlocking condition in Animator")]
        public bool useIsBlocking;
        [Tooltip("Use IsRevived condition in Animator")]
        public bool useIsRevived;
        [Tooltip("Use SpinoSpecial trigger for Spinosaurus")]
        public bool useSpinoSpecial;

        [Header("Speed Multipliers")]
        [Tooltip("Speed multiplier for walk animation")]
        public float walkSpeedMultiplier;
        [Tooltip("Speed multiplier for attack animation")]
        public float attackSpeedMultiplier;
        [Tooltip("Speed multiplier for special animation")]
        public float specialSpeedMultiplier;
    }

    [SerializeField] public DinosaurAnimationSettings[] dinosaurSettings;

    private void OnEnable()
    {
        // Ensure array is initialized with default settings for 4 dinosaurs
        if (dinosaurSettings == null || dinosaurSettings.Length != 4)
        {
            dinosaurSettings = new DinosaurAnimationSettings[4];
        }

        // Initialize default settings if not set
        InitializeSettings(0, "Triceratops", 0.5f, 0.7f, false);
        InitializeSettings(1, "Spinosaurus", 0.6f, 0.8f, true);
        InitializeSettings(2, "Parasaurolophus", 0.5f, 0.7f, false);
        InitializeSettings(3, "Therizinosaurus", 0.4f, 0.6f, false);
    }

    private void InitializeSettings(int index, string name, float attackDuration, float specialDuration, bool useSpinoSpecial)
    {
        if (string.IsNullOrEmpty(dinosaurSettings[index].dinosaurName))
        {
            dinosaurSettings[index] = new DinosaurAnimationSettings
            {
                dinosaurName = name,
                idleDuration = 0.1f,
                walkDuration = 0.2f,
                attackDuration = attackDuration,
                specialDuration = specialDuration,
                knockbackDuration = 0.5f,
                reviveBackDuration = 0.6f,
                reviveFrontDuration = 0.6f,
                downedDuration = 0.8f,
                transitionDuration = 0.1f,
                exitTime = 0.9f,
                useIsMoving = true,
                useIsDowned = true,
                useIsBlocking = true,
                useIsRevived = true,
                useSpinoSpecial = useSpinoSpecial,
                walkSpeedMultiplier = 1.0f,
                attackSpeedMultiplier = 1.0f,
                specialSpeedMultiplier = 1.0f
            };
        }
    }
}