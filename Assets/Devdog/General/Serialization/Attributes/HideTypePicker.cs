using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Devdog.General
{
    [AttributeUsage(AttributeTargets.Field)]
    public class HideTypePicker : Attribute
    {

        public HideTypePicker()
        {
        }
    }
}
