using MaidBeats.Models;
using MaidBeats.Mvvm;

namespace MaidBeats.ViewModels.Partial
{
    public class ModViewModel : ViewModel
    {
        private readonly Mod _mod;

        public string AuthorName => _mod.Author?.Username ?? "-";
        public string Category => _mod.Category;
        public string Description => _mod.Description.Replace("\r", "").Replace("\n", "");
        public string Name => _mod.Name;
        public string LatestVersion => _mod.Version;
        public string InstalledVersion => "1.0.0";

        public ModViewModel(Mod mod)
        {
            _mod = mod;
        }
    }
}