/*
 * Part of: SmartChart
 * Monitors specified value and uses ChartDataPusher to display it on SmartChart with specified time interval.
 */

using System.Collections;
using UnityEngine;

namespace ToucanSystems
{
    /// <summary>
    /// Monitors specified value and uses ChartDataPusher to display it on SmartChart with specified time interval.
    /// </summary>
    [RequireComponent(typeof(ChartDataPusher))]
    public class ChartDataMonitor : MonoBehaviour
    {
        [HideInInspector]
        public float monitorValue;

        [SerializeField]
        private float updateInterval = 1;

        private ChartDataPusher dataPusher;

        private void OnEnable()
        {
            dataPusher = GetComponent<ChartDataPusher>();
            StartCoroutine(MonitorDataCoroutine());
        }

        private IEnumerator MonitorDataCoroutine()
        {
            yield return new WaitForSeconds(1);
            while (true)
            {
                dataPusher.PushValue(monitorValue);
                yield return new WaitForSeconds(updateInterval);
            }
        }
    }
}