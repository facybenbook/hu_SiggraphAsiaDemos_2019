﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Devdog.General
{
    /// <summary>
    /// When used this field will not be displayed in the properties sidebar of the node editor.
    /// </summary>
    public class HideInPropertiesAttribute : PropertyAttribute
    {

        public HideInPropertiesAttribute()
        {
            
        }
    }
}
