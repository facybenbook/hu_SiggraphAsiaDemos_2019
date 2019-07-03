using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.CodeDom.Compiler;
using System.IO;
using UnityEngine.Assertions;

namespace Devdog.General.Editors
{
    using Object = UnityEngine.Object;

    public static class EditorUtility
    {
        public static void ErrorIfEmpty(System.Object o, string msg)
        {
            if (o == null)
            {
                EditorGUILayout.HelpBox(msg, MessageType.Error);
            }
        }

        public static void ErrorIfEmpty(Object o, string msg)
        {
            if (o == null)
            {
                EditorGUILayout.HelpBox(msg, MessageType.Error);
            }
        }

        public static void ErrorIfEmpty(bool o, string msg)
        {
            if (o)
            {
                EditorGUILayout.HelpBox(msg, MessageType.Error);
            }
        }
    }
}