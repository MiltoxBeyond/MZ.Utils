using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MZ.Utils.MauiSpecific.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class NavigationParameterAttribute : QueryParameterAttribute
    {
        public NavigationParameterAttribute(string? QueryID = null) : base(QueryID) { }
    }
}
