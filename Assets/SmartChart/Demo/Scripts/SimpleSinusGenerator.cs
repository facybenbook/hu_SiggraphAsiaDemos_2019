using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ToucanSystems;

/// <summary>
/// Test script for pushing some sinus values onto the chart.
/// </summary>
public class SimpleSinusGenerator : MonoBehaviour
{

    [SerializeField]
    private SmartChart chart;

    void OnEnable()
    {
        StartCoroutine(GenerateSinus());
    }

    private IEnumerator GenerateSinus()
    {
        yield return new WaitForSeconds(6);
        List<Vector2> sinus = new List<Vector2>();
        for (float i = -4.71f; i <= 4.71; i += 0.2f)
        {
            sinus.Add(new Vector2(i, Mathf.Sin(i)));
            chart.chartData[0].data = sinus.ToArray();
            chart.UpdateChart();
            yield return null;
        }
        
    }
}
