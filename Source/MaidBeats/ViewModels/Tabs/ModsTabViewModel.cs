using System.Threading.Tasks;

using MaidBeats.Models;
using MaidBeats.ViewModels.Partial;

using Reactive.Bindings;

namespace MaidBeats.ViewModels.Tabs
{
    public class ModsTabViewModel : TabBaseViewModel
    {
        public ReactiveCollection<ModViewModel> Mods { get; }
        public ReactiveProperty<bool> IsLoading { get; set; }

        public ModsTabViewModel() : base("Mods")
        {
            Mods = new ReactiveCollection<ModViewModel>();
            IsLoading = new ReactiveProperty<bool>(false);
        }

        public override async Task InitializeAsync()
        {
            Mods.Clear();
        }
    }
}