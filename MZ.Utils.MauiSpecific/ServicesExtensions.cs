using Microsoft.Maui.Controls;
using MZ.Utils.MauiSpecific.Attributes;
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
        internal static IDictionary<Type, ShellRouteAttribute> RouteTypes { get; } = Assembly
                                                                .GetExecutingAssembly()
                                                                .GetTypes()
                                                                .Where(type => typeof(Page).IsAssignableFrom(type) 
                                                                        && type.GetCustomAttribute<ShellRouteAttribute>() != null)
                                                                .ToDictionary(type => type, type =>  type.GetCustomAttribute<ShellRouteAttribute>()!);
        public static T RegisterRoutes<T>(this T app) where T : Application
        {
            foreach (var type in RouteTypes)
            {
                type.Value.RegisterRoute();
            }

            return app;
        }
        public static T RegisterTypes<T>(this T services) where T : IServiceCollection
        {
            foreach(var type in RouteTypes)
            {
                type.Value.RegisterViewModel(services);
            }

            return services;
        }
    }
}
