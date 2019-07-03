using System;
using UnityEngine;
using System.Collections;
using Devdog.General.UI;
using UnityEditor;

namespace Devdog.General.Editors
{
    [CustomEditor(typeof(UIWindow), true)]
    [CanEditMultipleObjects]
    public class UIWindowEditor : Editor
    {
        private Type[] _inputHandlers = new Type[0];

        protected void OnEnable()
        {
            _inputHandlers = ReflectionUtility.GetAllTypesThatImplement(typeof (IUIWindowInputHandler));
        }

        public override void OnInspectorGUI()
        {
            var t = (UIWindow)target;
            base.OnInspectorGUI();

            if (t.isVisible)
            {
                EditorGUILayout.LabelField("Window is Visible");
            }
            else
            {
                EditorGUILayout.LabelField("Window is Hidden");
            }

            if (t.gameObject.GetComponent<IUIWindowInputHandler>() == null)
            {
                EditorGUILayout.HelpBox("No input handler found", MessageType.Warning);
                foreach (var type in _inputHandlers)
                {
                    var tempType = type;
                    if (GUILayout.Button("Add: " + tempType.Name))
                    {
                        t.gameObject.AddComponent(tempType);
                    }
                }
            }
        }
    }
}