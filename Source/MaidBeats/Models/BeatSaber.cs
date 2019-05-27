using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Windows.Forms;

using MaidBeats.Models.BeatMods;
using MaidBeats.Models.Platform;

using Prism.Mvvm;

namespace MaidBeats.Models
{
    public class BeatSaber : BindableBase
    {
        private readonly IPlatform _platform;
        private readonly StatusService _statusService;
        private bool _isConfiguring;
        public ObservableCollection<Mod> AvailableMods { get; }
        public ObservableCollection<Mod> InstalledMods { get; }
        public ObservableCollection<Mod> ConfiguredMods { get; }

        public BeatSaber(IPlatform platform, StatusService statusService)
        {
            _platform = platform;
            _statusService = statusService;
            _isConfiguring = false;

            InstallationPath = null;
            GameVersion = null;
            AvailableMods = new ObservableCollection<Mod>(); // All Installable Mods
            InstalledMods = new ObservableCollection<Mod>(); // Already Installed Mods
            ConfiguredMods = new ObservableCollection<Mod>(); // Configuring (Changed) Mods
        }

        public void TryToDetectInstallationPath()
        {
            InstallationPath = _platform.TryToDetectInstallationPath();
        }

        public void TryToDetectGameVersion()
        {
            var path = Path.Combine(InstallationPath, "BeatSaberVersion.txt");
            if (!File.Exists(path))
                return;

            using var stream = new StreamReader(path);
            GameVersion = stream.ReadToEnd();
        }

        public void SelectInstallationPathByUser()
        {
            var dialog = new FolderBrowserDialog
            {
                // ReSharper disable once LocalizableElement
                Description = "Please Select BeatSaber Installed Location",
                RootFolder = Environment.SpecialFolder.ProgramFiles,
                ShowNewFolderButton = false
            };

            if (dialog.ShowDialog() != DialogResult.OK)
                return;
            InstallationPath = dialog.SelectedPath;
        }

        public void CheckInstalledMods(List<Mod> mods)
        {
            _statusService.Text = "Checking installed mods...";
            _isConfiguring = true;
            AvailableMods.Clear();
            InstalledMods.Clear();
            ConfiguredMods.Clear();

            AvailableMods.AddRange(mods);

            // local func
            bool IsInstalled(IEnumerable<Download> platforms)
            {
                foreach (var platform in platforms)
                {
                    if (platform.Type == "steam")
                        continue;
                    return platform.HashMd5.All(w => CalcMd5(Path.Combine(InstallationPath, w.File)) == w.Hash);
                }
                return false;
            }

            foreach (var mod in mods)
            {
                var installed = mod.Versions.FirstOrDefault(w => IsInstalled(w.Value.Downloads));
                if (installed.Equals(default(KeyValuePair<string, RawMod>)))
                    continue;

                mod.InstalledVersion = installed.Key; // set installed version
                InstalledMods.Add(mod);
            }

            ConfiguredMods.AddRange(InstalledMods);

            ClearDependencyTree();
            CreateDependencyTree();

            _isConfiguring = false;
        }

        public void CreateDependencyTree()
        {
            foreach (var mod in ConfiguredMods)
            {
                if (!mod.Dependencies.ContainsKey(mod.LatestVersionStr))
                    continue;

                foreach (var dependency in mod.Dependencies[mod.LatestVersionStr])
                {
                    var dep = AvailableMods.SingleOrDefault(w => w.Name == dependency);
                    dep?.Dependents?.Add(mod.Name);
                }
            }
        }

        public void UpdateDependencyTree(Mod mod, bool isInstalling, bool doRemove = true)
        {
            if (_isConfiguring)
                return; // skip

            if (isInstalling)
            {
                if (mod.Dependencies.ContainsKey(mod.LatestVersionStr))
                    foreach (var dependency in mod.Dependencies[mod.LatestVersionStr])
                    {
                        var dep = AvailableMods.SingleOrDefault(w => w.Name == dependency);
                        if (dep == null)
                            continue;

                        if (!dep.Dependents.Contains(mod.Name))
                            dep.Dependents.Add(mod.Name);
                        UpdateDependencyTree(dep, true);
                    }

                if (!ConfiguredMods.Contains(mod))
                    ConfiguredMods.Add(mod);
            }
            else
            {
                if (mod.Dependencies.ContainsKey(mod.LatestVersionStr))
                    foreach (var dependency in mod.Dependencies[mod.LatestVersionStr])
                    {
                        var dep = AvailableMods.SingleOrDefault(w => w.Name == dependency);
                        if (dep == null)
                            continue;

                        dep.Dependents.Remove(mod.Name);

                        // UpdateDependencyTree(dep, false, false);
                    }

                if (mod.Dependents.Count <= 0 && doRemove)
                    ConfiguredMods.Remove(mod);
            }
        }

        public void ClearDependencyTree()
        {
            foreach (var mod in AvailableMods)
                mod.Dependents.Clear();
        }

        private string CalcMd5(string path)
        {
            if (!File.Exists(path))
                return null;

            using var md5 = MD5.Create();
            using var stream = new FileStream(path, FileMode.Open);
            var hash = md5.ComputeHash(stream);
            stream.Close();

            return string.Concat(hash.Select(w => w.ToString("x2")));
        }

        #region InstallationPath

        private string _installationPath;

        public string InstallationPath
        {
            get => _installationPath;
            private set
            {
                if (_installationPath != value)
                    SetProperty(ref _installationPath, value);
            }
        }

        #endregion

        #region GameVersion

        private string _gameVersion;

        public string GameVersion
        {
            get => _gameVersion;
            set
            {
                if (_gameVersion != value)
                    SetProperty(ref _gameVersion, value);
            }
        }

        #endregion
    }
}