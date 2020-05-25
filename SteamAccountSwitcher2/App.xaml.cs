using Serilog;
using System;
using System.Windows;

namespace SteamAccountSwitcher2
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.RollingFile(pathFormat: "log.txt", retainedFileCountLimit: 5)
                .CreateLogger();
            Log.Information("The global logger has been configured");

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Log.Logger.Error(e.ExceptionObject as Exception , "CurrentDomain_UnhandledException");
        }
    }
}
