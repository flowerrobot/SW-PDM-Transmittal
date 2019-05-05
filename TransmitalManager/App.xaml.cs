using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
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
            CancellationTokenSource ctk = new CancellationTokenSource();
            var c = new List<Task>();

            c.Add(Recipient.LoadCompanies(ctk.Token));
            c.Add(Recipient.LoadRecipients(ctk.Token));
            c.Add(User.AllUsersAsync(ctk.Token));
            c.Add(Project.AllProjectsAsync(ctk.Token));

            Task.WaitAll(c.ToArray());

            ss.Close(TimeSpan.FromSeconds(0.5));

            MainWindow = new MainWindow();
            MainWindow.Show();
        }

    }
}
