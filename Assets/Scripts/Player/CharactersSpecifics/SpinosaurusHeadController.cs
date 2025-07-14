using UnityEngine;

public class SpinosaurusHeadController : MonoBehaviour
{
    [SerializeField] private Animator headAnimator;

    private int mouthOpenHash;
    private int mouthCloseHash;

    private void Awake()
    {
        headAnimator = GetComponent<Animator>();
        if (headAnimator == null)
            Debug.LogError($"Head Animator missing on {gameObject.name}!");

        mouthOpenHash = Animator.StringToHash("MouthOpen");
        mouthCloseHash = Animator.StringToHash("MouthClose");
    }

    public void PlayMouthOpen()
    {
        headAnimator.Play(mouthOpenHash, 0, 0f);
        headAnimator.Update(0f); // Ensure immediate update
        headAnimator.Play(mouthOpenHash, 0, 1f); // Hold at last frame
    }

    public void PlayMouthClose()
    {
        headAnimator.Play(mouthCloseHash, 0, 0f);
    }
}