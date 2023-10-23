using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.Shapes;
using Avalonia.ReactiveUI;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace storage.Desktop;

internal class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .UseReactiveUI()
            .WithInterFont()
            .LogToTrace()
            .UseReactiveUI();
    }
}