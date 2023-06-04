using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MZ.Utils.ViewModel.Attributes
{
    public class BindCommandAttribute : Attribute
    {
        public string MethodName { get; }
        public bool UsesParameter { get; }
        public BindCommandAttribute(string methodName, bool usesParameter = false)
        {
            MethodName = methodName;
            UsesParameter = usesParameter;
        } 
    }
}
