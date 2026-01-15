using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogMove : MonoBehaviour
{
    public float moveDuration = 1f;

    RectTransform rectTransform;

    private RectTransform endPoint;
   
    public int startPosition;
    public string uiTagName;

    private void Awake()
    {

        rectTransform = GetComponent<RectTransform>();
        GameObject logUIElement = GameObject.FindGameObjectWithTag(uiTagName);
        endPoint = logUIElement.GetComponent<RectTransform>();

    }
    // Start is called before the first frame update
    public void StartMove(Vector2 start)
    {
        //start position for the UI

        rectTransform.anchoredPosition = start;

        //end position for the UI
        Vector2 targetPosition = endPoint.anchoredPosition;

        StartCoroutine(MoveToCorner(targetPosition));

    }

    public void Interact(int PlayerID)
    {
        if (PlayerID == 1)
        {
            startPosition = 1;
        }
        else if (PlayerID == 2)
        {
            startPosition = 2;
        }
    }

    IEnumerator MoveToCorner(Vector2 target)
    {
        Vector2 start = rectTransform.anchoredPosition;
        float elapsed = 0f;
        rectTransform.anchoredPosition = target;
        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / moveDuration;

            rectTransform.anchoredPosition = Vector2.Lerp(start, target, t);
            yield return null;
        }



        if (moveDuration < elapsed)
            Destroy(gameObject);

    }
}
