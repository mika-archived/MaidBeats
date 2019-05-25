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
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            ShutdownMode = ShutdownMode.OnMainWindowClose;
            ThemeService.Current.Register(this, Theme.Windows, Accent.Windows);
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<IPlatform, Oculus>(); // TODO: support steam
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
    }
}