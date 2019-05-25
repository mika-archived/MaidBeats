using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Windows.Forms;

using Prism.Mvvm;

namespace MaidBeats.Models
{
    public class BeatSaber : BindableBase
    {
        private readonly Oculus _oculus;
        public ObservableCollection<string> GameVersions { get; }
        public ObservableCollection<Mod> InstalledMods { get; }

        public BeatSaber(Oculus oculus)
        {
            _oculus = oculus;
            InstallationPath = null;
            GameVersions = new ObservableCollection<string> { "1.0.0" }; // equals to supported version
            GameVersion = GameVersions[0];
            InstalledMods = new ObservableCollection<Mod>();
        }

        public void TryToDetectInstallationPath()
        {
            foreach (var path in _oculus.LibraryPaths)
            {
                if (!Directory.Exists(path))
                    continue;

                var software = Path.Combine(path, "Software");
                if (!Directory.Exists(software))
                    continue;

                var beatSaber = Path.Combine(software, "hyperbolic-magnetism-beat-saber");
                if (!Directory.Exists(beatSaber))
                    continue;
                InstallationPath = beatSaber;
                break;
            }
        }

        public void CheckGameVersion()
        {
            var path = Path.Combine(InstallationPath, "BeatSaberVersion.txt");
            using var stream = new StreamReader(path);
            var version = stream.ReadToEnd();
            if (GameVersions.Contains(version))
                GameVersion = version;
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

        public void CheckInstalledMods(IEnumerable<Mod> mods)
        {
            foreach (var mod in mods.Where(w => w.GameVersion == GameVersion))
                foreach (var platform in mod.Downloads)
                {
                    // currently, only support oculus or universal binary
                    if (platform.Type == "steam")
                        continue;

                    var installed = platform.HashMd5.All(w => CalcMd5(Path.Combine(InstallationPath, w.File)) == w.Hash);
                    if (installed)
                        InstalledMods.Add(mod);
                }
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

        #region Version

        private string _gameVersion;

        public string GameVersion
        {
            get => _gameVersion;
            private set
            {
                if (_gameVersion != value)
                    SetProperty(ref _gameVersion, value);
            }
        }

        #endregion
    }
}