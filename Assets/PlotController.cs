using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlotController : MonoBehaviour
{
    [Header("Dependencies")]
    public FishingRodController fishingRodController;
    public RectTransform plotArea;

    [Header("Settings")]
    public float proximityThreshold1 = 3.0f;
    public float proximityThreshold2 = 6.0f;

    // Waveform control variables
    public float bpm = 120f;
    public int beatsPerCycle = 8;

    [Header("Plot Appearance")]
    public Color axisColor = Color.white;
    public Color waveColor = Color.cyan;
    public Color verticalLineColor = Color.red;
    public Color dotColor = Color.yellow;
    public float lineWidth = 10f;
    public float axisLineWidth = 5f;

    public DotStatus dotStatus;

    // Private variables
    private float time = 0f;
    private float yMax;
    private float yStart;
    private float xStart;

    // Plot dimensions
    private float plotWidth;
    private float plotHeight;

    // Component references (created programmatically)
    private GameObject waveformRenderer;
    private GameObject verticalLine;
    private RectTransform dot;

    // Axis renderers
    private GameObject xAxisRenderer;
    private GameObject yAxisRenderer;

    // Wave generation
    private int waveResolution = 200;

    private List<RectTransform> beatDots = new List<RectTransform>();

    private void Start()
    {
        if (fishingRodController == null)
        {
            Debug.LogError("FishingRodController is not assigned.");
            return;
        }

        InitializePlotParameters();
        CreatePlotComponents();
        SetupVerticalLine();
        SetupDot();
        UpdateVerticalLineColor();
    }

    private void Update()
    {
        time += Time.deltaTime;

        UpdateWaveform();
        UpdateDotPosition(fishingRodController.rodPosition);
        UpdateVerticalLineColor();
    }

    private void InitializePlotParameters()
    {
        float panelHeight = plotArea.rect.height;
        plotWidth = plotArea.rect.width;
        plotHeight = plotArea.rect.height;

        Debug.Log(fishingRodController.maxAngle);
        yMax = fishingRodController.maxAngle;
        yStart = fishingRodController.rodPosition;
        xStart = 0.5f;
    }

    private void CreatePlotComponents()
    {
        // Create waveform using UI Image components
        waveformRenderer = CreateUILine("Waveform", Color.clear);

        // Create vertical line using UI Image
        verticalLine = CreateUILine("VerticalLine", verticalLineColor);

        // Create axis renderers as children of the plot area
        GameObject axisParent = new GameObject("Axes");
        axisParent.transform.SetParent(plotArea);
        axisParent.transform.localPosition = Vector3.zero;
        axisParent.transform.localScale = Vector3.one;

        // X-Axis
        xAxisRenderer = CreateUILine("X-Axis", axisColor, axisParent.transform);

        // Y-Axis
        yAxisRenderer = CreateUILine("Y-Axis", axisColor, axisParent.transform);

        // Create dot
        GameObject dotObj = new GameObject("Dot");
        dotObj.transform.SetParent(plotArea);
        dotObj.transform.localPosition = Vector3.zero;
        dotObj.transform.localScale = Vector3.one;
        dot = dotObj.AddComponent<RectTransform>();

        // Add Image component to dot
        Image dotImage = dotObj.AddComponent<Image>();
        dotImage.sprite = CreateCircleSprite();
        dotImage.color = dotColor;

        // Set dot size
        dot.sizeDelta = new Vector2(10, 10);

        UpdateAxes();
    }

    private GameObject CreateUILine(string name, Color color, Transform parent = null)
    {
        GameObject lineObj = new GameObject(name);
        if (parent == null) parent = plotArea;
        lineObj.transform.SetParent(parent);
        lineObj.transform.localPosition = Vector3.zero;
        lineObj.transform.localScale = Vector3.one;

        RectTransform rectTransform = lineObj.AddComponent<RectTransform>();
        Image image = lineObj.AddComponent<Image>();
        image.color = color;

        return lineObj;
    }

    private void UpdateAxes()
    {
        // Convert plot coordinates to local UI coordinates
        float xAxisY = PlotToLocalY(0);
        float yAxisX = PlotToLocalX(0);

        // X-Axis (horizontal line)
        SetUILinePosition(xAxisRenderer,
            new Vector2(-plotWidth / 2, xAxisY),
            new Vector2(plotWidth / 2 - 20, xAxisY),
            axisLineWidth);

        // Y-Axis (vertical line)
        SetUILinePosition(yAxisRenderer,
            new Vector2(yAxisX, -plotHeight / 2),
            new Vector2(yAxisX, plotHeight / 2 - 20),
            axisLineWidth);
    }

    private void SetupVerticalLine()
    {
        if (verticalLine == null) return;

        float xPos = PlotToLocalX(xStart);
        SetUILinePosition(verticalLine,
            new Vector2(xPos, -plotHeight / 2),
            new Vector2(xPos, plotHeight / 2),
            lineWidth);
    }

    private void SetupDot()
    {
        if (dot == null) return;

        UpdateDotPosition(yStart);
    }

    private void UpdateWaveform()
    {
        if (waveformRenderer == null) return;

        // Clear existing waveform segments
        Transform waveformTransform = waveformRenderer.transform;
        for (int i = waveformTransform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(waveformTransform.GetChild(i).gameObject);
        }

        // Create waveform using multiple UI line segments
        List<Vector2> points = new List<Vector2>();

        for (int i = 0; i < waveResolution; i++)
        {
            float xPlot = (float)i / (waveResolution - 1) * 4f;
            float yPlot = GetSineWaveValue(xPlot);

            Vector2 localPos = new Vector2(PlotToLocalX(xPlot), PlotToLocalY(yPlot));
            points.Add(localPos);
        }

        // Create line segments between consecutive points
        for (int i = 0; i < points.Count - 1; i++)
        {
            GameObject segment = new GameObject($"WaveSegment_{i}");
            segment.transform.SetParent(waveformTransform);
            segment.transform.localPosition = Vector3.zero;
            segment.transform.localScale = Vector3.one;

            RectTransform segmentRect = segment.AddComponent<RectTransform>();
            Image segmentImage = segment.AddComponent<Image>();
            segmentImage.color = waveColor;

            SetUILinePosition(segment, points[i], points[i + 1], lineWidth);
        }
    }

    private float GetSineWaveValue(float x)
    {
        // Calculate the scroll speed based on bpm and beatsPerCycle
        float cyclesPerSecond = bpm / 60f / beatsPerCycle; // Convert bpm to cycles per second, adjusted by beatsPerCycle

        // Shift x leftward based on time and cyclesPerSecond
        float shiftedX = x + xStart + time * cyclesPerSecond;

        // Calculate sine wave value, centered at yMax / 2 and ranging between 0 and yMax
        return (Mathf.Sin(shiftedX * Mathf.PI * 2f) + 1f) * (yMax / 2f);
    }

    private void UpdateDotPosition(float yValue)
    {
        if (dot == null) return;

        float xPos = PlotToLocalX(xStart);
        float yPos = PlotToLocalY(yValue);

        dot.anchoredPosition = new Vector2(xPos, yPos);
    }

    private float PlotToLocalX(float plotX)
    {
        // Convert plot X coordinate to local UI coordinate
        // X range: 0 to 4, centered in plot area
        return (plotX - 2f) * (plotWidth / 4f);
    }

    private float PlotToLocalY(float plotY)
    {
        // Convert plot Y coordinate to local UI coordinate
        // Y range: yMin to yMax, centered in plot area
        float normalizedY = (plotY - yMax / 2f) / (yMax / 2f);
        return normalizedY * (plotHeight / 3f); // Use 2/3 of plot height for better visibility
    }

    // Helper method to create a simple circle sprite
    private Sprite CreateCircleSprite()
    {
        // Create a simple 32x32 white circle texture
        int size = 32;
        Texture2D texture = new Texture2D(size, size);
        Color[] colors = new Color[size * size];

        Vector2 center = new Vector2(size / 2f, size / 2f);
        float radius = size / 2f - 1;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                Vector2 pos = new Vector2(x, y);
                float distance = Vector2.Distance(pos, center);

                if (distance <= radius)
                {
                    colors[y * size + x] = Color.white;
                }
                else
                {
                    colors[y * size + x] = Color.clear;
                }
            }
        }

        texture.SetPixels(colors);
        texture.Apply();

        return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
    }

    // Helper method to position a UI line between two points
    private void SetUILinePosition(GameObject lineObj, Vector2 startPos, Vector2 endPos, float width)
    {
        RectTransform rectTransform = lineObj.GetComponent<RectTransform>();
        if (rectTransform == null) return;

        Vector2 direction = endPos - startPos;
        float distance = direction.magnitude;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Set the size and position
        rectTransform.sizeDelta = new Vector2(distance, width);
        rectTransform.anchoredPosition = (startPos + endPos) / 2f;
        rectTransform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        // Set anchor and pivot
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
    }

    private void UpdateVerticalLineColor()
    {
        if (verticalLine == null || dot == null) return;

        // Calculate the distance between the dot and the vertical line
        float dotPosition = fishingRodController.rodPosition;
        float waveValue = GetSineWaveValue(xStart);
        float distance = Mathf.Abs(dotPosition - waveValue);

        if (distance < proximityThreshold1)
        {
            verticalLine.GetComponent<Image>().color = Color.green;
            dotStatus = DotStatus.OnTheLine;
        }
        else if (distance < proximityThreshold2)
        {
            verticalLine.GetComponent<Image>().color = Color.yellow;
            dotStatus = DotStatus.CloseEnough;
        }
        else
        {
            verticalLine.GetComponent<Image>().color = Color.red;
            dotStatus = DotStatus.NotOnTheLine;
        }
    }
}