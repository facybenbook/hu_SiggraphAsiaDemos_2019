/*
 * Part of: SmartChart
 * Class respresenting single data set for SmartChart.
 */

using UnityEngine;

namespace ToucanSystems
{
    /// <summary>
    /// Class respresenting single data set for the chart.
    /// </summary>
    [System.Serializable]
    public class SmartChartData
    {

        /// <summary>
        /// Color used to display data line.
        /// </summary>
        public Color32 dataLineColor = Color.black;
        /// <summary>
        /// Material used to display data line.
        /// </summary>
        public Sprite dataLineSprite;
        /// <summary>
        /// Color used to fill area under the chart line.
        /// </summary>
        public Color32 dataFillColor = Color.black;
        /// <summary>
        /// Texture used to fill area under the chart line.
        /// </summary>
        public Texture dataFillTexture;
        /// <summary>
        /// Sprite that will be displayed as marker.
        /// </summary>
        public Sprite markerSprite;
        /// <summary>
        /// Dimensions of markers.
        /// </summary>
        public Vector2 markerSize;
        /// <summary>
        /// Color tint of markers.
        /// </summary>
        public Color markerColor;
        /// <summary>
        /// Thickness of line that will represent data.
        /// </summary>
        [Range(0, 100)]
        public float dataLineWidth = 1;
        /// <summary>
        /// Actual data, stored in array of Vector2s.
        /// </summary>
        public Vector2[] data;
    }
}
