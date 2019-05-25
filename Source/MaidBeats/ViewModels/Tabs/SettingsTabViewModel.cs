using System.Windows.Input;

using MaidBeats.Extensions;
using MaidBeats.Models;

using Prism.Commands;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace MaidBeats.ViewModels.Tabs
{
    public class SettingsTabViewModel : TabBaseViewModel
    {
        private readonly BeatSaber _beatSaber;
        public ReactiveProperty<string> InstallationPath { get; }
        public ReactiveProperty<string> GameVersion { get; }
        public ReadOnlyReactiveCollection<string> GameVersions { get; }

        public SettingsTabViewModel(BeatSaber beatSaber) : base("Settings")
        {
            _beatSaber = beatSaber;
            InstallationPath = beatSaber.ObserveProperty(w => w.InstallationPath).ToReactiveProperty().AddTo(this);
            GameVersion = beatSaber.ObserveProperty(w => w.Version).ToReactiveProperty().AddTo(this);
            GameVersions = beatSaber.GameVersions.ToReadOnlyReactiveCollection().AddTo(this);
        }

        #region ChooseFolderCommand

        private ICommand _chooseFolderCommand;
        public ICommand ChooseFolderCommand => _chooseFolderCommand ?? (_chooseFolderCommand = new DelegateCommand(ChooseFolder));

        private void ChooseFolder()
        {
            _beatSaber.SelectInstallationPathByUser();
        }

        #endregion
    }
}