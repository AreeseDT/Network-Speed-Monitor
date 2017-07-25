using System.Windows;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace NetworkSpeedMonitor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var database = new Database();
            database.Database.Migrate();

            MainWindow = new NetworkSpeed(database);
            MainWindow.Show();
        }
    }
}
