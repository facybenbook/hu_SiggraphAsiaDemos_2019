﻿using System;
using System.Reflection;
using Devdog.General.Editors.ReflectionDrawers;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Devdog.General.Localization.Editors
{
    [CustomDrawer(typeof(LocalizedSprite))]
    public class LocalizedSpriteDrawer : LocalizedObjectDrawerBase<Sprite, LocalizedSprite>
    {
        public LocalizedSpriteDrawer(FieldInfo fieldInfo, object value, object parentValue, int arrayIndex)
            : base(fieldInfo, value, parentValue, arrayIndex)
        {
        }

        public override DrawerBase FindDrawerRelative(string fieldName)
        {
            return null;
        }
    }
}
