using System;
using System.Collections;
using UnityEngine;

public class Icetroid : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float speed = 10f; 
    [SerializeField] float destroyTime = 1f;
    private bool explosionStarted = false;
    
    [Header("References")]
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Collider2D col;
    [SerializeField] Animator explosionAnimator;
    [SerializeField] GameObject explosionEffects;
    [SerializeField] GameObject FallEffects;

    public event Action OnDestroyed; 

    
    private void Awake()
    {
        rb.linearVelocity = (Vector2.down + Vector2.left) * speed;
        Destroy(gameObject, destroyTime * 3);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.gameObject.name == "HurtBox")
        {
            other.GetComponentInParent<MainPlayerController>().ActivateFreezeEffect();
            Impact();
        }
        else if(other.CompareTag("Ground"))
        {
            Impact();
        }
    }
    
    private void Impact()
    {
        col.enabled = false;
        rb.constraints = RigidbodyConstraints2D.FreezePosition;
        FallEffects.SetActive(false);
        
        if (!explosionStarted)
        {
            explosionEffects.SetActive(true);
            explosionAnimator.SetTrigger("Play");
            explosionStarted = true;
        }
        
        Destroy(gameObject, destroyTime);
    }
    
    private void OnDestroy()
    {
        OnDestroyed?.Invoke(); 
    }
}
