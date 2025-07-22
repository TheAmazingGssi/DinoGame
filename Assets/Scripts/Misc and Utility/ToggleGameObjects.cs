using UnityEngine;

public class ToggleGameObjects : MonoBehaviour
{
    [SerializeField] private GameObject obj;
    
    public void Toggle()
    {
        if (obj != null)
        {
            obj.SetActive(!obj.activeSelf);
        }
        else
        {
            Debug.LogWarning("Object to toggle is not assigned.");
        }
    }
    
    public void SetActive(bool isActive)
    {
        if (obj != null)
        {
            obj.SetActive(isActive);
        }
        else
        {
            Debug.LogWarning("Object to set active is not assigned.");
        }
    }
    
    
}
