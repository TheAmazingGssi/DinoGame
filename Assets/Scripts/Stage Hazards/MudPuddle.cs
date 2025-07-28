using System;
using UnityEngine;

public class MudPuddle : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.gameObject.name == "FeetCollider") 
            other.gameObject.GetComponentInParent<MainPlayerController>().ToggleMudSlowEffect();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")  && other.gameObject.name == "FeetCollider")
          other.gameObject.GetComponentInParent<MainPlayerController>().ToggleMudSlowEffect();
    }
}
