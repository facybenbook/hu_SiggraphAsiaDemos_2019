using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using ToucanSystems;

/// <summary>
/// Test script for animating chart.
/// </summary>
public class SimpleChartAnimation : MonoBehaviour
{
    [SerializeField]
    private SmartChart chart;
    [SerializeField]
    private int dataIndex;
    [SerializeField]
    private Image[] markers;
    [SerializeField]
    private RectTransform dataRT;
    [SerializeField]
    private RectTransform fillRT;
    [SerializeField]
    private float duration = 0.5f;
    [SerializeField]
    private float delay = 0;
    [SerializeField]
    private AnimationCurve animationCurve;

    private void Start()
    {
        Animate();
    }

    public void Animate()
    {
        StopAllCoroutines();
        StartCoroutine(AnimateCoroutine());
    }

    private IEnumerator AnimateCoroutine()
    {
        yield return new WaitForSeconds(delay);
        for (float i = 0; i <= duration; i += Time.deltaTime)
        {
            dataRT.localScale = new Vector3(1, Mathf.Lerp(0, 2, animationCurve.Evaluate(i / duration)), 1);
            fillRT.localScale = dataRT.localScale;
            yield return null;
        }
        for (float i = 0; i <= duration / 2; i += Time.deltaTime)
        {
            Color newColor = new Color(1, 1, 1, Mathf.SmoothStep(0, 1, i / (duration / 2)));
            chart.chartData[dataIndex].markerColor = newColor;
            for (int j = 0; j < markers.Length; j++)
            {
                markers[j].color = newColor;
            }
            yield return null;
        }
    }
}