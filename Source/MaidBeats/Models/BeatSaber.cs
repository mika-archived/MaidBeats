using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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