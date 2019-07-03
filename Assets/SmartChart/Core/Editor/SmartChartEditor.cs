/*
 * Part of: SmartChart
 * Custom inspector for SmartChart. 
 */

using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

namespace ToucanSystems
{
    [CustomEditor(typeof(SmartChart))]
    [CanEditMultipleObjects]
    public class SmartChartEditor : Editor
    {
        private bool sizingOptions = false;
        private bool axesOptions = false;
        private bool gridOptions = false;
        private bool labelsOptions = false;
        private bool markerOptions = false;
        private bool useCustomHandles;
        Color cyan;
        Color green;
        Color orange;
        Color red;
        Color violet;
        GUIStyle foldoutStyle;
        SmartChart chart;

        [MenuItem("GameObject/UI/SmartChart")]
        public static void CreateChart(MenuCommand menuCommand)
        {
            GameObject parent = menuCommand.context as GameObject;
            if (parent == null || parent.GetComponentInParent<Canvas>() == null)
            {
                GameObject canvas = new GameObject("Canvas", typeof(Canvas));
                canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.AddComponent<CanvasScaler>();
                canvas.AddComponent<GraphicRaycaster>();
                parent = canvas;
                //return;
            }
            GameObject chart = new GameObject("SmartChart");
            Undo.RegisterCreatedObjectUndo(chart, "create chart");
            Undo.SetTransformParent(chart.transform, parent.transform, "parent " + chart.name);
            GameObjectUtility.SetParentAndAlign(chart, parent);
            
            chart.AddComponent<SmartChart>();
        }

        public void Awake()
        {
            useCustomHandles = true;
        }

        public override void OnInspectorGUI()
        {
            SetupStyles();
            serializedObject.Update();
            chart = (SmartChart)target;
            EditorGUI.BeginChangeCheck();

            GUILayout.BeginVertical("box");            
            EditorGUI.indentLevel++;
            ChartDataSection();
            SizingOptionsSection();
            AxesOptionsSection();
            GridOptionsSection();
            LabelsOptionsSection();
            MarkersOptionsSection();
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
            GUI.backgroundColor = Color.white;
            SerializedProperty uch = serializedObject.FindProperty("useCustomHandles");
            EditorGUILayout.PropertyField(uch, new GUIContent("Use Custom Handles (experimental)"));
            useCustomHandles = uch.boolValue;
            GUILayout.EndVertical();


            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();

                int dataCount = 0;
                for (int i = 0; i < chart.chartData.Length; i++)
                {
                    dataCount += chart.chartData[i].data.Length;
                }

                chart.DeleteMarkers(dataCount);
                chart.SetupValues(true);
                chart.UpdateChart();
            }

        }

        private void ChartDataSection()
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.Space();
            SerializedProperty chartData = serializedObject.FindProperty("chartData");
            EditorGUILayout.PropertyField(chartData, true);

