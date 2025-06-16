using UnityEngine;

[CreateAssetMenu(fileName = "AnimationData", menuName = "Scriptable Objects/AnimationData", order = 2)]
public class AnimationData : ScriptableObject
{
    [System.Serializable]
    public struct DinosaurAnimationSettings
    {
        public string dinosaurName;
        [Header("State Durations (Seconds)")]
        public float idleDuration;
        public float walkDuration;
        public float attackDuration;
        public float specialDuration;
        public float knockbackDuration;
        public float reviveBackDuration;
        public float reviveFrontDuration;
        public float downedDuration;

        [Header("Transition Settings")]
        public float transitionDuration;
        public float exitTime;

        [Header("Conditions")]
        public bool useIsMoving;
        public bool useIsDowned;
        public bool useIsBlocking;
        public bool useIsRevived;

        [Header("Speed Multipliers")]
        public float walkSpeedMultiplier;
        public float attackSpeedMultiplier;
        public float specialSpeedMultiplier;
    }

    public DinosaurAnimationSettings[] dinosaurSettings = new DinosaurAnimationSettings[4];

    private void OnEnable()
    {
        // Initialize default values if not set
        if (dinosaurSettings[0].dinosaurName == "")
        {
            dinosaurSettings[0] = new DinosaurAnimationSettings
            {
                dinosaurName = "Triceratops",
                idleDuration = 0.1f,
                walkDuration = 0.2f,
                attackDuration = 0.5f,
                specialDuration = 0.7f,
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
                walkSpeedMultiplier = 1.0f,
                attackSpeedMultiplier = 1.0f,
                specialSpeedMultiplier = 1.0f
            };
        }

        if (dinosaurSettings[1].dinosaurName == "")
        {
            dinosaurSettings[1] = new DinosaurAnimationSettings
            {
                dinosaurName = "Spinosaurus",
                idleDuration = 0.1f,
                walkDuration = 0.2f,
                attackDuration = 0.6f, // Slightly longer due to 3-hit sequence
                specialDuration = 0.8f, // Longer for grab animation
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
                walkSpeedMultiplier = 1.0f,
                attackSpeedMultiplier = 1.0f,
                specialSpeedMultiplier = 1.0f
            };
        }

        if (dinosaurSettings[2].dinosaurName == "")
        {
            dinosaurSettings[2] = new DinosaurAnimationSettings
            {
                dinosaurName = "Parasaurolophus",
                idleDuration = 0.1f,
                walkDuration = 0.2f,
                attackDuration = 0.5f,
                specialDuration = 0.7f, // AOE roar
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
                walkSpeedMultiplier = 1.0f,
                attackSpeedMultiplier = 1.0f,
                specialSpeedMultiplier = 1.0f
            };
        }

        if (dinosaurSettings[3].dinosaurName == "")
        {
            dinosaurSettings[3] = new DinosaurAnimationSettings
            {
                dinosaurName = "Therizinosaurus",
                idleDuration = 0.1f,
                walkDuration = 0.2f,
                attackDuration = 0.4f, // Faster due to 3 attacks/s
                specialDuration = 0.6f, // Shorter for claw flurry
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
                walkSpeedMultiplier = 1.0f,
                attackSpeedMultiplier = 1.0f,
                specialSpeedMultiplier = 1.0f
            };
        }
    }
}