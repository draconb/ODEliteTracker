using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ODEliteTracker.Database;
using ODEliteTracker.Services;
using ODEliteTracker.Stores;
using ODEliteTracker.Themes;
using ODEliteTracker.ViewModels;
using ODEliteTracker.Views;
using ODJournalDatabase.Database.Interfaces;
using ODJournalDatabase.JournalManagement;
using ODMVVM.Navigation;
using ODMVVM.Services.MessageBox;
using ODMVVM.ViewModels;
using System.IO;
using System.Windows;
using System.Windows.Threading;

namespace ODEliteTracker
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        public static Version AppVersion { get; internal set; } = new Version(1, 0);

        //#if INSTALL
        //        public readonly static string BaseDirectory = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "OD Explorer");
        //#else
        public readonly static string BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
//#endif
        private const string database = "ODEliteTracker.db";
        private static readonly string connectionString = $"DataSource={Path.Combine(BaseDirectory, database)};";



        private static readonly IHost _host = Host
            .CreateDefaultBuilder()
            .ConfigureAppConfiguration(c => { c.SetBasePath(Path.GetDirectoryName(AppContext.BaseDirectory) ?? string.Empty); })
            .ConfigureServices((context, services) =>
            {
                //Windows
                services.AddSingleton(provider => new MainWindow(provider.GetRequiredService<IODNavigationService>())
                {
                    DataContext = provider.GetRequiredService<MainViewModel>()
                });
                services.AddTransient(provider => new LoaderWindow()
                {
                    DataContext = provider.GetRequiredService<LoaderViewModel>()
                });
                //Database
                services.AddSingleton<IODDatabaseProvider, ODEliteTrackerDatabaseProvider>();
                services.AddSingleton(new ODEliteTrackerDbContextFactory(connectionString));
                //Navigation
                services.AddSingleton<IODNavigationService, ODNavigationService>();
                services.AddSingleton<Func<Type, ODViewModel>>(provider => viewModelType => (ODViewModel)provider.GetRequiredService(viewModelType));
                
                //View Models
                services.AddSingleton<MainViewModel>();

                services.AddTransient<ColonisationViewModel>();
                services.AddTransient<MassacreMissionsViewModel>();
                services.AddTransient<TradeMissionViewModel>();
                services.AddTransient<SettingsViewModel>();
                services.AddTransient<LoadingViewModel>();
                services.AddTransient<BGSViewModel>();
                services.AddTransient<PowerPlayViewModel>();
                services.AddTransient<LoaderViewModel>();

                //Services
                services.AddSingleton<ThemeManager>();
                services.AddSingleton<JournalEventParser>();
                services.AddSingleton<IManageJournalEvents, JournalManager>();
                services.AddTransient<IODDialogService, ODDialogService>();

                //Store
                services.AddSingleton<SettingsStore>();
                services.AddSingleton<ColonisationStore>();
                services.AddSingleton<SharedDataStore>();
                services.AddSingleton<MassacreMissionStore>();
                services.AddSingleton<TradeMissionStore>();
                services.AddSingleton<BGSDataStore>();
                services.AddSingleton<PowerPlayDataStore>();
                services.AddSingleton<TickDataStore>();
                
            }).Build();

        /// <summary>
        /// Gets services.
        /// </summary>
        public static IServiceProvider Services
        {
            get { return _host.Services; }
        }

        /// <summary>
        /// Occurs when the application is loading.
        /// </summary>
        private async void OnStartup(object sender, StartupEventArgs e)
        {
            await _host.StartAsync();

            //Disable shutdown when the dialog closes
            ShutdownMode = ShutdownMode.OnExplicitShutdown;
            var updateWindow = _host.Services.GetRequiredService<LoaderWindow>();
            if (updateWindow.ShowDialog() is bool v && !v)
            {
                Shutdown();
                return;
            }

            var settings = Services.GetRequiredService<SettingsStore>();
            settings.LoadSettings();
            ShutdownMode = ShutdownMode.OnMainWindowClose;

            var mainWindow = Services.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        /// <summary>
        /// Occurs when the application is closing.
        /// </summary>
        private async void OnExit(object sender, ExitEventArgs e)
        {
            var settings = Services.GetRequiredService<SettingsStore>();
            settings.SaveSettings();
            await _host.StopAsync();

            _host.Dispose();
        }


        /// <summary>
        /// Occurs when an exception is thrown by an application but not handled.
        /// </summary>
        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            // For more info see https://docs.microsoft.com/en-us/dotnet/api/system.windows.application.dispatcherunhandledexception?view=windowsdesktop-6.0
        }
    }
}
