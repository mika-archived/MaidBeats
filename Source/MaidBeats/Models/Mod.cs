using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using MaidBeats.Models.BeatMods;

using Prism.Mvvm;

using SemVer;

namespace MaidBeats.Models
{
    public class Mod : BindableBase
    {
        private readonly Dictionary<string, List<string>> _dependencies;
        private readonly Dictionary<string, RawMod> _versions;

        public Author Author => Versions[LatestVersionStr].Author;
        public string Category => Versions[LatestVersionStr].Category;
        public string Description => Versions[LatestVersionStr].Description;
        public string GameVersion => Versions[LatestVersionStr].GameVersion;
        public string Name => Versions[LatestVersionStr].Name;
        public bool IsRequired => Versions[LatestVersionStr].IsRequired;
        public string Status => Versions[LatestVersionStr].Status;
        public Version LatestVersion { get; set; }
        public string LatestVersionStr => LatestVersion.ToString();
        public ReadOnlyDictionary<string, RawMod> Versions => new ReadOnlyDictionary<string, RawMod>(_versions);
        public ReadOnlyDictionary<string, List<string>> Dependencies => new ReadOnlyDictionary<string, List<string>>(_dependencies);
        public ObservableCollection<string> Dependents { get; }

        public Mod(RawMod mod)
        {
            _versions = new Dictionary<string, RawMod>();
            _dependencies = new Dictionary<string, List<string>>();
            Dependents = new ObservableCollection<string>();
            AddVersion(mod.Version, mod);
        }

        public void AddVersion(string version, RawMod raw)
        {
            if (_versions.ContainsKey(version))
                return;
            _versions.Add(version, raw);
            LatestVersion = Versions.Max(w => new Version(w.Key));
        }

        public void AddDependencies(string version, List<string> dependencies)
        {
            if (_dependencies.ContainsKey(version))
                return;
            _dependencies.Add(version, dependencies);
        }

        #region InstalledVersion

        private string _installedVersion;

        public string InstalledVersion
        {
            get => _installedVersion;
            set
            {
                if (_installedVersion != value)
                    SetProperty(ref _installedVersion, value);
            }
        }

        #endregion
    }
}