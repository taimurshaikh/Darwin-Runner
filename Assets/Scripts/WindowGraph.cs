using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
public class WindowGraph : MonoBehaviour
{
    [SerializeField]
    private Sprite circleSprite;
    private RectTransform graphContainer;
    private List<GameObject> points;
    List<float> valueList =  new List<float>() {};
    void Awake()
    {
        graphContainer = transform.Find("GraphContainer").GetComponent<RectTransform>();    
        points = new List<GameObject>();
        ShowGraph();
    }

    private GameObject CreateCircle(Vector2 anchoredPosition)
    {
        GameObject circleObject = new GameObject("circle", typeof(Image));
        circleObject.transform.SetParent(graphContainer, false);
        circleObject.GetComponent<Image>().sprite = circleSprite;
        RectTransform rectTransform = circleObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = new Vector2(11, 11);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        return circleObject;
    }

    public void ShowGraph()
    {
        if (valueList.Count <= 0)
        {
            return;
        }
        float buffer = 0.2f;
        
        float graphHeight = graphContainer.sizeDelta.y;
        float graphWidth = graphContainer.sizeDelta.x;

        float yMax = valueList.Max();
        float yMin = valueList.Min();
        float yDiff = Mathf.Max(yMax - yMin, 5f);
        yMax += (yDiff * buffer);
        yMin -= (yDiff * buffer);
        float xSize = graphWidth / (valueList.Count + 1);

        GameObject prevCircleGameObject = null;
        GameObject circleGameObject = null;
        for (int i = 0; i < valueList.Count; i++)
        {
            float xPos = xSize + i * xSize;
            float yPos = ((valueList[i] - yMin) / (yMax - yMin)) * graphHeight;
            circleGameObject = points[points.Count - 1];
            points.Add(CreateCircle(new Vector2(xPos, yPos)));    
            if (prevCircleGameObject != null)
            {
                CreateDotConnection(prevCircleGameObject.GetComponent<RectTransform>().anchoredPosition, circleGameObject.GetComponent<RectTransform>().anchoredPosition);
            }
            prevCircleGameObject = circleGameObject;
        }
    }

    private void CreateDotConnection(Vector2 dotPosA, Vector2 dotPosB)
    {
        GameObject lineObject = new GameObject("dotConnection", typeof(Image));
        lineObject.transform.SetParent(graphContainer, false);
        lineObject.GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
        RectTransform rectTransform = lineObject.GetComponent<RectTransform>();
        Vector2 dir = (dotPosB - dotPosA).normalized;
        float distance = Vector2.Distance(dotPosA, dotPosB);
        rectTransform.anchorMin = new Vector2(0, 0); 
        rectTransform.anchorMax = new Vector2(0, 0);   
        rectTransform.sizeDelta = new Vector2(distance, 10f);
        rectTransform.anchoredPosition = dotPosA + dir * distance * 0.5f;
        rectTransform.localEulerAngles = new Vector3(0,0, (dir.y < 0 ? -Mathf.Acos(dir.x) : Mathf.Acos(dir.x)) * Mathf.Rad2Deg);
    }

    public void ClearGraph()
    {
        foreach (GameObject point in points)
        {
            Destroy(point);
        }
        points.Clear();
    }

    public void AddNewPoint(float val)
    {
        valueList.Add(val);
    }
}
