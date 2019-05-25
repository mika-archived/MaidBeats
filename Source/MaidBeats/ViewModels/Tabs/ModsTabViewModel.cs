using System.Collections.ObjectModel;

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
            Mods = new ObservableCollection<ModViewModel>().ToReadOnlyReactiveCollection().AddTo(this);
            IsLoading = new ReactiveProperty<bool>(false).AddTo(this);
        }
    }
}