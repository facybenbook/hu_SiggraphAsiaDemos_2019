using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ToucanSystems;

public class SimpleChartSlider : MonoBehaviour 
{
	[SerializeField]
	private SmartChart chart;
	[SerializeField]
	private float chartScale = 20;
	[SerializeField]
	private Slider slider;

	private void Awake()
	{
		OnSliderValueChanged (slider.value);
	}

	public void OnSliderValueChanged(float value)
	{
		if (chart.chartData.Length > 0 && chart.chartData[0].data.Length > 0)
		{
			float minVal = float.PositiveInfinity;
			float maxVal = float.NegativeInfinity;

			for (int i = 0; i < chart.chartData [0].data.Length; i++) 
			{
				var val = chart.chartData [0].data [i].x;

				if (val < minVal) 
				{
					minVal = val;
				}

				if (val > maxVal) 
				{
					maxVal = val;
				}
			}

			float chartStart = Mathf.Lerp (minVal, maxVal - chartScale, value);
			chart.minXValue = chartStart;
			chart.maxXValue = chartStart + chartScale;

			chart.UpdateChart ();
		}

	}
}
