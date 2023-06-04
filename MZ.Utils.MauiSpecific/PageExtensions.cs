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
            page.BindingContext = page.Handler?.MauiContext?.Services.GetService<T>();
        }

        public static T? GetViewModel<T>(this Page page) where T : BaseViewModel
        {
            return page.BindingContext as T;
        }
    }
}
