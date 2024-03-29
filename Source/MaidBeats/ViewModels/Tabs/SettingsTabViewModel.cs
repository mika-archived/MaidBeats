﻿using System.Threading.Tasks;
using System.Windows.Input;

using MaidBeats.Extensions;
using MaidBeats.Models;
using MaidBeats.Models.BeatMods;

using Prism.Commands;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace MaidBeats.ViewModels.Tabs
{
    public class SettingsTabViewModel : TabBaseViewModel
    {
        private readonly BeatSaber _beatSaber;
        private readonly BeatModsClient _client;
        public ReactiveProperty<string> InstallationPath { get; }
        public ReadOnlyReactiveProperty<string> CustomLevelsPath { get; }
        public ReactiveProperty<string> GameVersion { get; }
        public ReadOnlyReactiveCollection<string> GameVersions { get; }

        public SettingsTabViewModel(BeatSaber beatSaber, BeatModsClient client) : base("Settings")
        {
            _beatSaber = beatSaber;
            _client = client;

            InstallationPath = beatSaber.ObserveProperty(w => w.InstallationPath).ToReactiveProperty().AddTo(this);
            CustomLevelsPath = beatSaber.ObserveProperty(w => w.CustomLevelsPath).ToReadOnlyReactiveProperty().AddTo(this);
            GameVersion = beatSaber.ToReactivePropertyAsSynchronized(w => w.GameVersion, ignoreValidationErrorValue: true)
                                   .SetValidateNotifyError(w => string.IsNullOrWhiteSpace(w) ? "Error" : null)
                                   .AddTo(this);
            GameVersions = client.GameVersions.ToReadOnlyReactiveCollection();
        }

        public override async Task InitializeAsync()
        {
            IsLoading = true;

            await _client.FetchGameVersionsAsync();
            GameVersion.Value = _beatSaber.GameVersion;

            IsLoading = false;
        }

        #region ChooseFolderCommand

        private ICommand _chooseFolderCommand;
        public ICommand ChooseFolderCommand => _chooseFolderCommand ??= new DelegateCommand(ChooseFolder);

        private void ChooseFolder()
        {
            _beatSaber.SelectInstallationPathByUser();
            _beatSaber.TryToDetectGameVersion();
        }

        #endregion

        #region IsLoading

        private bool _isLoading;

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (_isLoading != value)
                    SetProperty(ref _isLoading, value);
            }
        }

        #endregion
    }
}