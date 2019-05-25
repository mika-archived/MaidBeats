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
        public ObservableCollection<Mod> InstalledMods { get; }

        public BeatSaber(IPlatform platform, StatusService statusService)
        {
            _platform = platform;
            _statusService = statusService;

            InstallationPath = null;
            GameVersion = null;
            InstalledMods = new ObservableCollection<Mod>();
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

        public void CheckInstalledMods(IEnumerable<Mod> mods)
        {
            _statusService.Text = "Checking installed mods...";

            foreach (var mod in mods)
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