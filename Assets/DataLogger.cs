using System;
using System.IO;
using System.Text;

public class DataLogger
{
    private string filePath;
    private StringBuilder csvContent;

    public DataLogger(string fileName)
    {
        string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "FishingGame");

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        filePath = Path.Combine(folderPath, fileName);
        csvContent = new StringBuilder();

        // Write the header row
        csvContent.AppendLine("time,targetY,playerY,dotStatus,score,numFishCaught,beatOffset,weight");
    }

    public void LogData(float time, float targetY, float playerY, string dotStatus, float score, int numFishCaught, int beatOffset, int weight)
    {
        string row = string.Format("{0},{1},{2},{3},{4},{5},{6},{7}", time, targetY, playerY, dotStatus, score, numFishCaught, beatOffset, weight);
        csvContent.AppendLine(row);
    }

    public void Save()
    {
        File.WriteAllText(filePath, csvContent.ToString());
    }
}