using System.Collections.Generic;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using OCRApp.Presentation;
using OCRApp.Services;
using OCRApp.ViewModels;
using Uno.Extensions;
using Uno.Extensions.Hosting;
using Uno.Extensions.Navigation;

namespace OCRApp;

public class App : Application
{
    internal static Window? MainWindow { get; private set; }
    public static IHost? Host { get; private set; }

    protected async override void OnLaunched(LaunchActivatedEventArgs args)
    {
        var builder = this.CreateBuilder(args)

            // Add navigation support for toolkit controls such as TabBar and NavigationView
            .UseToolkitNavigation()
            .Configure(host => host
#if DEBUG
            // Switch to Development environment when running in DEBUG
            .UseEnvironment(Environments.Development)
#endif
                .UseLogging(configure: (context, logBuilder) =>
                {
                    // Configure log levels for different categories of logging
                    logBuilder.SetMinimumLevel(
                        context.HostingEnvironment.IsDevelopment() ?
                            LogLevel.Information :
                            LogLevel.Warning);
                }, enableUnoLogging: true)
                .ConfigureServices((context, services) =>
                {
                    // TODO: Register your services
                    services.AddSingleton<IOCRService, OCRService>();
                    services.AddSingleton<IMessenger, WeakReferenceMessenger>();
                })
                .UseNavigation(RegisterRoutes, configure: config => config with { AddressBarUpdateEnabled = false })
            );
        MainWindow = builder.Window;

        Host = await builder.NavigateAsync<Shell>();
    }

    private static void RegisterRoutes(IViewRegistry views, IRouteRegistry routes)
    {
        views.Register(
            new ViewMap(ViewModel: typeof(ShellViewModel)),
            new ViewMap<MainPage, MainViewModel>(),
            new ViewMap<HomePage, HomeViewModel>(),
            new ViewMap<AccountPage, AccountViewModel>(),
            new ViewMap<ResultsPage, ResultsViewModel>(Data: new DataMap<IEnumerable<string>>()),
            new ViewMap<HistoryPage, HistoryViewModel>()
        );

        routes.Register(
            new RouteMap("", View: views.FindByViewModel<ShellViewModel>(),
                Nested: new RouteMap[]
                {
                    new RouteMap("Main", View: views.FindByViewModel<MainViewModel>(),
                        Nested: new[]
                        {
                            new RouteMap("Home", View: views.FindByViewModel<HomeViewModel>(), IsDefault: true),
                            new RouteMap("Account", View: views.FindByViewModel<AccountViewModel>()),
                        }),
                    new RouteMap("Results", View: views.FindByViewModel<ResultsViewModel>()),
                    new RouteMap("History", View: views.FindByViewModel<HistoryViewModel>()),
                }
            )
        );
    }
}