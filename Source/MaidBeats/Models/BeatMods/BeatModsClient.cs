﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Prism.Mvvm;

namespace MaidBeats.Models.BeatMods
{
    public class BeatModsClient : BindableBase
    {
        private readonly string _host = "https://beatmods.com";
        private readonly HttpClient _httpClient;
        private readonly StatusService _statusService;
        public ObservableCollection<string> GameVersions { get; }
        public ObservableCollection<Mod> AvailableMods { get; }

        public BeatModsClient(StatusService statusService)
        {
            _statusService = statusService;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", $"MaidBeats/{MaidBeatsInfo.Version.Value}");
            GameVersions = new ObservableCollection<string>();
            AvailableMods = new ObservableCollection<Mod>();
        }

        /// <summary>
        ///     fetch supported version
        /// </summary>
        public async Task FetchGameVersionsAsync()
        {
            if (GameVersions.Count > 0)
                return; // use cache

            _statusService.Text = "Fetching game versions from remote...";
            var versions = await GetAsync<IEnumerable<string>>("version");
            GameVersions.AddRange(versions);
        }

        public async Task FetchAllModsAsync(string gameVersion)
        {
            if (AvailableMods.Count > 0)
                return;

            _statusService.Text = "Fetching available mod list for current game version from remote...";
            var parameters = new Dictionary<string, object> { { "gameVersion", gameVersion } };
            var rawMods = await ModsAsync(parameters);
            AvailableMods.AddRange(Flatten(rawMods));

            RegenerateDependencyTree();
        }

        private IEnumerable<Mod> Flatten(IEnumerable<RawMod> rawMods)
        {
            var mods = new List<Mod>();
            foreach (var rawMod in rawMods)
            {
                var mod = mods.SingleOrDefault(w => w.Name == rawMod.Name);
                if (mod == null)
                    mods.Add(new Mod(rawMod));
                else
                    mod.AddVersion(rawMod.Version, rawMod);
            }

            return mods.Where(w => w.Status == "approved");
        }

        // regenerate dependency tree, using latest approved version as dependencies in available mods
        private void RegenerateDependencyTree()
        {
            foreach (var mod in AvailableMods)
                foreach (var version in mod.Versions)
                {
                    if (!version.Value.HasDependencies)
                        continue;

                    var dependencies = version.Value.Dependencies.Select(w => AvailableMods.SingleOrDefault(v => w.Name == v.Name)).Where(w => w != null).Select(w => w.Name);
                    mod.AddDependencies(version.Key, dependencies.ToList());
                }
        }

        /// <summary>
        ///     fetch registered mod list, supported parameters are category, status, name, version, gameVersion, author and sort
        /// </summary>
        private async Task<IEnumerable<RawMod>> ModsAsync(Dictionary<string, object> parameters = null)
        {
            return await GetAsync<IEnumerable<RawMod>>("mod", parameters);
        }

        private async Task<T> GetAsync<T>(string url, Dictionary<string, object> parameters = null)
        {
            var endpoint = $"{_host}/api/v1/{url}";
            if (parameters != null)
                endpoint += "?" + string.Join("&", parameters.Select(w => $"{w.Key}={w.Value}"));
            using var response = await _httpClient.GetAsync(new Uri(endpoint));
            return JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
        }
    }
}