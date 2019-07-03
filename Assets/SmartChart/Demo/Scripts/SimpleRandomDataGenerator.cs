using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ToucanSystems;

/// <summary>
/// Test script for filling chart with random data.
/// </summary>
public class SimpleRandomDataGenerator : MonoBehaviour
{

    [SerializeField]
    private SmartChart chart;

    private List<List<Color32>> colorsPalette;
    private int paletteIndex = 0;

    private void Start()
    {
        colorsPalette = new List<List<Color32>>();
        for (int i = 0; i < 4; i++)
        {
            colorsPalette.Add(new List<Color32>());
        }
        colorsPalette[0].Add(new Color32(181, 106, 206, 77));
        colorsPalette[0].Add(new Color32(37, 138, 229, 77));
        colorsPalette[0].Add(new Color32(43, 214, 185, 77));

        colorsPalette[1].Add(new Color32(255, 123, 172, 77));
        colorsPalette[1].Add(new Color32(114, 97, 198, 77));
        colorsPalette[1].Add(new Color32(0, 255, 255, 77));

        colorsPalette[2].Add(new Color32(0, 146, 69, 77));
        colorsPalette[2].Add(new Color32(0, 255, 255, 77));
        colorsPalette[2].Add(new Color32(51, 183, 221, 77));

        colorsPalette[3].Add(new Color32(237, 28, 36, 77));
        colorsPalette[3].Add(new Color32(255, 248, 179, 77));
        colorsPalette[3].Add(new Color32(229, 111, 32, 77));
    }

    private void OnEnable()
    {
        StartCoroutine(FillChartCoroutine());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator FillChartCoroutine()
    {
        yield return new WaitForSeconds(1);
        while (true)
        {
            FillChart();
            yield return new WaitForSeconds(2);
        }
    }

    public void FillChart()
    {
        List<SmartChartData> exData = new List<SmartChartData>();
        for (int i = 0; i < 3; i++)
        {
            SmartChartData chartData = new SmartChartData();
            exData.Add(chartData);
            chartData.dataLineColor = new Color(0, 0, 0, 0);
            chartData.dataFillColor = colorsPalette[paletteIndex][i];
            float x = chart.minXValue;
            float modifier = (chart.maxXValue - chart.minYValue) / 10;
            List<Vector2> tuturu = new List<Vector2>();
            while (x <= chart.maxXValue)
            {
                tuturu.Add(new Vector2(x, Random.Range(chart.minYValue, chart.maxYValue)));
                x += modifier;
                chartData.data = tuturu.ToArray();
                exData[i] = chartData;
            }
        }
        chart.chartData = exData.ToArray();
        chart.UpdateChart();
        paletteIndex++;
        if (paletteIndex >= 4)
        {
            paletteIndex = 0;
        }
    }
}
