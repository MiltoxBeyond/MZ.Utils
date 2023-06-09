using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static MZ.Utils.ViewModel.Internal.BindCache;
using System.Windows.Input;
using MZ.Utils.ViewModel.Attributes;

namespace MZ.Utils.ViewModel.Internal
{
    internal class BindCache : Dictionary<Type, Dictionary<string, BindSet>>
    {
        internal class BindSet
        {
            public BindCommandAttribute? bindAttr { get; set; }
            public PropertyInfo? propertyInfo { get; set; }
            public MethodInfo? methodInfo { get; set; }
        }

        private static readonly Type ICommandType = typeof(ICommand);
        public Dictionary<string, BindSet> BuildCache(Type type)
        {
            if (ContainsKey(type))
            {
                return this[type];
            }

            var properties = type.GetProperties().Where(pi => ICommandType.IsAssignableFrom(pi.PropertyType) && pi.GetCustomAttribute<BindCommandAttribute>() != null);
            var methodNames = properties.Select(pi => pi.GetCustomAttribute<BindCommandAttribute>()!.MethodName);
            var methods = type.GetMethods()
                              .Where(method => methodNames.Contains(method.Name))
                              .ToDictionary(
                                method => method.Name,
                                method =>
                                {
                                    var set = new BindSet
                                    {
                                        propertyInfo = properties.First(pi => pi.GetCustomAttribute<BindCommandAttribute>()!.MethodName == method.Name),
                                        methodInfo = method
                                    };
                                    set.bindAttr = set.propertyInfo.GetCustomAttribute<BindCommandAttribute>();

                                    return set;
                                }


                              );

            Add(type, methods);

            return methods;
        }
    }
}
