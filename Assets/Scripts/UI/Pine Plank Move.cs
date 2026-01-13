using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinePlankMove : MonoBehaviour
{
    public float moveDuration = 1f;

    RectTransform rectTransform;

    private RectTransform endPoint;



    private void Awake()
    {

        rectTransform = GetComponent<RectTransform>();

        GameObject PinePlankUIElement = GameObject.FindGameObjectWithTag("PinePlank");
        endPoint = PinePlankUIElement.GetComponent<RectTransform>();

    }
    // Start is called before the first frame update
    void Start()
    {


        //start position for the UI
        rectTransform.anchoredPosition = Vector2.zero;
        //end position for the UI

        Vector2 targetPosition = endPoint.anchoredPosition;

        StartCoroutine(MoveToCorner(targetPosition));

    }

    // Update is called once per frame
    void Update()
    {

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
