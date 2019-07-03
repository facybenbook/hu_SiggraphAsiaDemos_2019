using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Devdog.General
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class ReplacedByAttribute : Attribute
    {
        public Type type { get; protected set; }

        public ReplacedByAttribute(Type type)
        {
            this.type = type;
        }
    }
}
