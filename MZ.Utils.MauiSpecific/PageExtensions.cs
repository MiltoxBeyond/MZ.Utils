using MZ.Utils.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MZ.Utils.MauiSpecific
{
    public static class PageExtensions
    {
        public static void SetViewModel<T>(this Page page) where T : BaseViewModel
        {
            page.BindingContext = GetService<T>();
        }

        public static T? GetViewModel<T>(this Page page) where T : BaseViewModel
        {
            return page.BindingContext as T;
        }
        public static TService? GetService<TService>()
                => Current.GetService<TService>();

        public static IServiceProvider Current
                =>
        #if WINDOWS10_0_17763_0_OR_GREATER
			        MauiWinUIApplication.Current.Services;
        #elif ANDROID
                    MauiApplication.Current.Services;
        #elif IOS || MACCATALYST
			        MauiUIApplicationDelegate.Current.Services;
        #else
                            null;
        #endif
    }
}
