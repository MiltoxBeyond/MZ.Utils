using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MZ.Utils.MauiSpecific.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ShellRouteAttribute : Attribute
    {
        public string? Route { get; }
        public Type PageType { get; }
        public Type? ViewModelType { get; }

        public ShellRouteAttribute(string? route, Type pageType,  Type? viewModelType = null)
        {
            Route = route;
            PageType = pageType;
            ViewModelType = viewModelType;
        }

        public void RegisterRoute()
        {
            if (Route != null)
                Routing.RegisterRoute(Route, PageType);
        }

        public void RegisterView(IServiceCollection services)
        {
            if (PageType == null) return;
            services.AddTransient(PageType);
        }

        public void RegisterViewModel(IServiceCollection services, bool isSingleton = false)
        {
            if (ViewModelType == null) return;
            if (!isSingleton)
                services.AddTransient(ViewModelType);
            else 
                services.AddSingleton(ViewModelType);
        }
    }
}
