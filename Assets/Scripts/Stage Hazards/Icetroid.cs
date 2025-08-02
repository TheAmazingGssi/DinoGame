using System;
using System.Collections;
using UnityEngine;

public class Icetroid : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float speed = 5f; 
    [SerializeField] float destroyTime = 3f;
    private bool explosionStarted = false;
    
    [Header("References")]
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Animator explosionAnimator;
    [SerializeField] GameObject explosionEffects;
    [SerializeField] GameObject FallEffects;

    private void Awake()
    {
        rb.linearVelocity = (Vector2.down + Vector2.left) * speed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.gameObject.name == "HurtBox")
        {
            other.GetComponentInParent<MainPlayerController>().ToggleFreezeEffect();
            StartCoroutine(EffectCoroutine());
        }
        else if(other.CompareTag("Ground"))
        {
            StartCoroutine(EffectCoroutine());
        }
    }
    
    private IEnumerator EffectCoroutine()
    {
        rb.constraints = RigidbodyConstraints2D.FreezePosition;
        FallEffects.SetActive(false);
        
        if (!explosionStarted)
        {
            explosionEffects.SetActive(true);
            explosionAnimator.SetTrigger("Explode");
            explosionStarted = true;
        }
        
        yield return new WaitForSeconds(destroyTime);
        Destroy(gameObject, destroyTime);
    }
}
