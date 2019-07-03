using System.Collections;
using UnityEngine;

/// <summary>
/// Test script for animating chart resizing.
/// </summary>
public class SimpleChartScaling : MonoBehaviour
{

    [SerializeField]
    private RectTransform chartRectTransform;

    private Vector2 randSize, initSize;
    [SerializeField]
    private float speed = 100.0f;
    private float startTime;
    private float journeyLength;

    void OnEnable()
    {
        randSize = new Vector2(0, 0);
        initSize = chartRectTransform.sizeDelta;
        startTime = Time.time;
        journeyLength = Vector2.Distance(initSize, randSize);
        StartCoroutine(Resize());
    }

    IEnumerator Resize()
    {
        yield return new WaitForSeconds(1f);
        while (true)
        {
            startTime = Time.time;
            float randWidth = Random.Range(1400, 1800);
            float randHeight = Random.Range(500, 800);

            randSize = new Vector2(randWidth, randHeight);
            initSize = chartRectTransform.sizeDelta;
            journeyLength = Vector2.Distance(initSize, randSize);
            yield return new WaitForSeconds(journeyLength/speed);
        }
    }

    private void Update()
    {
        float distCovered = (Time.time - startTime) * speed;
        float fracJourney = distCovered / journeyLength;

        Vector2 newSize = Vector2.Lerp(initSize, randSize, fracJourney);
        chartRectTransform.position += (Vector3)(chartRectTransform.sizeDelta - newSize) / 2;
        chartRectTransform.sizeDelta = newSize;
    }


}
