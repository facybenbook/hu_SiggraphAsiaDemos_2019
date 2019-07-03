/*
 * Part of: SmartChart
 * Pushes values onto the chart.
 */
using System.Collections.Generic;
using UnityEngine;

namespace ToucanSystems
{
    /// <summary>
    /// Pushes values onto the chart.
    /// </summary>
    public class ChartDataPusher : MonoBehaviour
    {

        [SerializeField]
        private SmartChart chart;

        [SerializeField]
        private float valueModifier = 1;

        [SerializeField]
        private int dataIndex = 0;

        [SerializeField]
        private float xModifier = 1;

        [SerializeField]
        private bool adjustYRange = false;

        [SerializeField]
        private float minYValue = 80;

        [SerializeField]
        private int maxYValueStep = 10;

        private List<Vector2> chartDataList;

        private void OnEnable()
        {
            chartDataList = new List<Vector2>();
        }

        /// <summary>
        /// Updates charts data to contain new value at the beggining of dataset.
        /// </summary>
        /// <param name="value">Value to be pushed onto the chart.</param>
        public void PushValue(float value)
        {
            if (chart.chartData.Length != 0)
            {
                value *= valueModifier;

                chartDataList.Insert(0, new Vector2(0, value));

                for (int i = 1; i < chartDataList.Count; i++)
                {
                    chartDataList[i] = new Vector2(i * xModifier, chartDataList[i].y);
                    if (chartDataList[i].x > chart.maxXValue)
                    {
                        chartDataList.RemoveAt(i);
                    }
                }

                chart.chartData[dataIndex].data = chartDataList.ToArray();

                if (adjustYRange)
                {
                    float maxValue = GetMaxValue();
                    float lastMaxValue = chart.maxYValue;

                    if (maxValue > minYValue)
                    {
                        chart.maxYValue = maxValue;
                    }
                    else
                    {
                        chart.maxYValue = minYValue;
                    }

                    if (lastMaxValue != chart.maxYValue)
                    {
                        chart.SetupValues(false);
                    }
                }

                chart.UpdateChart();
            }
        }

        private float GetMaxValue()
        {
            float maxValue = chartDataList[0].y;

            for (int i = 0; i < chartDataList.Count; i++)
            {
                if (chartDataList[i].y > maxValue)
                {
                    maxValue = chartDataList[i].y;
                }
            }
            while ((int)maxValue % maxYValueStep != 0)
            {
                maxValue++;
            }
            maxValue = Mathf.Floor(maxValue);
            return maxValue;
        }
    }
}