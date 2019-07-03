using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ToucanSystems;

/// <summary>
/// Test code for building captions for the chart.
/// </summary>
[ExecuteInEditMode]
public class SimpleCaptions : MonoBehaviour
{

    [SerializeField]
    private SmartChart chart;
    [SerializeField]
    private List<Text> captionsTexts;
    [SerializeField]
    private List<string> captions;
    [SerializeField]
    private List<Color> dataColors;
    [SerializeField]
    private List<Image> colorImages;

    public void UpdateCaptions()
    {
        OnValidate();
    }

    private void OnValidate()
    {
        if (chart != null)
        {
            dataColors = new List<Color>();
            for (int i = 0; i < chart.chartData.Length; i++)
            {
                dataColors.Add(chart.chartData[i].dataLineColor);
            }
        }

        if (colorImages.Count == dataColors.Count)
        {
            for (int i = 0; i < dataColors.Count; i++)
            {
                colorImages[i].color = dataColors[i];
            }
        }

        if (captionsTexts.Count == captions.Count)
        {
            for (int i = 0; i < captions.Count; i++)
            {
                captionsTexts[i].text = captions[i];
            }
        }
    }
}