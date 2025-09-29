using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ComicPanelAnimator : MonoBehaviour
{
    [SerializeField] private GameObject[] imageObjects;
    [SerializeField] private GameObject menuObject;
    [SerializeField] private RectTransform imageTarget;
    [SerializeField] private RectTransform menuTarget;
    [SerializeField] private float slideDistance = 50f;
    [SerializeField] private float animDuration = 0.5f;
    [SerializeField] private float delayBetween = 0.2f;

    void Start()
    {
        foreach (GameObject obj in imageObjects)
        {
            obj.SetActive(false);
        }
        menuObject.SetActive(false);

        StartCoroutine(AnimateSequence());
    }

    IEnumerator AnimateSequence()
    {
        for (int i = 0; i < imageObjects.Length; i++)
        {
            GameObject obj = imageObjects[i];
            RectTransform objRT = obj.GetComponent<RectTransform>();
            RectTransform targetRT = imageTarget;

            obj.SetActive(true);

            Vector2 startPos = targetRT.anchoredPosition + new Vector2(-slideDistance, slideDistance);
            objRT.anchoredPosition = startPos;
            SetAlpha(obj, true, 0f);
            objRT.localScale = Vector3.one;

            yield return StartCoroutine(AnimateObject(objRT, targetRT.anchoredPosition, true));

            yield return new WaitForSeconds(delayBetween);
        }

        RectTransform menuRT = menuObject.GetComponent<RectTransform>();

        menuObject.SetActive(true);

        menuRT.anchoredPosition = menuTarget.anchoredPosition;
        menuRT.localScale = Vector3.zero;
        SetAlpha(menuObject, false, 1f);

        yield return StartCoroutine(AnimateObject(menuRT, menuTarget.anchoredPosition, false));
    }

    IEnumerator AnimateObject(RectTransform rt, Vector2 targetPos, bool isImage)
    {
        float t = 0f;

        if (isImage)
        {
            Vector2 startPos = rt.anchoredPosition;
            while (t < 1f)
            {
                t += Time.deltaTime / animDuration;
                rt.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
                SetAlpha(rt.gameObject, true, t);
                yield return null;
            }
            rt.anchoredPosition = targetPos;
            SetAlpha(rt.gameObject, true, 1f);
        }
        else
        {
            Vector3 startScale = rt.localScale;
            Vector3 endScale = Vector3.one;
            while (t < 1f)
            {
                t += Time.deltaTime / animDuration;
                rt.localScale = Vector3.Lerp(startScale, endScale, t);
                yield return null;
            }
            rt.localScale = endScale;
        }
    }

    void SetAlpha(GameObject obj, bool isImage, float alpha)
    {
        if (isImage)
        {
            Image img = obj.GetComponent<Image>();
            if (img)
            {
                Color c = img.color;
                c.a = alpha;
                img.color = c;
            }
        }
        else
        {
            CanvasGroup cg = obj.GetComponent<CanvasGroup>();
            if (cg) cg.alpha = alpha;
        }
    }
}