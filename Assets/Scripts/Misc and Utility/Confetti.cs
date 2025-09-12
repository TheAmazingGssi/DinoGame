using System;
using UnityEngine;

public class Confetti: MonoBehaviour
{
    [SerializeField] private ParticleSystem[] confetti;

    private void Start()
    {
        GameManager.OnLevelEnd += ActivateConfetti;
    }

    private void OnDestroy()
    {
        GameManager.OnLevelEnd -= ActivateConfetti;
    }
    private void ActivateConfetti(MainPlayerController controller)
    {
        for (int i = 0; i < confetti.Length; i++)
            confetti[i].Play();
    }
}
