using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MZ.Utils.MauiSpecific.Interfaces
{
    internal interface INavigationAware
    {
        void OnNavigatedFrom();
        void OnNavigatedTo();
    }
}
