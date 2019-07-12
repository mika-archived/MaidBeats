using System.Threading.Tasks;

using MaidBeats.Models;

namespace MaidBeats.ViewModels.Tabs
{
    public class SongsTabViewModel : TabBaseViewModel
    {
        private readonly BeatSaber _beatSaber;

        public SongsTabViewModel(BeatSaber beatSaber) : base("Songs")
        {
            _beatSaber = beatSaber;
        }

        public override async Task InitializeAsync()
        {
            await Task.Run(() => _beatSaber.CheckInstalledSongs());
        }
    }
}