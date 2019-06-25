using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

using MaidBeats.Extensions;
using MaidBeats.Models;
using MaidBeats.Models.BeatMods;
using MaidBeats.ViewModels.Partial;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace MaidBeats.ViewModels.Tabs
{
    public class ModsTabViewModel : TabBaseViewModel
    {
        private readonly BeatSaber _beatSaber;
        private readonly BeatModsClient _client;
        private readonly CompatTable _compatTable;

        public ReadOnlyReactiveProperty<string> GameVersion { get; }
        public ReadOnlyReactiveCollection<ModViewModel> Mods { get; }
        public ReactiveProperty<bool> IsLoading { get; }
        public ReactiveCommand ApplyChangesCommand { get; }

        public ModsTabViewModel(BeatSaber beatSaber, BeatModsClient client, CompatTable compatTable) : base("Mods")
        {
            _beatSaber = beatSaber;
            _client = client;
            _compatTable = compatTable;

            GameVersion = beatSaber.ObserveProperty(w => w.GameVersion).Select(WithCompatibleVersion).ToReadOnlyReactiveProperty().AddTo(this);
            GameVersion.Subscribe(_ =>
            {
                // clear caches
                _client.AvailableMods.Clear();
            }).AddTo(this);
            Mods = client.AvailableMods.ToReadOnlyReactiveCollection(w => new ModViewModel(w, beatSaber)).AddTo(this);
            IsLoading = new ReactiveProperty<bool>(false).AddTo(this);
            ApplyChangesCommand = _beatSaber.ObserveProperty(w => w.HasChanges).ToReactiveCommand();
            ApplyChangesCommand.Subscribe(async _ =>
            {
                IsLoading.Value = true;

                await _beatSaber.ApplyChanges();
                await Task.Run(() => _beatSaber.CheckInstalledMods(_client.AvailableMods.ToList()));

                IsLoading.Value = false;
            }).AddTo(this);
        }

        public override async Task InitializeAsync()
        {
            IsLoading.Value = true;

            await _client.FetchAllModsAsync(_compatTable.Has(_beatSaber.GameVersion) ? _compatTable.As(_beatSaber.GameVersion) : _beatSaber.GameVersion);
            await Task.Run(() => _beatSaber.CheckInstalledMods(_client.AvailableMods.ToList()));

            IsLoading.Value = false;
        }

        private string WithCompatibleVersion(string version)
        {
            return _compatTable.Has(version) ? $"{version} (Compatible with {_compatTable.As(version)})" : version;
        }
    }
}