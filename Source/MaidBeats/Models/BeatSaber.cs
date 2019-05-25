using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Forms;

using Prism.Mvvm;

namespace MaidBeats.Models
{
    public class BeatSaber : BindableBase
    {
        public ObservableCollection<string> GameVersions { get; }

        public BeatSaber()
        {
            InstallationPath = null;
            GameVersions = new ObservableCollection<string> { "1.0.0" };
            Version = GameVersions[0];
        }

        public void TryToDetectInstallationPath()
        {
            var programs = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            var oculus = Path.Combine(programs, "Oculus");
            if (!Directory.Exists(oculus))
                return; //

            var software = Path.Combine(oculus, "Software", "Software");
            if (!Directory.Exists(software))
                return;

            var beatsaber = Path.Combine(software, "hyperbolic-magnetism-beat-saber");
            if (!Directory.Exists(beatsaber))
                return;

            InstallationPath = beatsaber;
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