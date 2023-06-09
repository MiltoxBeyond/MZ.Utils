using MZ.Utils.MauiSpecific.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MZ.Utils.MauiSpecific
{
    public static class ShellExtensions
    {
        private static readonly Dictionary<Shell, Page?> State = new Dictionary<Shell, Page?>();

        public static T RegisterRouting<T>(this T shell) where T : Shell
        {
            shell.Navigating += (shell, ev) => Shell_Navigating((shell as T)!, ev);
            shell.Navigated += (shell, ev) => Shell_Navigated((shell as T)!, ev);

            return shell;
        }

        private static void Shell_Navigated<T>(T shell, ShellNavigatedEventArgs e) where T : Shell
        {
            if (State[shell] != null && State[shell] is INavigationAware aware)
            {
                //Navigated from
                aware.OnNavigatedFrom();

                State[shell] = null;
            }

            if(shell.CurrentPage is INavigationAware curNav)
            {
                curNav.OnNavigatedTo();
            }
        }

        private static void Shell_Navigating<T>(T shell, ShellNavigatingEventArgs e) where T : Shell
        {
            State[shell] = shell.CurrentPage;
        }
    }
}
