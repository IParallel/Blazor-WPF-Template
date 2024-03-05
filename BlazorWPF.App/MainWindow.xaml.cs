using System;
using System.Windows;
using BlazorWPF.WebApp;
using Microsoft.AspNetCore.Components.WebView.Wpf;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorWPF.App;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        var webView = new BlazorWebView
        {
            HostPage = @"wwwroot\index.html"
        };
        var services = new ServiceCollection();
        services.AddWpfBlazorWebView();
#if DEBUG
        services.AddBlazorWebViewDeveloperTools();
#endif
        webView.Services = services.BuildServiceProvider();
        webView.RootComponents.Add(new RootComponent
        {
            ComponentType = typeof(Root),
            Selector = "#app"
        });
        Content = webView;

        AppDomain.CurrentDomain.UnhandledException += (sender, error) =>
        {
            MessageBox.Show(error.ExceptionObject.ToString(), "Unhandled Exception", MessageBoxButton.OK,
                MessageBoxImage.Error);
        };
    }
}