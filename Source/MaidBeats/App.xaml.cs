﻿using System.Windows;

using MaidBeats.Models;
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
            // Nothing to do in currently
            containerRegistry.RegisterSingleton<BeatSaber>();
        }

        protected override void InitializeShell(Window shell)
        {
            var beatSaber = Container.Resolve<BeatSaber>();
            beatSaber.TryToDetectInstallationPath();

            while (string.IsNullOrWhiteSpace(beatSaber.InstallationPath))
                beatSaber.SelectInstallationPathByUser();

            base.InitializeShell(shell);
        }

        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }
    }
}