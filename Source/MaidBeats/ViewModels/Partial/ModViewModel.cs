using System;
using System.Collections.Specialized;

using MaidBeats.Extensions;
using MaidBeats.Models;
using MaidBeats.Models.BeatMods;
using MaidBeats.Mvvm;

using Reactive.Bindings;

namespace MaidBeats.ViewModels.Partial
{
    public class ModViewModel : ViewModel
    {
        private readonly BeatSaber _beatSaber;
        private readonly Mod _mod;

        public string AuthorName => _mod.Author?.Username ?? "-";
        public string Category => _mod.Category;
        public string Description => _mod.Description.Replace("\r", "").Replace("\n", "");
        public string Name => _mod.Name;
        public string LatestVersion => _mod.Version;
        public ReactiveProperty<string> InstalledVersion { get; }
        public bool IsRequired => _mod.IsRequired;
        public ReactiveProperty<bool?> IsLatestVersion { get; }
        public ReactiveProperty<bool> IsChecked { get; }

        public ModViewModel(Mod mod, BeatSaber beatSaber)
        {
            _mod = mod;
            _beatSaber = beatSaber;
            InstalledVersion = new ReactiveProperty<string>("-");
            IsLatestVersion = new ReactiveProperty<bool?>();
            IsChecked = new ReactiveProperty<bool>(_mod.IsRequired);

            _beatSaber.InstalledMods.ToCollectionChanged().Subscribe(w =>
            {
                if (w.Value.Name != _mod.Name)
                    return;
                if (w.Action == NotifyCollectionChangedAction.Remove || w.Action == NotifyCollectionChangedAction.Reset)
                {
                    IsChecked.Value = false; // removed from installed (uninstalled by hand or other application)
                    IsLatestVersion.Value = null;
                    InstalledVersion.Value = "-";
                }
                else if (w.Action == NotifyCollectionChangedAction.Add)
                {
                    IsChecked.Value = true;
                    IsLatestVersion.Value = _mod.Version == w.Value.Version;
                    InstalledVersion.Value = w.Value.Version;
                }
            }).AddTo(this);
        }
    }
}