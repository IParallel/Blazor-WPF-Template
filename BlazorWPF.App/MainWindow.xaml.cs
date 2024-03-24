using System;
using System.Diagnostics;
using System.IO;
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
    private BlazorWebView webView;
    public MainWindow()
    {
        InitializeComponent();
        this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight - 9;

        webView = new BlazorWebView
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
        WebViewContainer.Child = webView;

        AppDomain.CurrentDomain.UnhandledException += (sender, error) =>
        {
            MessageBox.Show(error.ExceptionObject.ToString(), "Unhandled Exception", MessageBoxButton.OK,
                MessageBoxImage.Error);
        };
        Application.Current.Exit += Current_Exit;
    }

	private void Current_Exit(object sender, ExitEventArgs e)
	{
		try
		{
			string? webViewCacheDir = webView.WebView.CoreWebView2.Environment.UserDataFolder;
			var webViewProcessId = Convert.ToInt32(webView.WebView.CoreWebView2.BrowserProcessId);
			var webViewProcess = Process.GetProcessById(webViewProcessId);

			webView.WebView.Dispose();
			webViewProcess.WaitForExit(3000);

			Directory.Delete(webViewCacheDir, true);
		}
		catch (Exception ex)
		{
			// log warning
		}
	}

	private void StackPanel_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        DragMove();
    }

    private void btn_Minimize_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void btn_Maximize_Click(object sender, RoutedEventArgs e)
    {
        if (WindowState == WindowState.Maximized)
            WindowState = WindowState.Normal;
        else WindowState = WindowState.Maximized;
    }

    private void btn_Close_Click(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }
}