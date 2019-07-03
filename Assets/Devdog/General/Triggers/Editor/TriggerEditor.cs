using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace Devdog.General.Editors
{
    [CustomEditor(typeof(Trigger), true)]
    [CanEditMultipleObjects]
    public class TriggerEditor : Editor
    {
        private SerializedProperty _window;

        private static Color _outOfRangeColor;
        private static Color _inRangeColor;
        private static Dictionary<Type, Type[]> _interfaceTypeLookup = new Dictionary<Type, Type[]>();

        public virtual void OnEnable()
        {
            _window = serializedObject.FindProperty("_window");

            _outOfRangeColor = new Color(Color.cyan.r, Color.cyan.g, Color.cyan.b, 0.2f);
            _inRangeColor = new Color(Color.green.r, Color.green.g, Color.green.b, 0.3f);
        }

        public void OnSceneGUI()
        {
            var trigger = (TriggerBase)target;
            if (trigger.rangeHandler != null && trigger.rangeHandler.Equals(null) == false)
            {
                return;
            }

            var discColor = _outOfRangeColor;
            if (Application.isPlaying && trigger.inRange)
            {
                discColor = _inRangeColor;
            }

            if (GeneralSettingsManager.instance != null && GeneralSettingsManager.instance.settings != null)
            {
                var useRange = GeneralSettingsManager.instance.settings.triggerUseDistance;

                Handles.color = discColor;
                var euler = trigger.transform.rotation.eulerAngles;
                euler.x += 90.0f;
                Handles.DrawSolidDisc(trigger.transform.position, Vector3.up, useRange);

                discColor.a += 0.2f;
                Handles.color = discColor;
                Handles.CircleCap(0, trigger.transform.position, Quaternion.Euler(euler), useRange);
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var script = serializedObject.FindProperty("m_Script");
            EditorGUILayout.PropertyField(script);

            // Draws remaining items
            DrawPropertiesExcluding(serializedObject, new string[]
            {
                "_window",
                script.name
            });

            DrawAddModules();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawAddModules()
        {
            var t = (TriggerBase) target;

            DrawWindowContainer(t);

            DrawInterfaceHandler<ITriggerInputHandler>(t, "Input", false);
            DrawInterfaceHandler<ITriggerRangeHandler>(t, "Range", true);
        }

        private static void DrawInterfaceHandler<T>(TriggerBase trigger, string triggerActionName, bool createChildObject)
        {
            if (trigger.GetComponentInChildren<T>() == null)
            {
                if (_interfaceTypeLookup.ContainsKey(typeof (T)) == false)
                {
                    _interfaceTypeLookup[typeof (T)] = ReflectionUtility.GetAllTypesThatImplement(typeof (T));
                }

                EditorGUILayout.HelpBox("You can add a " + typeof(T).Name + " to control this trigger's " + triggerActionName.ToLower(), MessageType.Info);
                foreach (var type in _interfaceTypeLookup[typeof(T)])
                {
                    var tempType = type;
                    var tempTrigger = trigger;
                    if (GUILayout.Button("Add: " + tempType.Name))
                    {
                        var objToAddTo = tempTrigger.gameObject;
                        if (createChildObject)
                        {
                            objToAddTo = new GameObject("_" + tempType.Name);
                            objToAddTo.transform.SetParent(tempTrigger.transform);
                            objToAddTo.transform.localPosition = Vector3.zero;
                            objToAddTo.transform.localRotation = Quaternion.identity;
                        }

                        objToAddTo.AddComponent(tempType);
                    }
                }
            }
            else
            {
                EditorGUILayout.HelpBox(triggerActionName + " is handled by " + typeof(T).Name, MessageType.Info);
            }
        }

        private void DrawWindowContainer(TriggerBase t)
        {
            var windowHandler = t.gameObject.GetComponent<ITriggerWindowContainer>();
            if (windowHandler == null)
            {
                EditorGUILayout.PropertyField(_window);
            }
            else
            {
                EditorGUILayout.HelpBox("Window is managed by " + windowHandler.GetType().Name, MessageType.Info);
            }
        }
    }
}