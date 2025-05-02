using Microsoft.Extensions.DependencyInjection;
using ODEliteTracker.Database;
using ODEliteTracker.Services;
using ODEliteTracker.Stores;
using ODEliteTracker.Themes;
using ODEliteTracker.ViewModels;
using ODEliteTracker.Views;
using ODJournalDatabase.Database.Interfaces;
using ODJournalDatabase.JournalManagement;
using ODMVVM.Navigation;
using System.Net.Http.Headers;
using System.Net.Http;

namespace ODEliteTracker.Extensions
{
    public static class HostBuilderExtensions
    {
        public static void AddDatabase(this IServiceCollection services, string connectionString)
        {
            services.AddSingleton<IODDatabaseProvider, ODEliteTrackerDatabaseProvider>();
            services.AddSingleton(new ODEliteTrackerDbContextFactory(connectionString));
        }

        public static void AddWindows(this IServiceCollection services)
        {
            services.AddSingleton(provider => new MainWindow(provider.GetRequiredService<IODNavigationService>())
            {
                DataContext = provider.GetRequiredService<MainViewModel>()
            });
            services.AddTransient(provider => new LoaderWindow()
            {
                DataContext = provider.GetRequiredService<LoaderViewModel>()
            });
        }

        public static void AddViewModels(this IServiceCollection services)
        {
            services.AddSingleton<MainViewModel>();

            services.AddTransient<ColonisationViewModel>();
            services.AddTransient<MassacreMissionsViewModel>();
            services.AddTransient<TradeMissionViewModel>();
            services.AddTransient<SettingsViewModel>();
            services.AddTransient<LoadingViewModel>();
            services.AddTransient<BGSViewModel>();
            services.AddTransient<PowerPlayViewModel>();
            services.AddTransient<LoaderViewModel>();
        }

        public static void AddServices(this IServiceCollection services)
        {
            services.AddSingleton<ThemeManager>();
            services.AddSingleton<JournalEventParser>();
            services.AddSingleton<IManageJournalEvents, JournalManager>();
        }

        public static void AddStores(this IServiceCollection services)
        {
            services.AddSingleton<SettingsStore>();
            services.AddSingleton<ColonisationStore>();
            services.AddSingleton<SharedDataStore>();
            services.AddSingleton<MassacreMissionStore>();
            services.AddSingleton<TradeMissionStore>();
            services.AddSingleton<BGSDataStore>();
            services.AddSingleton<PowerPlayDataStore>();
            services.AddSingleton<TickDataStore>();
        }

        public static void AddHttpClients(this IServiceCollection services)
        {
            services.AddHttpClient<EliteBGSApiService>((httpClient) =>
            {
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.BaseAddress = new Uri("https://elitebgs.app/api/ebgs/v5/");
                httpClient.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
            })
           .ConfigurePrimaryHttpMessageHandler(() =>
           {
               return new SocketsHttpHandler
               {
                   PooledConnectionLifetime = TimeSpan.FromSeconds(5),
                   ConnectTimeout = TimeSpan.FromSeconds(10),
               };
           });
        }
    }
}
