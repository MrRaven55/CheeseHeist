using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OakMove : MonoBehaviour, IInteractable
{
    public float moveDuration = 1f;

    RectTransform rectTransform;

   private RectTransform endPoint;
    PlayerMovement PlayerID;
    public int startPosition;


    private void Awake()
    {

        rectTransform = GetComponent<RectTransform>();
        GameObject oakUIElement = GameObject.FindGameObjectWithTag("Oak");
        endPoint = oakUIElement.GetComponent<RectTransform>();
       
    }
    // Start is called before the first frame update
    void Start()
    {
        //start position for the UI

        if (startPosition == 1)
            rectTransform.anchoredPosition = Vector2.zero;
         if (startPosition == 2) 
            rectTransform.anchoredPosition = Vector2.one;
        
        //end position for the UI
        Vector2 targetPosition = endPoint.anchoredPosition;
       
        StartCoroutine(MoveToCorner(targetPosition));
        
    }

   public void Interact (int PlayerID)
    {
        if(PlayerID == 1)
        {
            startPosition = 1;
        }
        else if(PlayerID == 2)
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
