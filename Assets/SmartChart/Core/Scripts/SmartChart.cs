/**
* Title : Smart SmartChart ™
* Author : Toucan Systems (http://toucan-systems.pl)
*
* Function overview: Creates line chart working with Unity built-in UI system.
* Dependencies: ui-extensions by ddreaper (https://bitbucket.org/ddreaper/unity-ui-extensions), Iterpolate.cs by Fernando Zapata.
* Use instructions: Create chart on Canvas (GameObject/UI/SmartChart) or add SmartChart component to existing GameObject.
*
* Version: 1.0.0.0
* Released: 2017.10
**/

using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace ToucanSystems
{
    /// <summary>
    /// Component used to draw line charts in Unity build-in UI System.
    /// </summary>
    [ExecuteInEditMode]
    [RequireComponent(typeof(RectTransform))]
    public class SmartChart : MonoBehaviour
    {
        #region SmartChart data variables
        /// <summary>
        /// Data to be displayed on chart.
        /// </summary>
        public SmartChartData[] chartData;

        /// <summary>
        /// If set true line representing data will be smooth.
        /// </summary>
        public bool smoothData = true;
        /// <summary>
        /// Number of interpolated points between actual data points.
        /// </summary>
        [Range(1, 40)]
        public int dataSmoothing = 10;
        /// <summary>
        /// Determines if area under data lines should be filled;
        /// </summary>
        public bool fillAreaUnderLine = true;
        /// <summary>
        /// Determines if line caps should be shown.
        /// </summary>
        public bool showLineCaps = false;
        #endregion

        #region Sizing options variables
        [SerializeField]
        private float xMargin = 0f;
        [SerializeField]
        private float yMargin = 0f;
        #endregion

        #region Axes options variables
        [SerializeField]
        [Range(0, 100)]
        private float axesWidth = 1f;
        [SerializeField]
        private Color32 axesColor = Color.black;
        [SerializeField]
        private bool invertXAxis = false;
        [SerializeField]
        private bool invertYAxis = false;
        [SerializeField]
        private bool stickXAxisToZero = false;
        [SerializeField]
        private bool stickYAxisToZero = false;
        #endregion

        #region Grid options variables
        [SerializeField]
        private bool showXGrid = false;
        [SerializeField]
        private bool showYGrid = false;
        [SerializeField]
        private Color32 gridColor = Color.black;
        [SerializeField]
        [Range(0, 100)]
        private float gridLineWidth = 1;
        [SerializeField]
        private float xGridStep = 1;
        [SerializeField]
        private float yGridStep = 1;
        #endregion

        #region Labels options variables
        /// <summary>
        /// If set true, labels on X axis will be displayed.
        /// </summary>
        [SerializeField]
        private bool showLabelsOnXAxis = true;
        /// <summary>
        /// If set true, labels on Y axis will be displayed.
        /// </summary>
        [SerializeField]
        private bool showLabelsOnYAxis = true;
        /// <summary>
        /// Minimal value displayed on X axis.
        /// </summary>
        public float minXValue = 0;
        /// <summary>
        /// Maximum value displayed on X axis.
        /// </summary>
        public float maxXValue = 100;
        /// <summary>
        /// Minimal value displayed on Y axis.
        /// </summary>
        public float minYValue = 0;
        /// <summary>
        /// Maximum value displayed on Y axis.
        /// </summary>
        public float maxYValue = 100;
        /// <summary>
        /// Number of value labels displayed on X axis.
        /// </summary>
        [Range(2, 100)]
        public int xValuesCount = 2;
        /// <summary>
        /// Number of value labels displayed on Y axis.
        /// </summary>
        [Range(2, 100)]
        public int yValuesCount = 2;
        /// <summary>
        /// Size of labels text font.
        /// </summary>
        [Range(1, 100)]
        public int labelsFontSize = 14;
        [SerializeField]
        private Color32 labelsFontColor = Color.black;
        [SerializeField]
        private Font labelsFont;
        [SerializeField]
        [Range(0, 8)]
        private int xValuesPrecision = 2;
        [SerializeField]
        [Range(0, 8)]
        private int yValuesPrecision = 2;
        [SerializeField]
        private float xValuesOffset = 0;
        [SerializeField]
        private float yValuesOffset = 0;
        #endregion

        #region Markers options variables
        public bool showMarkers = false;
        [Range(1, 100)]
        public int markersLabelsFontSize = 14;
        [SerializeField]
        private Color markersLabelsFontColor = Color.white;
        [SerializeField]
        private Font markersLabelsFont;
        [SerializeField]
        private Vector2 markersFieldOffset;
        [SerializeField]
        private Vector2 markersLabelsOffset;
        [SerializeField]
        private bool showValuePrefix = false;
        [SerializeField]
        private string valuePrefix;
        [SerializeField]
        private Sprite markersLabelsBackground;
        [SerializeField]
        private InteractionType markerInteractionType;
        #endregion

        #region Other variables
        private float chartWidth, chartHeight;
        private int lastChartDataLength;
        private RectTransform chartRT;
        public Material fillMaterial;
#pragma warning disable 0414
        [SerializeField]
        private bool useCustomHandles = false;
        private Vector2 positionAdjustment2
        {
            get
            {
                return new Vector2(chartRT.pivot.x * chartWidth, chartRT.pivot.y * chartHeight); ;
            }
        }
        private Vector3 positionAdjustment3
        {
            get
            {
                return new Vector3(chartRT.pivot.x * chartWidth, chartRT.pivot.y * chartHeight, 0);
            }
        }
        #endregion

        private void Awake()
        {
            if (chartData == null)
            {
                chartData = new SmartChartData[0];
            }
            lastChartDataLength = chartData.Length;
        }

        /// <summary>
        /// Updates chart resolution, redraws data, axes and grid.
        /// </summary>
        public void UpdateChart()
        {
            Resources.UnloadUnusedAssets();
            SetupChartResolution();
            DrawData();
            DrawAxes();
            DrawGrid();
            UpdateLabelsPositionsOnResize();
        }

        /// <summary>
        /// Creates and sets up axes labels.
        /// </summary>
        /// <param name="deleteOldLabels">Determines if old labels should be deleted or just updated.</param>
        public void SetupValues(bool deleteOldLabels)
        {
            if (deleteOldLabels)
            {
                DeleteLabels();
            }

            if (showLabelsOnXAxis)
            {
                float stepX = (maxXValue - minXValue) / (xValuesCount - 1);

                for (int i = 0; i < xValuesCount; i++)
                {
                    DrawLabel("label" + i.ToString() + "x", (Mathf.Round((stepX * i + minXValue) * Mathf.Pow(10, xValuesPrecision)) / Mathf.Pow(10, xValuesPrecision)).ToString(), i, true);
                }
            }

            if (showLabelsOnYAxis)
            {
                float stepY = (maxYValue - minYValue) / (yValuesCount - 1);

                for (int i = 0; i < yValuesCount; i++)
                {
                    DrawLabel("label" + i.ToString() + "y", (Mathf.Round((stepY * i + minYValue) * Mathf.Pow(10, yValuesPrecision)) / Mathf.Pow(10, yValuesPrecision)).ToString(), i, false);
                }
            }
        }

        private void UpdateLabelsPositionsOnResize()
        {
            if (showLabelsOnXAxis)
            {
                for (int i = 0; i < xValuesCount; i++)
                {
                    UpdateLabelPosition(FindChild("label" + i.ToString() + "x"), i, true);
                }
            }
            if (showLabelsOnYAxis)
            {
                for (int i = 0; i < yValuesCount; i++)
                {
                    UpdateLabelPosition(FindChild("label" + i.ToString() + "y"), i, false);
                }
            }
        }

        private void SetupChartResolution()
        {
            if (chartRT == null)
            {
                chartRT = GetComponent<RectTransform>();
                chartRT.pivot = new Vector2(0, 0);
            }
            chartWidth = chartRT.sizeDelta.x;
            chartHeight = chartRT.sizeDelta.y;
        }

        //private void OnValidate()
        //{
        //    UpdateChart();
        //}

        private void OnRectTransformDimensionsChange()
        {
            UpdateChart();
            if (chartRT.sizeDelta.x < 4)
            {
                chartRT.sizeDelta = new Vector2(4, chartRT.sizeDelta.y);
            }
            if (chartRT.sizeDelta.y < 4)
            {
                chartRT.sizeDelta = new Vector2(chartRT.sizeDelta.x, 4);
            }
        }

        private void DrawLabel(string labelName, string labelValue, float labelPosition, bool isOnXAxis)
        {
            GameObject label = FindChild(labelName);

            if (label == null)
            {
                label = new GameObject(labelName);
                label.transform.SetParent(FindChild("LabelsContainer").transform);
                label.AddComponent<RectTransform>();
                Text labelTextTmp = label.AddComponent<Text>();
                labelTextTmp.alignment = TextAnchor.MiddleCenter;
                label.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
                ContentSizeFitter labelCSF = label.AddComponent<ContentSizeFitter>();
                labelCSF.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                labelCSF.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            }
            Text labelText = label.GetComponent<Text>();

            labelText.text = labelValue;
            labelText.color = labelsFontColor;
            labelText.fontSize = labelsFontSize;
            labelText.font = labelsFont != null ? labelsFont : Resources.GetBuiltinResource<Font>("Arial.ttf");

            UpdateLabelPosition(label, labelPosition, isOnXAxis);
        }

        private void DrawMarker(string markerName, string markerValue, Vector2 markerPosition, Sprite markerSprite, Vector2 markerSize, Color markerColor)
        {
            GameObject marker = FindChild(markerName);

            if (marker == null)
            {
                marker = new GameObject(markerName);
                marker.transform.SetParent(FindChild("MarkersContainer").transform);
                marker.AddComponent<RectTransform>();
                marker.AddComponent<Image>();

                marker.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
                marker.GetComponent<RectTransform>().anchorMax = new Vector2(0, 0);
                marker.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
                marker.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);

                marker.AddComponent<ChartMarkerController>();
                GameObject background = new GameObject("background");
                background.transform.SetParent(marker.transform);
                CanvasGroup backgroundCG = background.AddComponent<CanvasGroup>();
                backgroundCG.alpha = 0;
                HorizontalLayoutGroup backgroundHLGTmp = background.AddComponent<HorizontalLayoutGroup>();
                backgroundHLGTmp.childForceExpandHeight = false;
                backgroundHLGTmp.childAlignment = TextAnchor.MiddleCenter;
                backgroundHLGTmp.childControlHeight = true;
                backgroundHLGTmp.childControlWidth = true;
                ContentSizeFitter backgroundCSF = background.AddComponent<ContentSizeFitter>();
                backgroundCSF.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                backgroundCSF.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                background.AddComponent<Image>();
                background.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
                background.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
                background.GetComponent<RectTransform>().localPosition = markersFieldOffset;
                background.SetActive(false);
                GameObject markerLabel = new GameObject("label");
                Text markerLabelTextTmp = markerLabel.AddComponent<Text>();
                markerLabelTextTmp.text = markerValue;
                markerLabelTextTmp.alignment = TextAnchor.MiddleCenter;
                markerLabel.transform.SetParent(background.transform);
                markerLabel.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
                markerLabel.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
                markerLabel.GetComponent<RectTransform>().localPosition = markersLabelsOffset;
                marker.GetComponent<ChartMarkerController>().markerLabel = markerLabel;
                marker.GetComponent<ChartMarkerController>().background = background;
            }
            ChartMarkerController markerController = marker.GetComponent<ChartMarkerController>();
            Text markerLabelText = markerController.markerLabel.GetComponentInChildren<Text>();
            Image backgroundImage = markerController.background.GetComponent<Image>();
            markerController.interactionType = markerInteractionType;
            backgroundImage.sprite = markersLabelsBackground;
            Image markerImage = marker.GetComponent<Image>();
            RectTransform markerRT = marker.GetComponent<RectTransform>();

            markerImage.sprite = markerSprite;
            markerImage.color = markerColor;
            markerRT.anchoredPosition = markerPosition;
            markerRT.sizeDelta = markerSize;

            markerLabelText.fontSize = markersLabelsFontSize;
            markerLabelText.font = markersLabelsFont != null ? markersLabelsFont : Resources.GetBuiltinResource<Font>("Arial.ttf");
            markerLabelText.color = markersLabelsFontColor;

            if (showValuePrefix)
            {
                markerLabelText.text = valuePrefix + " " + markerValue;
            }
            else
            {
                markerLabelText.text = markerValue;
            }

            backgroundImage.GetComponent<RectTransform>().localPosition = markersFieldOffset;
            HorizontalLayoutGroup backgroundHLG = backgroundImage.GetComponent<HorizontalLayoutGroup>();
            if (backgroundHLG != null)
            {
                backgroundHLG.padding.left = (int)markersLabelsOffset.x;
                backgroundHLG.padding.bottom = (int)markersLabelsOffset.y;
            }
        }

        public void DeleteMarkers(int dataCount)
        {
            GameObject markersContainer = FindChild("MarkersContainer");
            ChartMarkerController[] markers = markersContainer.GetComponentsInChildren<ChartMarkerController>();
            if (dataCount < markers.Length)
            {
                for (int i = 0; i < markers.Length; i++)
                {
                    if (gameObject.activeInHierarchy)
                    {
#if UNITY_EDITOR
                        if (markers[i] != null && !ReferenceEquals(markers[i], null))
                        {
                            DestroyImmediate(markers[i].gameObject);
                            markers[i] = null;
                        }
#else
                        if (markers[i] != null)
                        {
                            Destroy(markers[i].gameObject);
                            markers[i] = null;
                        }
#endif
                    }
                }
            }
        }

        private void UpdateLabelPosition(GameObject label, float labelPosition, bool isOnXAxis)
        {
            if(label == null)
            {
                return;
            }

            if (isOnXAxis)
            {
                float xPosition = xMargin + (chartWidth - xMargin) * (labelPosition / (xValuesCount - 1)) - chartWidth / 2;
                float inverter = 0;
                if (invertXAxis)
                {
                    inverter = chartHeight;
                }
                label.GetComponent<RectTransform>().localPosition = new Vector3(xPosition, yMargin / 2 + xValuesOffset + inverter - chartHeight / 2, 0);
            }
            else
            {
                float yPosition = yMargin + (chartHeight - yMargin) * (labelPosition / (yValuesCount - 1)) - chartHeight / 2;
                float inverter = 0;
                if (invertYAxis)
                {
                    inverter = chartWidth;
                }
                label.GetComponent<RectTransform>().localPosition = new Vector3(xMargin / 2 + yValuesOffset + inverter - chartWidth / 2, yPosition, 0);
            }
        }

        private void DrawAxes()
        {
            SetupAxisMesh("XAxis", true);
            SetupAxisMesh("YAxis", false);
        }

        private void SetupAxisMesh(string axisName, bool isXAxis)
        {
            GameObject axis = FindChild(axisName);

            if (axis == null)
            {
                axis = new GameObject();
                axis.transform.name = axisName;
                axis.transform.SetParent(FindChild("AxesAndGridContainer").transform);
                axis.AddComponent<UILineRenderer>();
                axis.GetComponent<RectTransform>().pivot = chartRT.pivot;
                axis.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
                axis.GetComponent<UILineRenderer>().LineCaps = true;
            }
            RectTransform axisRT = axis.GetComponent<RectTransform>();
            UILineRenderer axisLine = axis.GetComponent<UILineRenderer>();

            axisRT.position = chartRT.position;
            axisRT.sizeDelta = chartRT.sizeDelta;
            axisLine.color = axesColor;
            axisLine.LineThickness = axesWidth;

            Vector2 vec1;
            Vector2 vec2;

            if (isXAxis)
            {
                if (stickXAxisToZero && minYValue < 0)
                {
                    float verticalZero = chartHeight * Mathf.Abs(minYValue) / (maxYValue - minYValue);
                    vec1 = new Vector2(xMargin, yMargin / 2 + verticalZero);
                    vec2 = new Vector2(chartWidth, yMargin / 2 + verticalZero);
                }
                else
                {
                    if (invertXAxis)
                    {
                        vec1 = new Vector2(xMargin - axesWidth / 2, chartHeight + axesWidth / 2);
                        vec2 = new Vector2(chartWidth + axesWidth / 2, chartHeight + axesWidth / 2);
                    }
                    else
                    {
                        vec1 = new Vector2(xMargin - axesWidth / 2, yMargin - axesWidth / 2);
                        vec2 = new Vector2(chartWidth + axesWidth / 2, yMargin - axesWidth / 2);
                    }
                }
            }
            else
            {
                if (stickYAxisToZero && minXValue < 0)
                {
                    float horizontalZero = chartWidth * Mathf.Abs(minXValue) / (maxXValue - minXValue);
                    vec1 = new Vector2(xMargin / 2 + horizontalZero, yMargin);
                    vec2 = new Vector2(xMargin / 2 + horizontalZero, chartHeight);
                }
                else
                {
                    if (invertYAxis)
                    {
                        vec1 = new Vector2(chartWidth + axesWidth / 2, yMargin - axesWidth / 2);
                        vec2 = new Vector2(chartWidth + axesWidth / 2, chartHeight + axesWidth / 2);
                    }
                    else
                    {
                        vec1 = new Vector2(xMargin - axesWidth / 2, yMargin - axesWidth / 2);
                        vec2 = new Vector2(xMargin - axesWidth / 2, chartHeight + axesWidth / 2);
                    }
                }
            }

            vec1 -= positionAdjustment2;
            vec2 -= positionAdjustment2;

            List<Vector2> vecs = new List<Vector2>();
            vecs.Add(vec1);
            vecs.Add(vec2);

            axisLine.Points = vecs.ToArray();
            axis.transform.SetAsFirstSibling();
        }

        private void DrawData()
        {
            if (chartData != null)
            {
                for (int i = 0; i < chartData.Length; i++)
                {
                    GameObject data = FindChild("data" + i.ToString());
                    GameObject fill = FindChild("fill" + i.ToString());

                    if (data == null)
                    {
                        fill = new GameObject("fill" + i.ToString());
                        fill.transform.SetParent(FindChild("FillContainer").transform);
                        RectTransform fillRTTmp = fill.AddComponent<RectTransform>();

                        data = new GameObject();
                        data.transform.name = "data" + i.ToString();
                        data.transform.SetParent(FindChild("DataContainer").transform);
                        data.AddComponent<UILineRenderer>();
                        data.GetComponent<RectTransform>().pivot = chartRT.pivot;
                        data.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);

                        fillRTTmp.pivot = -data.GetComponent<RectTransform>().pivot;
                        fillRTTmp.localScale = new Vector3(1, 1, 1);

                        RawImage fillRI = fill.AddComponent<RawImage>();
                        fillRI.material = new Material(fillMaterial);
                        fillRI.texture = new Texture2D((int)(chartWidth - xMargin), 1);
                    }

                    RectTransform dataRT = data.GetComponent<RectTransform>();
                    UILineRenderer dataLine = data.GetComponent<UILineRenderer>();
                    dataLine.LineCaps = showLineCaps;
                    dataRT.position = chartRT.position - positionAdjustment3;
                    dataRT.sizeDelta = chartRT.sizeDelta;
                    dataLine.color = chartData[i].dataLineColor;
                    dataLine.LineThickness = chartData[i].dataLineWidth;
                    dataLine.sprite = chartData[i].dataLineSprite;

                    RectTransform fillRT = fill.GetComponent<RectTransform>();
                    RawImage fillImage = fill.GetComponent<RawImage>();

                    fillRT.localPosition = dataRT.localPosition + new Vector3(xMargin, yMargin, 0);
                    fillRT.sizeDelta = dataRT.sizeDelta - new Vector2(xMargin, yMargin);
                    List<Vector3> displayData = new List<Vector3>();
                    for (int j = 0; j < chartData[i].data.Length; j++)
                    {
                        float x = xMargin + (chartWidth - xMargin) * (chartData[i].data[j].x / (maxXValue - minXValue)) - (chartWidth - xMargin) * (minXValue / (maxXValue - minXValue));
                        float y = yMargin + (chartHeight - yMargin) * (chartData[i].data[j].y / (maxYValue - minYValue)) - (chartHeight - yMargin) * (minYValue / (maxYValue - minYValue));
                        Vector3 vec = new Vector3(x, y);
                        if (showMarkers)
                        {
                            DrawMarker("marker" + j + "_" + data.transform.name, chartData[i].data[j].y.ToString(), vec, chartData[i].markerSprite, chartData[i].markerSize, chartData[i].markerColor);
                        }
                        displayData.Add(vec);
                    }

                    if (smoothData)
                    {
                        IEnumerable<Vector3> interpolatedPoints = Interpolate.NewCatmullRom(displayData.ToArray(), dataSmoothing, false);
                        displayData = interpolatedPoints.ToList();
                    }

                    List<Vector2> finalPoints = new List<Vector2>();

                    for (int j = 0; j < displayData.Count; j++)
                    {
                        finalPoints.Add(displayData[j]);
                    }

                    if (fillAreaUnderLine && finalPoints.Count > 1)
                    {
                        List<Vector2> linePoints = GetBresenhamLine(finalPoints);
                        Texture fillTexture = fillImage.texture;
                        Color underColor = chartData[i].dataFillColor;

                        if (fillTexture.width != chartWidth - xMargin || fillTexture.height != 1)
                        {
                            fillTexture = new Texture2D((int)chartWidth - (int)xMargin, 1);
                            fillRT.sizeDelta = new Vector2(chartWidth - xMargin, chartHeight - yMargin);
                        }

                        Color32[] colorArray = new Color32[fillTexture.width];
                        int linePointsIterator = 0;
                        int colorArrayIterator = 0;
                        
                        while (linePointsIterator < linePoints.Count && linePoints[linePointsIterator].x <= chartWidth - xMargin)
                        {
                            if (colorArrayIterator < colorArray.Length)
                            {
                                if (linePoints[linePointsIterator].x >= minXValue)
                                {
                                    float setY = linePoints[linePointsIterator].y;
                                    linePointsIterator++;
                                    setY -= yMargin;
                                    if (setY >= chartHeight - yMargin)
                                    {
                                        setY = chartHeight - yMargin;
                                    }
                                    else if (setY <= 0)
                                    {
                                        setY = 0;
                                    }

                                    float yValue = setY / (chartHeight - yMargin);
                                    byte a = (byte)(yValue * 255);
                                    float reminder = yValue * 255 - a;
                                    byte b = (byte)(reminder * 255);
                                    colorArray[colorArrayIterator] = new Color32(a, b, 0, 0);
                                    colorArrayIterator++;
                                }
                                else
                                {
                                    linePointsIterator++;
                                }
                            }
                            else
                            {
                                break;
                            }
                        }

                        //if (j < linePoints.Count && linePoints[j].x >= minXValue)
                        //{
                        //    if (linePoints[j].x >= minXValue)
                        //    {
                        //        float setY = linePoints[j].y;
                        //        setY -= yMargin;
                        //        if (setY >= chartHeight - yMargin)
                        //        {
                        //            setY = chartHeight - yMargin;
                        //        }
                        //        else if (setY <= 0)
                        //        {
                        //            setY = 0;
                        //        }

                        //        float yValue = setY / (chartHeight - yMargin);
                        //        byte a = (byte)(yValue * 255);
                        //        float reminder = yValue * 255 - a;
                        //        byte b = (byte)(reminder * 255);
                        //        colorArray[colorArrayIterator] = new Color32(a, b, 0, 0);
                        //        colorArrayIterator++;
                        //    }
                        //}
                        //else
                        //{
                        //    colorArray[colorArrayIterator] = new Color32(50, 0, 0, 0);
                        //    colorArrayIterator++;
                        //}


                        if (colorArray.Length == fillTexture.width && colorArray.Length > 2)
                        {
                            (fillTexture as Texture2D).SetPixels32(colorArray);
                            (fillTexture as Texture2D).Apply();
                            fillImage.texture = fillTexture;
                            fillImage.color = underColor;
                            fillImage.material.SetTexture("_SecTex", chartData[i].dataFillTexture);
                            float shift = (minXValue - chartData[i].data[0].x) / (maxXValue - minXValue);
                            if (minXValue >= chartData[i].data[0].x)
                            {
                                shift = 0;
                            }
                            fillImage.material.SetFloat("_ShiftFactor", shift);
                        }
                    }
                    else
                    {
                        fillImage.color = Color.clear;
                    }
                    dataLine.Points = finalPoints.ToArray();
                }
                for (int i = 0; i < chartData.Length; i++)
                {
                    GameObject data = FindChild("data" + i.ToString());
                    GameObject fill = FindChild("fill" + i.ToString());
                    fill.transform.SetSiblingIndex(i);
                    data.transform.SetSiblingIndex(i);
                }
            }
            if (!showMarkers)
            {
                DeleteMarkers(0);
            }
        }

        private List<Vector2> GetBresenhamLine(List<Vector2> points)
        {
            List<Vector2> linePoints = new List<Vector2>();
            for (int i = 0; i < points.Count - 1; i++)
            {
                int fromX = (int)points[i].x;
                int toX = (int)points[i + 1].x;
                for (int j = fromX; j < toX; j++)
                {
                    float newY = ((j - fromX) / (float)(toX - fromX)) * (points[i + 1].y - points[i].y) + points[i].y;
                    linePoints.Add(new Vector2(j, newY));
                }
            }
            return linePoints;
        }

        private void DeleteLabels()
        {
            Text[] labels = FindChild("LabelsContainer").GetComponentsInChildren<Text>();
            if (labels.Length != xValuesCount + yValuesCount)
            {
                for (int i = 0; i < labels.Length; i++)
                {
                    if (gameObject.activeInHierarchy && labels[i] != null && !ReferenceEquals(labels[i], null))
                    {
                        if (Application.isEditor)
                        {
                            DestroyImmediate(labels[i].gameObject);
                            labels[i] = null;
                        }
                        else
                        {
                            Destroy(labels[i].gameObject);
                            labels[i] = null;
                        }
                    }
                }
            }
        }

        private void Update()
        {
            if (chartData.Length != lastChartDataLength)
            {
                RedrawData();
                lastChartDataLength = chartData.Length;
            }
        }

        private void RedrawData()
        {
            for (int i = 0; i < lastChartDataLength; i++)
            {
                GameObject data = FindChild("data" + i.ToString());
                GameObject fill = FindChild("fill" + i.ToString());
                if (Application.isEditor)
                {
                    DestroyImmediate(data);
                    DestroyImmediate(fill);
                }
                else
                {
                    Destroy(data);
                    Destroy(fill);
                }
            }
            DrawData();
        }

        private void DrawGrid()
        {
            GameObject grid = FindChild("chartGrid");
            if (grid == null)
            {
                grid = new GameObject();
                grid.transform.name = "chartGrid";
                grid.transform.SetParent(FindChild("AxesAndGridContainer").transform);
                grid.AddComponent<UILineRenderer>();
                grid.GetComponent<UILineRenderer>().LineList = true;
                grid.GetComponent<RectTransform>().pivot = chartRT.pivot;
                grid.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            }

            RectTransform gridRT = grid.GetComponent<RectTransform>();
            UILineRenderer gridLine = grid.GetComponent<UILineRenderer>();

            gridRT.position = chartRT.position - positionAdjustment3;
            gridRT.sizeDelta = chartRT.sizeDelta;
            gridLine.color = gridColor;
            gridLine.LineThickness = gridLineWidth;

            List<Vector2> vecs = new List<Vector2>();

            if (showXGrid)
            {
                for (int i = 0; i < xValuesCount * xGridStep; i++)
                {
                    float x = xMargin + ((chartWidth - xMargin) * ((float)i / (xValuesCount - 1))) / xGridStep;
                    if (x <= chartWidth)
                    {
                        vecs.Add(new Vector2(x, yMargin));
                        vecs.Add(new Vector2(x, chartHeight + gridLineWidth / 2));
                    }
                }
            }

            if (showYGrid)
            {
                for (int i = 0; i < yValuesCount * yGridStep; i++)
                {
                    float y = yMargin + ((chartHeight - yMargin) * ((float)i / (yValuesCount - 1))) / yGridStep;
                    if (y <= chartHeight)
                    {
                        vecs.Add(new Vector2(xMargin, y));
                        vecs.Add(new Vector2(chartWidth + gridLineWidth / 2, y));
                    }
                }
            }
            gridLine.Points = vecs.ToArray();
            grid.transform.SetAsFirstSibling();
        }

        private GameObject FindChild(string name)
        {
            Transform[] children = GetComponentsInChildren<RectTransform>();
            foreach (var child in children)
            {
                if (child.name == name)
                {
                    return child.gameObject;
                }
            }
            return null;
        }
