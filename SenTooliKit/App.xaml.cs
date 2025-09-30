using System.Net;
using System.Net.Http;

using CommunityToolkit.Mvvm.Messaging;

using MaterialDesignThemes.Wpf;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using System.Windows;
using System.Windows.Threading;

using SenTooliKit.IServices.IServices;
using SenTooliKit.Manager;
using SenTooliKit.Services.Services;
using SenTooliKit.ViewModels;
using SenTooliKit.ViewModels.BilibiliPages;
using SenTooliKit.ViewModels.LoginPages;
using SenTooliKit.Views;
using SenTooliKit.Views.BilibiliPages;
using SenTooliKit.Views.LoginPages;


namespace SenTooliKit;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    [STAThread]
    private static void Main(string[] args)
    {
        MainAsync(args).GetAwaiter().GetResult();
    }


    public static IServiceProvider Service { get; private set; }

    public static WindowManager WindowManager { get; } = new();

    private static async Task MainAsync(string[] args)
    {
        using IHost host = CreateHostBuilder(args).Build();
        await host.StartAsync().ConfigureAwait(true);
        App app = new();
        app.InitializeComponent();
        Service = host.Services;
        WindowManager.ShowChromeWindow<MainWindow>();
        app.Run();

        await host.StopAsync().ConfigureAwait(true);
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostBuilderContext, configurationBuilder)
                => configurationBuilder.AddUserSecrets(typeof(App).Assembly))
            .ConfigureServices((hostContext, services) =>
            {
                //添加B站服务
                services.AddScoped<IBilibiliAuthService, BilibiliAuthService>();
                services.AddScoped<IBilibiliDmService, BilibiliDmService>();
                services.AddScoped<IBilibiliInfoService, BilibiliInfoService>();
                services.AddScoped<IBilibiliVideoService, BilibiliVideoService>();
                services.AddScoped<IBilibiliStreamService, BilibiliStreamService>();
                services.AddScoped<IBiliCommentsService, BiliCommentsService>();

                services.AddScoped<BCommentAnalysisUserControl>();
                services.AddScoped<BCommentAnalysisUserControlVM>();
                services.AddScoped<BCommentInfoUserControl>();
                services.AddScoped<BCommentInfoUserControlVM>();
                services.AddTransient<BCommentWindow>();
                services.AddTransient<BCommentWindowViewModel>();
                services.AddTransient<BStreamWindow>();
                services.AddTransient<BStreamWindowViewModel>();
                services.AddScoped<BDmAnalysisUserControl>();
                services.AddScoped<BDmAnalysisUserControlVM>();
                services.AddSingleton<BDmWindowViewModel>();
                services.AddSingleton<MainWindow>();
                services.AddSingleton<MainWindowViewModel>();
                services.AddScoped<BiliTkHomeUserControl>();
                services.AddScoped<BiliTkHomeUserControlVM>();
                services.AddSingleton<BDmInfoUserControl>();
                services.AddSingleton<BDmInfoUserControlVM>();
                services.AddSingleton<LoginUserControl>();
                services.AddSingleton<LoginUserControlViewModel>();
                
                services.AddTransient<BDmMainWindow>();
                services.AddTransient<BDmInfoUserControlVM>();
                services.AddTransient<BVideoWindow>();
                services.AddTransient<BVideoViewModel>();
                //注册WindowManager
                services.AddSingleton<WindowManager>();


                services.AddSingleton<WeakReferenceMessenger>();
                services.AddSingleton<IMessenger, WeakReferenceMessenger>(provider =>
                    provider.GetRequiredService<WeakReferenceMessenger>());


                services.AddSingleton(_ => Current.Dispatcher);

                services.AddTransient<ISnackbarMessageQueue>(provider =>
                {
                    Dispatcher dispatcher = provider.GetRequiredService<Dispatcher>();
                    return new SnackbarMessageQueue(TimeSpan.FromSeconds(3.0), dispatcher);
                });
            });
}