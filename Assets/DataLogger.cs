using System;
using System.IO;
using System.Text;
using UnityEngine;

public class DataLogger : MonoBehaviour
{
    public FishingRodController fishingRodController;
    public FishController fishController;
    public PlotController plotController;

    private string filePath;
    private string fileName;
    private StringBuilder csvContent;
    private float time = 0f;
    private float logTimer = 0f;
    private const float logInterval = 0.1f;

    void Start()
    {
        string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "FishingGame");

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        fileName = $"FishingGameLog_{timestamp}.csv";
        filePath = Path.Combine(folderPath, fileName);

        csvContent = new StringBuilder();
        csvContent.AppendLine("time,targetY,playerY,dotStatus,score,numFishCaught,beatOffset,weight");
    }

    void Update()
    {
        if (!plotController.isPaused && !plotController.isFinished)
        {
            time += Time.deltaTime;
            logTimer += Time.deltaTime;

            if (logTimer >= logInterval)
            {
                LogData();
                logTimer = 0f;
            }
        }

        if (plotController.isFinished)
        {
            time = 0f;
            Save();
            Debug.Log($"Data saved to {filePath}");
        }
    }

    private void LogData()
    {
        string row = string.Format("{0},{1},{2},{3},{4},{5},{6},{7}", time, plotController.targetY, fishingRodController.rodPosition, plotController.dotStatus.ToString(), fishController.score, fishController.fishCaughtCount, plotController.beatOffset, int.Parse(PlayerPrefs.GetString("PlayerWeight", "-1")));
        csvContent.AppendLine(row);
    }

    private void Save()
    {
        File.WriteAllText(filePath, csvContent.ToString());
    }
}