            SerializedProperty smoothData = serializedObject.FindProperty("smoothData");
            EditorGUILayout.PropertyField(smoothData, true);
            if (smoothData.boolValue)
            {
                SerializedProperty chartDataSmoothing = serializedObject.FindProperty("dataSmoothing");
                EditorGUILayout.PropertyField(chartDataSmoothing, true);
            }
            SerializedProperty fill = serializedObject.FindProperty("fillAreaUnderLine");
            EditorGUILayout.PropertyField(fill, true);
            SerializedProperty showLineCaps = serializedObject.FindProperty("showLineCaps");
            EditorGUILayout.PropertyField(showLineCaps, new GUIContent("Rounded line ends"), true);
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
        }

        private void SizingOptionsSection()
        {
            GUI.backgroundColor = orange;
            GUI.color = orange;
            sizingOptions = EditorGUI.Foldout(EditorGUILayout.GetControlRect(), sizingOptions, "Sizing Options", true, foldoutStyle);
            GUI.color = Color.white;

            if (sizingOptions)
            {
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.Space();
                SerializedProperty xMargin = serializedObject.FindProperty("xMargin");
                EditorGUILayout.PropertyField(xMargin, true);
                SerializedProperty yMargin = serializedObject.FindProperty("yMargin");
                EditorGUILayout.PropertyField(yMargin, true);
                EditorGUILayout.Space();
                EditorGUILayout.EndVertical();
            }
        }

        private void AxesOptionsSection()
        {
            GUI.backgroundColor = green;
            GUI.color = green;
            axesOptions = EditorGUI.Foldout(EditorGUILayout.GetControlRect(), axesOptions, "Axes Options", true, foldoutStyle);
            GUI.color = Color.white;

            if (axesOptions)
            {
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.Space();
                SerializedProperty axesWidth = serializedObject.FindProperty("axesWidth");
                EditorGUILayout.PropertyField(axesWidth, true);
                GUI.backgroundColor = Color.white;
                SerializedProperty axesColor = serializedObject.FindProperty("axesColor");
                EditorGUILayout.PropertyField(axesColor, true);
                GUI.backgroundColor = green;
                SerializedProperty invertXAxis = serializedObject.FindProperty("invertXAxis");
                EditorGUILayout.PropertyField(invertXAxis, true);
                SerializedProperty invertYAxis = serializedObject.FindProperty("invertYAxis");
                EditorGUILayout.PropertyField(invertYAxis, true);
                SerializedProperty stickXAxisToZero = serializedObject.FindProperty("stickXAxisToZero");
                EditorGUILayout.PropertyField(stickXAxisToZero, true);
                SerializedProperty stickYAxisToZero = serializedObject.FindProperty("stickYAxisToZero");
                EditorGUILayout.PropertyField(stickYAxisToZero, true);
                EditorGUILayout.Space();
                EditorGUILayout.EndVertical();
            }
        }

        private void GridOptionsSection()
        {
            GUI.backgroundColor = cyan;
            GUI.color = cyan;
            gridOptions = EditorGUI.Foldout(EditorGUILayout.GetControlRect(), gridOptions, "Grid Options", true, foldoutStyle);
            GUI.color = Color.white;

            if (gridOptions)
            {
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.Space();
                SerializedProperty showXGrid = serializedObject.FindProperty("showXGrid");
                EditorGUILayout.PropertyField(showXGrid, true);
                SerializedProperty showYGrid = serializedObject.FindProperty("showYGrid");
                EditorGUILayout.PropertyField(showYGrid, true);

                GUI.backgroundColor = Color.white;
                SerializedProperty gridColor = serializedObject.FindProperty("gridColor");
                EditorGUILayout.PropertyField(gridColor, true);
                GUI.backgroundColor = cyan;
                SerializedProperty gridLineWidth = serializedObject.FindProperty("gridLineWidth");
                EditorGUILayout.PropertyField(gridLineWidth, true);
                SerializedProperty xGridStep = serializedObject.FindProperty("xGridStep");
                EditorGUILayout.PropertyField(xGridStep, true);
                SerializedProperty yGridStep = serializedObject.FindProperty("yGridStep");
                EditorGUILayout.PropertyField(yGridStep, true);
                EditorGUILayout.Space();
                EditorGUILayout.EndVertical();
            }
        }

        private void LabelsOptionsSection()
        {
            GUI.backgroundColor = red;
            GUI.color = red;
            labelsOptions = EditorGUI.Foldout(EditorGUILayout.GetControlRect(), labelsOptions, "Labels Options", true, foldoutStyle);
            GUI.color = Color.white;

            if (labelsOptions)
            {
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.Space();
                SerializedProperty showLabelsOnXAxis = serializedObject.FindProperty("showLabelsOnXAxis");
                EditorGUILayout.PropertyField(showLabelsOnXAxis, true);
                SerializedProperty showLabelsOnYAxis = serializedObject.FindProperty("showLabelsOnYAxis");
                EditorGUILayout.PropertyField(showLabelsOnYAxis, true);
                SerializedProperty minXValue = serializedObject.FindProperty("minXValue");
                EditorGUILayout.PropertyField(minXValue, true);
                SerializedProperty maxXValue = serializedObject.FindProperty("maxXValue");
                EditorGUILayout.PropertyField(maxXValue, true);

                SerializedProperty minYValue = serializedObject.FindProperty("minYValue");
                EditorGUILayout.PropertyField(minYValue, true);
                SerializedProperty maxYValue = serializedObject.FindProperty("maxYValue");
                EditorGUILayout.PropertyField(maxYValue, true);

                SerializedProperty xValuesCount = serializedObject.FindProperty("xValuesCount");
                EditorGUILayout.PropertyField(xValuesCount, true);
                SerializedProperty yValuesCount = serializedObject.FindProperty("yValuesCount");
                EditorGUILayout.PropertyField(yValuesCount, true);
                SerializedProperty labelsFontSize = serializedObject.FindProperty("labelsFontSize");
                EditorGUILayout.PropertyField(labelsFontSize, true);
                SerializedProperty labelsFont = serializedObject.FindProperty("labelsFont");
                EditorGUILayout.PropertyField(labelsFont, true);

                GUI.backgroundColor = Color.white;
                SerializedProperty labelsFontColor = serializedObject.FindProperty("labelsFontColor");
                EditorGUILayout.PropertyField(labelsFontColor, true);
                GUI.backgroundColor = red;

                SerializedProperty xValuesPrecision = serializedObject.FindProperty("xValuesPrecision");
                EditorGUILayout.PropertyField(xValuesPrecision, true);
                SerializedProperty yValuesPrecision = serializedObject.FindProperty("yValuesPrecision");
                EditorGUILayout.PropertyField(yValuesPrecision, true);
                SerializedProperty xValuesDisplacement = serializedObject.FindProperty("xValuesOffset");
                EditorGUILayout.PropertyField(xValuesDisplacement, true);
                SerializedProperty yValuesDisplacement = serializedObject.FindProperty("yValuesOffset");
                EditorGUILayout.PropertyField(yValuesDisplacement, true);
                
                EditorGUILayout.Space();
                EditorGUILayout.EndVertical();
            }
        }

        private void MarkersOptionsSection()
        {
            GUI.backgroundColor = violet;
            GUI.color = violet;
            markerOptions = EditorGUI.Foldout(EditorGUILayout.GetControlRect(), markerOptions, "Markers Options", true, foldoutStyle);
            GUI.color = Color.white;

            if (markerOptions)
            {
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.Space();
                SerializedProperty showMarkers = serializedObject.FindProperty("showMarkers");
                EditorGUILayout.PropertyField(showMarkers, true);
                if (showMarkers.boolValue)
                {
                    SerializedProperty markersLabelsFontSize = serializedObject.FindProperty("markersLabelsFontSize");
                    EditorGUILayout.PropertyField(markersLabelsFontSize, true);
                    SerializedProperty markersLabelsFont = serializedObject.FindProperty("markersLabelsFont");
                    EditorGUILayout.PropertyField(markersLabelsFont, true);
                    GUI.backgroundColor = Color.white;
                    SerializedProperty markersLabelsFontColor = serializedObject.FindProperty("markersLabelsFontColor");
                    EditorGUILayout.PropertyField(markersLabelsFontColor, true);
                    GUI.backgroundColor = violet;
                    SerializedProperty markersFieldOffset = serializedObject.FindProperty("markersFieldOffset");
                    EditorGUILayout.PropertyField(markersFieldOffset, true);
                    SerializedProperty markersLabelsOffset = serializedObject.FindProperty("markersLabelsOffset");
                    EditorGUILayout.PropertyField(markersLabelsOffset, true);
                    SerializedProperty showValuePrefix = serializedObject.FindProperty("showValuePrefix");
                    EditorGUILayout.PropertyField(showValuePrefix, true);
                    if (showValuePrefix.boolValue)
                    {
                        SerializedProperty valuePrefix = serializedObject.FindProperty("valuePrefix");
                        EditorGUILayout.PropertyField(valuePrefix, true);
                    }
                    SerializedProperty markersLabelsBackground = serializedObject.FindProperty("markersLabelsBackground");
                    EditorGUILayout.PropertyField(markersLabelsBackground, true);
                    SerializedProperty markerInteractionType = serializedObject.FindProperty("markerInteractionType");
                    EditorGUILayout.PropertyField(markerInteractionType, true);
                }
                EditorGUILayout.Space();
                EditorGUILayout.EndVertical();
            }
        }

        private void SetupStyles()
        {
            float modifier = 0;
            if (!EditorGUIUtility.isProSkin)
            {
                modifier = 1f;
            }
            cyan = new Color32(146, 229, 214, 255) + new Color(modifier, modifier, modifier, 0);
            green = new Color32(163, 191, 146, 255) + new Color(modifier, modifier, modifier, 0);
            orange = new Color32(223, 179, 87, 255) + new Color(modifier, modifier, modifier, 0);
            red = new Color32(223, 87, 87, 255) + new Color(modifier, modifier, modifier, 0);
            violet = new Color32(186, 109, 171, 255) + new Color(modifier, modifier, modifier, 0);

            foldoutStyle = new GUIStyle(EditorStyles.foldout);
            foldoutStyle.fontStyle = FontStyle.Bold;
        }

        protected virtual void OnSceneGUI()
        {
            SmartChart chart = (SmartChart)target;
            if (useCustomHandles)
            {
                Undo.RecordObject(chart.GetComponent<RectTransform>(), "chart rect modification");
                RectHandles.ResizeRect(chart.GetComponent<RectTransform>(), Handles.CubeHandleCap, Color.cyan, HandleUtility.GetHandleSize(Vector3.zero) * 0.35f, 1);
            }
            chart.UpdateChart();
        }
    }
}
