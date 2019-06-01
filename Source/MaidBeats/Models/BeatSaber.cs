using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows.Forms;

using MaidBeats.Models.BeatMods;
using MaidBeats.Models.Installers;
using MaidBeats.Models.Platform;

using Prism.Mvvm;

namespace MaidBeats.Models
{
    public class BeatSaber : BindableBase
    {
        private readonly HttpClient _httpClient;
        private readonly List<InstallerBase> _installers;
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
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", $"MaidBeats/{MaidBeatsInfo.Version.Value}");
            _installers = new List<InstallerBase> { new InstallerBase(), new BSIPA() };
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
                {
                    mod.InstalledVersion = null;
                }
                else
                {
                    mod.InstalledVersion = installed.Key; // set installed version
                    InstalledMods.Add(mod);
                }
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

        public async Task ApplyChanges()
        {
            var changes = ConfiguredMods.Where(w => w.InstalledVersion != null && w.InstalledVersion != w.LatestVersionStr).ToList();
            var installs = ConfiguredMods.Except(InstalledMods).ToList();
            var uninstalls = InstalledMods.Except(ConfiguredMods).ToList();
            var hasChanges = changes.Count > 0 || installs.Count > 0 || uninstalls.Count > 0;
            if (!hasChanges)
                return;

            Debug.WriteLine($"Updates : {string.Join(", ", changes.Select(w => w.Name))}");
            Debug.WriteLine($"Installs: {string.Join(", ", installs.Select(w => w.Name))}");
            Debug.WriteLine($"Uninstalls: {string.Join(",", uninstalls.Select(w => w.Name))}");

            await Update(changes);
            await Install(installs);
            await Uninstall(uninstalls);
        }

        private async Task Update(List<Mod> mods)
        {
            foreach (var mod in mods)
            {
                await Uninstall(new List<Mod> { mod }, false);
                await Install(new List<Mod> { mod }, false);
            }
        }

        private async Task Install(List<Mod> mods, bool doRunInstaller = true)
        {
            foreach (var mod in mods)
            {
                // download from remote
                var remote = mod.Versions[mod.LatestVersionStr].Downloads.FirstOrDefault(w => w.Type == "universal" || w.Type == "oculus");
                if (remote == null)
                    throw new InvalidOperationException();

                var destDir = Path.Combine(Path.GetTempPath(), "MaidBeats");
                if (!Directory.Exists(destDir))
                    Directory.CreateDirectory(destDir);

                var extractTo = Path.Combine(destDir, Path.GetFileNameWithoutExtension(remote.Url) ?? throw new InvalidOperationException());
                using (var response = await _httpClient.GetAsync(new Uri($"https://beatmods.com{remote.Url}"), HttpCompletionOption.ResponseHeadersRead)) // TODO: Move to BeatModsClient
                {
                    response.EnsureSuccessStatusCode();
                    using var stream = await response.Content.ReadAsStreamAsync();
                    using var zip = new ZipArchive(stream);
                    zip.ExtractToDirectory(extractTo);
                }

                // checking md5sum
                var isValid = remote.HashMd5.All(w => CalcMd5(Path.Combine(extractTo, w.File)) == w.Hash);
                if (!isValid)
                    continue; // invalid md5sum, oh no....

                foreach (var file in remote.HashMd5)
                {
                    // copy and install to BeatSaber directory
                    var to = Path.Combine(InstallationPath, file.File);
                    if (!Directory.Exists(to))
                        Directory.CreateDirectory(Path.GetDirectoryName(to) ?? throw new InvalidOperationException());
                    File.Copy(Path.Combine(extractTo, file.File), Path.Combine(InstallationPath, file.File));
                }

                // install process per mod
                if (doRunInstaller)
                    foreach (var installer in _installers.Where(w => w.ShouldProcess(mod.Name)).OrderBy(w => w.Priority))
                        installer.Install(InstallationPath, mod);

                // cleanup directory
                Directory.Delete(extractTo, true);
            }
        }

        private Task Uninstall(List<Mod> mods, bool doRunUninstaller = true)
        {
            foreach (var mod in mods)
            {
                var files = mod.Versions[mod.InstalledVersion].Downloads.FirstOrDefault(w => w.Type == "universal" || w.Type == "oculus");
                if (files == null)
                    throw new InvalidOperationException();

                // uninstall process per mod
                if (doRunUninstaller)
                    foreach (var installer in _installers.Where(w => w.ShouldProcess(mod.Name)).OrderBy(w => w.Priority))
                        installer.Uninstall(InstallationPath, mod);

                foreach (var file in files.HashMd5)
                {
                    var path = Path.Combine(InstallationPath, file.File);
                    if (!File.Exists(path))
                        continue;
                    File.Delete(path);
                }
            }

            return Task.CompletedTask;
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