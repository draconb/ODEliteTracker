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
using ODEliteTracker.Notifications.Themes;
using ODCapi.Services;

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
            services.AddTransient<NotificationSettingsViewModel>();
            services.AddTransient<FleetCarrierViewModel>();
        }

        public static void AddServices(this IServiceCollection services, string basePath)
        {
            services.AddSingleton<ThemeManager>();
            services.AddSingleton<NotificationThemeManager>();
            services.AddSingleton<JournalEventParser>();
            services.AddSingleton<NotificationService>();
            services.AddSingleton<IManageJournalEvents, JournalManager>();
            services.AddSingleton(provider => new CAPIService(provider.GetRequiredService<OAuthService>(), basePath));
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
            services.AddSingleton<FleetCarrierDataStore>();
        }

        public static void AddHttpClients(this IServiceCollection services, string appName)
        {
            services.AddHttpClient<EliteBGSApiService>((httpClient) =>
            {
                httpClient.BaseAddress = new Uri("https://elitebgs.app/api/ebgs/v5/");
                httpClient.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue
                {
                    NoCache = true,
                };
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                return new SocketsHttpHandler
                {
                    PooledConnectionLifetime = TimeSpan.FromSeconds(5),
                    ConnectTimeout = TimeSpan.FromSeconds(10),
                };
            });
            
            services.AddHttpClient("OAuthService", ctx =>
            {
                ctx.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue
                {
                    NoCache = true,
                };
                ctx.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                return new SocketsHttpHandler
                {
                    PooledConnectionLifetime = TimeSpan.FromSeconds(5),
                    ConnectTimeout = TimeSpan.FromSeconds(10),
                };
            });

            services.AddSingleton(client =>
            {
                var clientFactory = client.GetRequiredService<IHttpClientFactory>();
                var httpClient = clientFactory.CreateClient("OAuthService");
                return new OAuthService(httpClient, appName);
            });
        }
    }
}