#if UNITY_EDITOR
        private void Reset()
        {
            if (gameObject.GetComponent<RectTransform>() == null)
            {
                chartRT = gameObject.AddComponent<RectTransform>();
            }

            fillMaterial = (Material)Resources.Load("ChartMaterial", typeof(Material));

            if (FindChild("AxesAndGridContainer") == null)
            {
                GameObject axesAndGridContainer = new GameObject("AxesAndGridContainer");
                GameObjectUtility.SetParentAndAlign(axesAndGridContainer, gameObject);
                RectTransform axesAndGridContainerRT = axesAndGridContainer.AddComponent<RectTransform>();
                axesAndGridContainerRT.anchorMin = new Vector2(0, 0);
                axesAndGridContainerRT.anchorMax = new Vector2(1, 1);
                axesAndGridContainerRT.offsetMin = new Vector2(0, 0);
                axesAndGridContainerRT.offsetMax = new Vector2(0, 0);
            }

            if (FindChild("LabelsContainer") == null)
            {
                GameObject labelsContainer = new GameObject("LabelsContainer");
                GameObjectUtility.SetParentAndAlign(labelsContainer, gameObject);
                RectTransform labelsContainerRT = labelsContainer.AddComponent<RectTransform>();
                labelsContainerRT.anchorMin = new Vector2(0, 0);
                labelsContainerRT.anchorMax = new Vector2(1, 1);
                labelsContainerRT.offsetMin = new Vector2(0, 0);
                labelsContainerRT.offsetMax = new Vector2(0, 0);
            }

            if (FindChild("DataContainer") == null)
            {
                GameObject dataContainer = new GameObject("DataContainer");
                GameObjectUtility.SetParentAndAlign(dataContainer, gameObject);
                RectTransform dataContainerRT = dataContainer.AddComponent<RectTransform>();
                dataContainerRT.anchorMin = new Vector2(0, 0);
                dataContainerRT.anchorMax = new Vector2(1, 1);
                dataContainerRT.offsetMin = new Vector2(0, 0);
                dataContainerRT.offsetMax = new Vector2(0, 0);
            }

            if (FindChild("FillContainer") == null)
            {
                GameObject fillContainer = new GameObject("FillContainer");
                GameObjectUtility.SetParentAndAlign(fillContainer, gameObject);
                RectTransform fillContainerRT = fillContainer.AddComponent<RectTransform>();
                fillContainerRT.anchorMin = new Vector2(0, 0);
                fillContainerRT.anchorMax = new Vector2(1, 1);
                fillContainerRT.offsetMin = new Vector2(0, 0);
                fillContainerRT.offsetMax = new Vector2(0, 0);
            }

            if (FindChild("MarkersContainer") == null)
            {
                GameObject markersContainer = new GameObject("MarkersContainer");
                GameObjectUtility.SetParentAndAlign(markersContainer, gameObject);
                RectTransform markersContainerRT = markersContainer.AddComponent<RectTransform>();
                markersContainerRT.anchorMin = new Vector2(0, 0);
                markersContainerRT.anchorMax = new Vector2(1, 1);
                markersContainerRT.offsetMin = new Vector2(0, 0);
                markersContainerRT.offsetMax = new Vector2(0, 0);
            }
            FindChild("AxesAndGridContainer").transform.SetSiblingIndex(0);
            FindChild("FillContainer").transform.SetSiblingIndex(1);
            FindChild("DataContainer").transform.SetSiblingIndex(2);
            FindChild("LabelsContainer").transform.SetSiblingIndex(3);
            FindChild("MarkersContainer").transform.SetSiblingIndex(4);
            SetupChartResolution();
        }
#endif
    }
}

