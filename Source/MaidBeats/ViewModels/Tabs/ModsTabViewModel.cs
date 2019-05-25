using System.Threading.Tasks;

using MaidBeats.Extensions;
using MaidBeats.Models;
using MaidBeats.ViewModels.Partial;

using Reactive.Bindings;

namespace MaidBeats.ViewModels.Tabs
{
    public class ModsTabViewModel : TabBaseViewModel
    {
        private readonly BeatMods _beatMods;
        private readonly BeatSaber _beatSaber;
        public ReadOnlyReactiveCollection<ModViewModel> Mods { get; }
        public ReactiveProperty<bool> IsLoading { get; }

        public ModsTabViewModel(BeatSaber beatSaber) : base("Mods")
        {
            _beatSaber = beatSaber;
            _beatMods = new BeatMods();
            Mods = _beatMods.AvailableMods.ToReadOnlyReactiveCollection(w => new ModViewModel(w, _beatSaber)).AddTo(this);
            IsLoading = new ReactiveProperty<bool>(false).AddTo(this);
        }

        public override async Task InitializeAsync()
        {
            IsLoading.Value = true;

            await _beatMods.FetchAsync(_beatSaber.GameVersion);
            _beatSaber.CheckInstalledMods(_beatMods.Mods);

            IsLoading.Value = false;
        }
    }
}