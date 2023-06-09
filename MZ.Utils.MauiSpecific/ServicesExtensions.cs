using Microsoft.Maui.Controls;
using MZ.Utils.MauiSpecific.Attributes;
using MZ.Utils.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MZ.Utils.MauiSpecific
{
    public static class ServicesExtensions
    {
        private static IDictionary<Type, ShellRouteAttribute>? _routes;
        internal static IDictionary<Type, ShellRouteAttribute> RouteTypes {
            get => _routes!;
        }

        private static void BuildRouteTypes<T>()
        {
            _routes = typeof(T).Assembly.GetTypes()
                               .Where(type => type.IsSubclassOf(PageType) 
                                               && type.GetCustomAttribute<ShellRouteAttribute>() != null)
                               .ToDictionary(type => type, type => type.GetCustomAttribute<ShellRouteAttribute>()!);
        }

        private static readonly Type PageType = typeof(Page);
        
        public static T RegisterRoutes<T>(this T appShell) where T : Shell
        {
            try
            {
                if(_routes == null)
                {
                    BuildRouteTypes<T>();
                }
                var types = RouteTypes;
                foreach (var type in types)
                {
                    type.Value.RegisterRoute();
                }
            }
            catch
            {

            }
            
            

            return appShell;
        }
        public static T RegisterTypes<T, ShellType>(this T services) where T : IServiceCollection
                                                                    where ShellType : Shell
        {
            if (_routes == null)
            {
                BuildRouteTypes<ShellType>();
            }
            foreach (var type in RouteTypes)
            {
                type.Value.RegisterView(services);
                type.Value.RegisterViewModel(services);                                   
            }

            return services;
        }

        private static readonly Dictionary<Type, Dictionary<PropertyInfo, string>> RouteCache = new Dictionary<Type, Dictionary<PropertyInfo, string>>();

        private static Dictionary<PropertyInfo, string> BuildRouteCache(Type type)
        {
            if(RouteCache.ContainsKey(type)) return RouteCache[type];

            var routeCache = new Dictionary<PropertyInfo, string>();

            var properties = type.GetProperties()
                                 .Where(prop => prop.GetCustomAttribute<GoToAttribute>() != null)
                                 .ToDictionary(prop => prop, prop => prop.GetCustomAttribute<GoToAttribute>()!.Route);

            RouteCache[type] = properties;
            return properties;
        }

        public static T BindRoutes<T>(this T viewmodel) where T : BaseViewModel
        {
            var routes = BuildRouteCache(viewmodel.GetType());

            foreach(var route in routes)
            {
                route.Key.SetValue(viewmodel, new BaseViewModelCommand(() => NavigateRoute(route.Value)));
            }

            return viewmodel;
        }

        private static async void NavigateRoute(string value)
        {
            try
            {
                if (Shell.Current.CurrentPage?.BindingContext is ShellAwareViewModel shellAware)
                {
                    await shellAware.GoToAsync(value);
                }
                else
                {
                    await Shell.Current.GoToAsync(value);
                }
            }
            catch(Exception ex)
            {
                Console.Error.WriteLine(ex);
            }
        }
    }
}
