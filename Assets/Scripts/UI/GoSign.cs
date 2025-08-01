using UnityEngine;
using UnityEngine.UI;

public class GoSign : MonoBehaviour
{
    [SerializeField] float dissapearTimer;
    [SerializeField] Image imageRenderer;

    float timerCounter;
    bool canDissapearFlag = true;

    private void OnEnable()
    {
        timerCounter = dissapearTimer;
    }

    // Update is called once per frame
    void Update()
    {
        timerCounter -= Time.deltaTime;
        //Debug.Log(timerCounter);
        if(timerCounter <= 0 && canDissapearFlag && imageRenderer.color.a == 0)
        {
            gameObject.SetActive(false);
        }
    }
}
