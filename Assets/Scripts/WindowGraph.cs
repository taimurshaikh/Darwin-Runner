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

    // Lists of the objects corresponding to the points and lines seen in the graph window
    private List<GameObject> points;
    private List<GameObject> lines;

    // List of average fitnesses that the graph will iterate over and display the values
    private List<float> valueList;

    // Maximum number of points that can be displayed on the graph at any time
    private int maxValues = 50;

    private void Awake() 
    {
        graphContainer = transform.Find("GraphContainer").GetComponent<RectTransform>();    
        points = new List<GameObject>();
        lines = new List<GameObject>();
        valueList = new List<float>();
    }

    private GameObject createCircle(Vector2 anchoredPosition)
    {
        // Instantiating a new circle object and setting its position parameters according to the anchored position argument 
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
        // In the unexpected case that there are no values to show, break out of the function
        if (valueList.Count <= 0) {
            return;
        }

        // Make graph 'responsive' to extreme y values
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

        for (int i = 0; i < valueList.Count; i++) {
            float xPos = xSize + i * xSize;
            float yPos = ((valueList[i] - yMin) / (yMax - yMin)) * graphHeight;
            
            if (points.Count > 0) {
                circleGameObject = points[points.Count - 1];
            }

            points.Add(createCircle(new Vector2(xPos, yPos))); 

            // Iteratively adding connections between neighbouring points
            if (prevCircleGameObject != null) {
                createDotConnection(prevCircleGameObject.GetComponent<RectTransform>().anchoredPosition, circleGameObject.GetComponent<RectTransform>().anchoredPosition);
            }
            prevCircleGameObject = circleGameObject;
        }

        // Need to account for the last point pair separately
        createDotConnection(points[points.Count - 1].GetComponent<RectTransform>().anchoredPosition, points[Mathf.Max(0, points.Count - 2)].GetComponent<RectTransform>().anchoredPosition);
    }

    private void createDotConnection(Vector2 dotPosA, Vector2 dotPosB)
    {
        // Create a line game object to connect two points on the graph
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
        lines.Add(lineObject);
    }

    public void ClearGraph(bool resetEvolution=false)
    {
        // Clear the graph window for the next generation
        foreach (GameObject point in points) {
            Destroy(point);
        }
        foreach (GameObject line in lines) {
            Destroy(line);
        }

        points.Clear();
        lines.Clear();

        // If this method is instead called when the user resets the evolution, the whole valueList is cleared, giving us a fresh graph
        if (resetEvolution == true) {
            valueList.Clear();
        }
    }

    public void AddNewPoint(float val)
    {
        // Simply recording the new point information in valueList to be displayed in the next ShowGraph() cal
        valueList.Add(val);

        // Only display the most recent points 
        if (valueList.Count > maxValues) {
            valueList.RemoveRange(0, Mathf.Min(valueList.Count - maxValues, valueList.Count));
        }
    }
}
