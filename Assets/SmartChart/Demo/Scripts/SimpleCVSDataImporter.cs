using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using ToucanSystems;

/// <summary>
/// Test script for filling chart with values from 3 CSV files.
/// </summary>
public class SimpleCVSDataImporter : MonoBehaviour
{

    [SerializeField]
    private string csv1FileName;
    [SerializeField]
    private string csv2FileName;
    [SerializeField]
    private string csv3FileName;
    [SerializeField]
    private SmartChart chart;
    [SerializeField]
    private SimpleCaptions captions;

    private string csvFilePath;
    private string file;

    private void Start()
    {
        List<SmartChartData> chartData = new List<SmartChartData>();
        chartData.Add(PrepareChartData(LoadDataFromCsvFile(csv1FileName), new Color32(255, 0, 153, 255), 3));
        chartData.Add(PrepareChartData(LoadDataFromCsvFile(csv2FileName), new Color32(0, 181, 255, 255), 3));
        chartData.Add(PrepareChartData(LoadDataFromCsvFile(csv3FileName), new Color32(255, 195, 0, 255), 3));
        DisplayDataOnChart(chartData.ToArray());
        captions.UpdateCaptions();
    }

    private List<Vector2> LoadDataFromCsvFile(string csvFileName)
    {
        try
        {
            csvFilePath = Application.streamingAssetsPath + "/" + csvFileName;

            if (File.Exists(csvFilePath))
            {
                List<Vector2> dataList = new List<Vector2>();

                file = File.ReadAllText(csvFilePath);
                var lines = file.Split(new string[] { "\n" }, System.StringSplitOptions.None);

                for (int i = 0; i < lines.Length; i++)
                {
                    var line = lines[i];

                    if (!string.IsNullOrEmpty(line))
                    {
                        var values = line.Split(',');
                        float x, y;
                        float.TryParse(values[0].Trim(), out x);
                        float.TryParse(values[1].Trim(), out y);
                        dataList.Add(new Vector2(x, y));
                    }
                }
                return dataList;
            }
            return null;
        }
        catch (Exception ex)
        {
            Debug.Log("Unable to parse CSV file.\n" + ex.Message);
            return null;
        }
    }

    private SmartChartData PrepareChartData(List<Vector2> dataList, Color32 color, float lineWidth)
    {
        if(dataList != null && dataList.Count > 0)
        {
            SmartChartData chartData = new SmartChartData();
            chartData.data = dataList.ToArray();
            chartData.dataLineColor = color;
            chartData.dataLineWidth = lineWidth;
            return chartData;
        }
        return null;
    }

    private void DisplayDataOnChart(SmartChartData[] chartData)
    {
        chart.chartData = chartData;
        chart.UpdateChart();
    }
}
