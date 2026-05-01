#if WINDOWS
using System.Drawing;
using System.Windows.Forms;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using WinRT.Interop;

namespace Monitor_Link_Rede.Platforms.Windows;

public sealed class WindowsTrayService : IDisposable
{
    private NotifyIcon? _notifyIcon;
    private AppWindow? _appWindow;
    private bool _allowClose;
    private bool _isInitialized;

    public void Initialize(Microsoft.UI.Xaml.Window window)
    {
        if (_isInitialized)
        {
            return;
        }

        _isInitialized = true;

        var hwnd = WindowNative.GetWindowHandle(window);
        var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hwnd);
        _appWindow = AppWindow.GetFromWindowId(windowId);

        _appWindow.Closing += OnAppWindowClosing;
        _appWindow.Changed += OnAppWindowChanged;

        var menu = new ContextMenuStrip();
        menu.Items.Add("Abrir aplicativo", null, (_, _) => ShowWindow());
        menu.Items.Add("Sair", null, (_, _) => ExitApplication());

        _notifyIcon = new NotifyIcon
        {
            Icon = SystemIcons.Application,
            Text = "Monitor_Link_Rede",
            Visible = true,
            ContextMenuStrip = menu
        };

        _notifyIcon.MouseClick += NotifyIconOnMouseClick;
        _notifyIcon.DoubleClick += (_, _) => ShowWindow();

        _notifyIcon.ShowBalloonTip(
            3500,
            "Monitor em segundo plano",
            "O monitoramento continua ativo. Clique no icone para abrir o app.",
            ToolTipIcon.Info);
    }

    private void NotifyIconOnMouseClick(object? sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            ShowWindow();
        }
    }

    private void OnAppWindowChanged(AppWindow sender, AppWindowChangedEventArgs args)
    {
        if (sender.Presenter is OverlappedPresenter presenter && presenter.State == OverlappedPresenterState.Minimized)
        {
            sender.Hide();
        }
    }

    private void OnAppWindowClosing(AppWindow sender, AppWindowClosingEventArgs args)
    {
        if (_allowClose)
        {
            return;
        }

        args.Cancel = true;
        sender.Hide();
    }

    private void ShowWindow()
    {
        if (_appWindow is null)
        {
            return;
        }

        _appWindow.Show();
        if (_appWindow.Presenter is OverlappedPresenter presenter)
        {
            presenter.Restore();
        }
    }

    private void ExitApplication()
    {
        _allowClose = true;

        if (_notifyIcon is not null)
        {
            _notifyIcon.Visible = false;
        }

        Microsoft.Maui.Controls.Application.Current?.Quit();
    }

    public void Dispose()
    {
        if (_notifyIcon is not null)
        {
            _notifyIcon.Visible = false;
            _notifyIcon.Dispose();
            _notifyIcon = null;
        }

        if (_appWindow is not null)
        {
            _appWindow.Closing -= OnAppWindowClosing;
            _appWindow.Changed -= OnAppWindowChanged;
            _appWindow = null;
        }
    }
}
#endif

