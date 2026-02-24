using Microsoft.AspNetCore.Components.WebView.WindowsForms;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OcelotUI.Desktop.Services;
using OcelotUI.UI;
using OcelotUI.UI.Services;

namespace OcelotUI.Desktop;

public class MainForm : Form
{
    public MainForm()
    {
        Text = "OcelotUI 配置編輯器";
        Width = 1400;
        Height = 900;
        StartPosition = FormStartPosition.CenterScreen;

        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true)
            .Build();

        var services = new ServiceCollection();
        services.AddWindowsFormsBlazorWebView();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddOcelotUI(configuration);
        services.AddScoped<ICultureSwitcher, DesktopCultureSwitcher>();

#if DEBUG
        services.AddBlazorWebViewDeveloperTools();
#endif

        var serviceProvider = services.BuildServiceProvider();

        var blazorWebView = new BlazorWebView
        {
            Dock = DockStyle.Fill,
            HostPage = "wwwroot/index.html",
            Services = serviceProvider,
        };

        blazorWebView.RootComponents.Add<OcelotUI.UI.Components.Routes>("#app");

        // 外部連結用系統瀏覽器開啟（僅攔截預設會被取消的外部 URL）
        blazorWebView.UrlLoading += (_, e) =>
        {
            if (e.UrlLoadingStrategy ==
                Microsoft.AspNetCore.Components.WebView.UrlLoadingStrategy.CancelLoad)
            {
                e.UrlLoadingStrategy =
                    Microsoft.AspNetCore.Components.WebView.UrlLoadingStrategy.OpenExternally;
            }
        };

        Controls.Add(blazorWebView);
    }
}
