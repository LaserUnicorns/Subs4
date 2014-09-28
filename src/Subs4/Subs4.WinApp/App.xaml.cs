using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using Subs4.WinApp.Init;
using Subs4.WinApp.Report;

namespace Subs4.WinApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            new Bootstrapper().Run();

            ServiceLocator.Current.GetInstance<RegionManager>().RegisterViewWithRegion(RegionNames.MainRegion, typeof (ReportView));
        }
    }
}
