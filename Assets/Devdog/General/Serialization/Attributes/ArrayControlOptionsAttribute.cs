﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Devdog.General
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ArrayControlOptionsAttribute : Attribute
    {
//        public bool includeArrayChildren { get; protected set; }
        public bool canRemoveItems = true;
        public bool canAddItems = true;

        public ArrayControlOptionsAttribute()
        {
        }
    }
}
