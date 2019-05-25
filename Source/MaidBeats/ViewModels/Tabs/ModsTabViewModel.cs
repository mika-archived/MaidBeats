using System;
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

        public ReadOnlyReactiveProperty<string> GameVersion { get; }
        public ReadOnlyReactiveCollection<ModViewModel> Mods { get; }
        public ReactiveProperty<bool> IsLoading { get; }

        public ModsTabViewModel(BeatSaber beatSaber, BeatModsClient client) : base("Mods")
        {
            _beatSaber = beatSaber;
            _client = client;

            GameVersion = beatSaber.ObserveProperty(w => w.GameVersion).ToReadOnlyReactiveProperty().AddTo(this);
            GameVersion.Subscribe(_ =>
            {
                // clear caches
                _client.AllMods.Clear();
                _client.AvailableMods.Clear();
            }).AddTo(this);
            Mods = client.AvailableMods.ToReadOnlyReactiveCollection(w => new ModViewModel(w, beatSaber)).AddTo(this);
            IsLoading = new ReactiveProperty<bool>(false).AddTo(this);
        }

        public override async Task InitializeAsync()
        {
            IsLoading.Value = true;

            await _client.FetchAllModsAsync(_beatSaber.GameVersion);
            await _client.FetchAvailableModsAsync(_beatSaber.GameVersion);
            _beatSaber.CheckInstalledMods(_client.AllMods);

            IsLoading.Value = false;
        }
    }
}