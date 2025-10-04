using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlotController : MonoBehaviour
{
    public FishingRodController fishingRodController;
    public RectTransform plotArea;
    public float proximityThreshold1 = 3.0f;
    public float proximityThreshold2 = 6.0f;
    public float bpm = 120f;
    public int beatsPerCycle = 8;
    public int reps = 8;

    public Color axisColor = Color.white;
    public Color waveColor = Color.cyan;
    public Color verticalLineColor = Color.red;
    public Color dotColor = Color.yellow;
    public float lineWidth = 10f;
    public float axisLineWidth = 5f;
    public DotStatus dotStatus;

    public bool isPaused = false;
    public bool isFinished = false;

    private float time = 0f;
    private float xMax = 4f;
    private float yMax;
    private float yStart;
    private float xStart;
    private int beatOffset = 0; // Offset relative to wave resolution
    private float plotWidth;
    private float plotHeight;
    private GameObject waveformRenderer;
    private GameObject verticalLine;
    private RectTransform dot;
    private GameObject xAxisRenderer;
    private GameObject yAxisRenderer;
    private int waveResolution = 500;
    private int firstRedPosition = -1;
    private int secondRedPosition = -1;

    private int cyclesCompleted = 0; // Counter for completed cycles
    private float cycleDuration; // Duration of one cycle

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
        SetUpBeatMarkers();

        cycleDuration = beatsPerCycle / (bpm / 60f); // Calculate cycle duration
    }

    private void Update()
    {
        if (isPaused || isFinished)
        {
            // While paused or finished, still update dot and vertical line color
            UpdateDotPosition(fishingRodController.rodPosition);
            UpdateVerticalLineColor();
            return; // Skip waveform scrolling
        }

        time += Time.deltaTime;

        // Check if a cycle is completed
        if (time >= cycleDuration)
        {
            cyclesCompleted++;
            time -= cycleDuration;

            if (cyclesCompleted >= reps)
            {
                isFinished = true;
                isPaused = true;
                return;
            }
        }

        UpdateWaveform();
        UpdateDotPosition(fishingRodController.rodPosition);
        UpdateVerticalLineColor();
    }

    private void InitializePlotParameters()
    {
        float panelHeight = plotArea.rect.height;
        plotWidth = plotArea.rect.width;
        plotHeight = plotArea.rect.height;
        yMax = fishingRodController.maxAngle;
        yStart = fishingRodController.rodPosition;
        xStart = 0.5f;
    }

    private void CreatePlotComponents()
    {
        waveformRenderer = CreateUILine("Waveform", Color.clear);
        verticalLine = CreateUILine("VerticalLine", verticalLineColor);
        GameObject axisParent = new GameObject("Axes");
        axisParent.transform.SetParent(plotArea);
        axisParent.transform.localPosition = Vector3.zero;
        axisParent.transform.localScale = Vector3.one;
        xAxisRenderer = CreateUILine("X-Axis", axisColor, axisParent.transform);
        yAxisRenderer = CreateUILine("Y-Axis", axisColor, axisParent.transform);
        GameObject dotObj = new GameObject("Dot");
        dotObj.transform.SetParent(plotArea);
        dotObj.transform.localPosition = Vector3.zero;
        dotObj.transform.localScale = Vector3.one;
        dot = dotObj.AddComponent<RectTransform>();
        Image dotImage = dotObj.AddComponent<Image>();
        dotImage.sprite = CreateCircleSprite();
        dotImage.color = dotColor;
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
        float xAxisY = PlotToLocalY(0);
        float yAxisX = PlotToLocalX(0);
        SetUILinePosition(xAxisRenderer,
            new Vector2(-plotWidth / 2, xAxisY),
            new Vector2(plotWidth / 2, xAxisY),
            axisLineWidth);
        SetUILinePosition(yAxisRenderer,
            new Vector2(yAxisX, -plotHeight / 2 + 10),
            new Vector2(yAxisX, plotHeight / 2),
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

    private void SetUpBeatMarkers()
    {
        for (int i = 0; i <= waveResolution; i++)
        {
            DrawBeatMarker(i);
        }
    }

    private void UpdateWaveform()
    {
        if (waveformRenderer == null) return;

        Transform waveformTransform = waveformRenderer.transform;
        for (int i = waveformTransform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(waveformTransform.GetChild(i).gameObject);
        }

        List<Vector2> points = new List<Vector2>();

        firstRedPosition = -1;
        secondRedPosition = -1;
        ClearAllBeatMarkers();
        for (int i = 0; i < waveResolution; i++)
        {
            float xPlot = (float)i / (waveResolution - 1) * xMax;
            float yPlot = GetSineWaveValue(xPlot);
            findFirstTwoReds(i, yPlot);
            Vector2 localPos = new Vector2(PlotToLocalX(xPlot), PlotToLocalY(yPlot));
            points.Add(localPos);
        }
        if (firstRedPosition != -1 && secondRedPosition != -1)
        {
            int interval = (int)Mathf.Round((secondRedPosition - firstRedPosition) / beatsPerCycle);
            // Starting from firstRedPosition, go backwards in wave resolution and set red
            for (int i = firstRedPosition; i >= 0; i -= interval)
            {
                SetBeatMarkerColour(i, Color.red);
            }
            // Starting from firstRedPosition, go forwards in wave resolution and set red
            for (int i = firstRedPosition + interval; i < waveResolution; i += interval)
            {
                SetBeatMarkerColour(i, Color.red);
            }
        }

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
        float cyclesPerSecond = bpm / 60f / beatsPerCycle;
        float shiftedX = x + xStart / 2 + time * cyclesPerSecond;
        float yValue = (Mathf.Sin(shiftedX * Mathf.PI * 2f) + 1f) * (yMax / 2f);
        return yValue;
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
        return (plotX - 2f) * (plotWidth / xMax);
    }

    private float PlotToLocalY(float plotY)
    {
        float normalizedY = (plotY - yMax / 2f) / (yMax / 2f);
        return normalizedY * (plotHeight / 3f); // Use 2/3 of plot height for better visibility
    }

    private Sprite CreateCircleSprite()
    {
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

    private void SetUILinePosition(GameObject lineObj, Vector2 startPos, Vector2 endPos, float width)
    {
        RectTransform rectTransform = lineObj.GetComponent<RectTransform>();
        if (rectTransform == null) return;

        Vector2 direction = endPos - startPos;
        float distance = direction.magnitude;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        rectTransform.sizeDelta = new Vector2(distance, width);
        rectTransform.anchoredPosition = (startPos + endPos) / 2f;
        rectTransform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
    }

    private void UpdateVerticalLineColor()
    {
        if (verticalLine == null || dot == null) return;

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

    private void DrawBeatMarker(int i)
    {
        if (i <= 0 || i > waveResolution) return;
        Transform plotTransform = plotArea.transform;
        float xLocal = PlotToLocalX(xMax / waveResolution * i);

        // Create a vertical line for the beat marker
        GameObject beatMarker = CreateUILine($"BeatMarker_{i}", Color.clear);
        SetUILinePosition(beatMarker,
            new Vector2(xLocal, -plotHeight / 2 + 10),
            new Vector2(xLocal, -plotHeight / 2 + 20),
            axisLineWidth);
    }

    private void findFirstTwoReds(int i, float y)
    {
        if (y <= 0.01f)
        {
            if (firstRedPosition == -1)
            {
                firstRedPosition = i + beatOffset;
            }
            else if (secondRedPosition == -1 && i >= firstRedPosition + 5)
            {
                secondRedPosition = i + beatOffset;
            }
        }
    }

    private void SetBeatMarkerColour(int i, Color colour)
    {
        GameObject beatMarker = GameObject.Find($"BeatMarker_{i}");
        if (beatMarker == null) return;
        beatMarker.GetComponent<Image>().color = colour;
    }

    private void ClearAllBeatMarkers()
    {
        for (int i = 0; i <= waveResolution; i++)
        {
            SetBeatMarkerColour(i, Color.clear);
        }
    }
}