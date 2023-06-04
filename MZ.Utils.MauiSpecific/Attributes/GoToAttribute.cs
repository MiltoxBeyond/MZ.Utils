using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MZ.Utils.MauiSpecific.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class GoToAttribute : Attribute
    {
        public string Route { get; }

        public GoToAttribute(string route)
        {
            Route = route;
        }
    }
}
