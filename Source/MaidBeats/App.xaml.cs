using System.Threading;
using System.Windows;

using MaidBeats.Models;
using MaidBeats.Models.BeatMods;
using MaidBeats.Models.Platform;
using MaidBeats.Views;

using MetroRadiance.UI;

using Prism.Ioc;
using Prism.Unity;

namespace MaidBeats
{
    /// <summary>
    ///     App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : PrismApplication
    {
        private readonly Mutex _mutex = new Mutex(false, "mochizuki.dev/maidbeats");

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            if (!_mutex.WaitOne(0, false))
            {
                _mutex.Close();
                Current.Shutdown();
                return;
            }

            ShutdownMode = ShutdownMode.OnMainWindowClose;
            ThemeService.Current.Register(this, Theme.Windows, Accent.Windows);
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<IPlatform, Oculus>(); // TODO: support steam
            containerRegistry.RegisterSingleton<CompatTable>();
            containerRegistry.RegisterSingleton<StatusService>();
            containerRegistry.RegisterSingleton<BeatModsClient>();
            containerRegistry.RegisterSingleton<BeatSaber>();
        }

        protected override void InitializeShell(Window shell)
        {
            var beatSaber = Container.Resolve<BeatSaber>();
            beatSaber.TryToDetectInstallationPath();

            while (string.IsNullOrWhiteSpace(beatSaber.InstallationPath))
                beatSaber.SelectInstallationPathByUser();
            beatSaber.TryToDetectGameVersion(); // if cannot detect game version, MaidBeats does not fetch installable mods / search installed mods.

            base.InitializeShell(shell);
        }

        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (_mutex != null)
            {
                _mutex.ReleaseMutex();
                _mutex.Close();
            }

            base.OnExit(e);
        }
    }
}