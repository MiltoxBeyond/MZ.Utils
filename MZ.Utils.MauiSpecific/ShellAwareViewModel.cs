using System.Buffers.Text;
using Microsoft.Maui.Controls;
using System.Reflection;
using System.Text.Json;
using System.Text;

namespace MZ.Utils.MauiSpecific
{
    public class ShellAwareViewModel : BaseVM.BaseViewModel,
                                       BaseVM.Interfaces.IInitializeAsync,
                                       BaseVM.Interfaces.INavigationAware,
                                       BaseVM.Interfaces.IResetOnNavigation,
                                       IQueryAttributable
    {
        private static Dictionary<Type, Dictionary<string, ShellAwareReference>?> _TypeLookup = new Dictionary<Type, Dictionary<string, ShellAwareReference>?>();
        private class ShellAwareReference
        {
            public PropertyInfo targetInfo { get; set; }
            public MethodInfo conversionMethod { get; set; }
        }

        public bool HasQuery { get => GetValue<bool>(); set => SetValue(value); }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            HasQuery = query.Count > 0;
            if (!HasQuery)
            {
                return;
            }

            var type = GetType();

            var lookup = !_TypeLookup.ContainsKey(type) ? _buildList(type) : _TypeLookup[type];

            if (lookup != null)
            {
                foreach (var pair in query)
                {
                    //Check if key exists in query parameters
                    if (lookup.ContainsKey(pair.Key))
                    {
                        //Store
                        var value = query[pair.Key] is string str ? Uri.UnescapeDataString(str) : query[pair.Key].ToString();
                        //Save time and CPU effort
                        if (!string.IsNullOrWhiteSpace(value))
                        {
                            //Shorten following lines
                            var current = lookup[pair.Key];

                            var converted = current.conversionMethod.Invoke(null, new object[] { value });
                            //Set target value with deserialized value. Try not to make it too complex.
                            current.targetInfo.SetValue(this, converted);
                        }
                    }
                }
            }
        }

        public async Task<bool> GoToAsync(string path, IDictionary<string, object?> customData = null)
        {
            var finalPath = string.Empty;
            try
            {
                //Process data
                if (customData == null)
                {
                    customData = new Dictionary<string, object?>();
                }

                var properties = GetType().GetProperties().Where(i => i.CustomAttributes.Any(c => c.AttributeType == typeof(NavigationParameterAttribute)));

                foreach (var prop in properties)
                {
                    var attrib = prop.GetCustomAttribute<NavigationParameterAttribute>();
                    customData[attrib!.QueryId ?? prop.Name] = prop.GetValue(this)!;
                }

                if (customData.Count > 0)
                {
                    var builder = new StringBuilder(path);
                    builder.Append("?");
                    var x = 0;
                    foreach (var itemPair in customData)
                    {
                        if (x > 0)
                        {
                            builder.Append("&");
                        }
                        builder.Append(itemPair.Key);
                        builder.Append("=");
                        builder.Append(Serialize(itemPair.Value));
                        x++;
                    }
                    finalPath = builder.ToString();
                }
                else finalPath = path;
                await Shell.Current.GoToAsync(finalPath);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private Dictionary<string, ShellAwareReference>? _buildList(Type type)
        {
            var attribs = type.GetProperties().Where(prop => prop.CustomAttributes.Any(c => typeof(QueryParameterAttribute).IsAssignableFrom(c.AttributeType)));
            var names = attribs.ToDictionary(a => a.Name, a => a.GetCustomAttribute<QueryParameterAttribute>()?.QueryId);

            var properties = type.GetProperties().Where(p => names.Keys.Contains(p.Name));
            var result = new Dictionary<string, ShellAwareReference>();
            if (properties != null && properties.Count() > 0)
            {

                foreach (var prop in properties)
                {
                    var reference = new ShellAwareReference
                    {
                        targetInfo = prop,
                        conversionMethod = ConvertReference.MakeGenericMethod(prop.PropertyType)
                    };
                    //Add queryId => reference
                    result.Add(names[prop.Name]!, reference);
                }
            }
            else result = null;
            _TypeLookup.Add(type, result);

            return result;
        }

        #region //Static Conversion Methods
        private static MethodInfo ConvertReference;
        static ShellAwareViewModel()
        {
            ConvertReference = typeof(ShellAwareViewModel).GetMethod(nameof(Deserialize), BindingFlags.Static | BindingFlags.NonPublic)!;
        }

        private static string Serialize<T>(T input)
        {
            var result = string.Empty;
            if (input is string strInput)
            {
                result = JsonSerializer.Serialize(Uri.EscapeDataString(strInput));
                result = result.Trim().Trim('\'', '"');
            }
            else
            {
                result = JsonSerializer.Serialize(input);
                result = Uri.EscapeDataString(result);
            }

            return result;
        }

        private static T? Deserialize<T>(string value)
        {
            try
            {
                if (typeof(T) == typeof(string))
                {
                    value = $"\"{value}\"";
                }
                else
                {
                    value = Uri.UnescapeDataString(value);
                }

                return JsonSerializer.Deserialize<T>(value)!;
            }
            catch
#if (DEBUG)
            (Exception ex)
            {
                Console.WriteLine(ex);
                return default;
            }
#else
            {
                return default;
            }
#endif

        }
        #endregion

        public ShellAwareViewModel()
        {

        }
        public virtual Task InitializeAsync() => Task.CompletedTask;
        public virtual void OnNavigatedFrom() { }
        public virtual void OnNavigatedTo() { }
        public virtual void Reset() { }
    }
}