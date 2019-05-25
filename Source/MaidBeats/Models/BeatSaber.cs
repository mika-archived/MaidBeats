using System;
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

        public BeatSaber(Oculus oculus)
        {
            _oculus = oculus;
            InstallationPath = null;
            GameVersions = new ObservableCollection<string> { "1.0.0" };
            Version = GameVersions[0];
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
            set
            {
                if (_installationPath != value)
                    SetProperty(ref _installationPath, value);
            }
        }

        #endregion

        #region Version

        private string _version;

        public string Version
        {
            get => _version;
            set
            {
                if (_version != value)
                    SetProperty(ref _version, value);
            }
        }

        #endregion
    }
}