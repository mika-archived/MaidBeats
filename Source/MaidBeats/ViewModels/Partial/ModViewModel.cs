using System;
using System.Collections.Specialized;
using System.Reactive.Linq;

using MaidBeats.Extensions;
using MaidBeats.Models;
using MaidBeats.Mvvm;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

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
        public string LatestVersion => _mod.LatestVersion.ToString();
        public ReactiveProperty<string> InstalledVersion { get; }
        public ReactiveProperty<bool?> IsLatestVersion { get; }
        public ReactiveProperty<bool> IsRequired { get; }
        public ReactiveProperty<string> DependentBy { get; }
        public ReactiveProperty<bool> IsChecked { get; }

        public ModViewModel(Mod mod, BeatSaber beatSaber)
        {
            _mod = mod;
            _beatSaber = beatSaber;
            InstalledVersion = mod.ObserveProperty(w => w.InstalledVersion).Select(w => w ?? "-").ToReactiveProperty().AddTo(this);
            IsLatestVersion = InstalledVersion.Select(w => w == "-" ? (bool?) null : w == LatestVersion).ToReactiveProperty().AddTo(this);
            IsRequired = mod.Dependents.ToCollectionChanged().Select(_ => mod.Dependents.Count > 0 || mod.IsRequired).ToReactiveProperty(mod.IsRequired).AddTo(this);
            DependentBy = new ReactiveProperty<string>();
            mod.Dependents.ToCollectionChanged().Subscribe(_ => DependentBy.Value = mod.Dependents.Count > 0 ? $"Dependent by {string.Join(", ", mod.Dependents)}" : null).AddTo(this);
            IsChecked = new ReactiveProperty<bool>(false);
            IsChecked.Subscribe(w => beatSaber.UpdateDependencyTree(_mod, w)).AddTo(this);
            beatSaber.ConfiguredMods.ToCollectionChanged().Subscribe(w =>
            {
                // DO NOT CHANGE INSTALLED MODS
                switch (w.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        if (Name == w.Value.Name)
                            IsChecked.Value = true;
                        break;

                    case NotifyCollectionChangedAction.Remove:
                        if (Name == w.Value.Name)
                            IsChecked.Value = false;
                        break;

                    case NotifyCollectionChangedAction.Reset:
                        IsChecked.Value = false;
                        break;
                }
            });
        }
    }
}