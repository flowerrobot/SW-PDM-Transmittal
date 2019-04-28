using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using TransmittalManager.Models;

namespace TransmittalManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override async void OnStartup(StartupEventArgs e)
        {
            var ss = new SplashScreen("LoadingSplashScreen");
            //  ss.Show(true,true);

            // Load any easily cached information
            var u =  User.AllUsersAsync();
            var p = Project.AllProjectsAsync();

            await u;
            await p;

            ss.Close(TimeSpan.FromSeconds(0.5));

            MainWindow = new MainWindow();
            MainWindow.Show();
        }

    }
}
