using System.Threading.Tasks;

using MaidBeats.Models;
using MaidBeats.ViewModels.Partial;

using Reactive.Bindings;

namespace MaidBeats.ViewModels.Tabs
{
    public class ModsTabViewModel : TabBaseViewModel
    {
        private readonly BeatMods _beatMods;
        public ReadOnlyReactiveCollection<ModViewModel> Mods { get; }
        public ReactiveProperty<bool> IsLoading { get; set; }

        public ModsTabViewModel() : base("Mods")
        {
            _beatMods = new BeatMods();
            Mods = _beatMods.Mods.ToReadOnlyReactiveCollection(w => new ModViewModel(w));
            IsLoading = new ReactiveProperty<bool>(false);
        }

        public override async Task InitializeAsync()
        {
            IsLoading.Value = true;
            await _beatMods.FetchAsync();
            ;
            IsLoading.Value = false;
        }
    }
}