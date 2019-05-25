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

        public ObservableCollection<string> GameVersions { get; }
        public ObservableCollection<Mod> AllMods { get; }
        public ObservableCollection<Mod> AvailableMods { get; }

        public BeatModsClient()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", $"MaidBeats/{MaidBeatsInfo.Version.Value}");
            GameVersions = new ObservableCollection<string>();
            AllMods = new ObservableCollection<Mod>();
            AvailableMods = new ObservableCollection<Mod>();
        }

        /// <summary>
        ///     fetch supported version
        /// </summary>
        public async Task FetchGameVersionsAsync()
        {
            if (GameVersions.Count > 0)
                return; // use cache

            var versions = await GetAsync<IEnumerable<string>>("version");
            GameVersions.AddRange(versions);
        }

        public async Task FetchAllModsAsync()
        {
            if (AllMods.Count > 0)
                return;

            var mods = await ModsAsync();
            AllMods.AddRange(mods);
        }

        public async Task FetchAvailableModsAsync(string gameVersion)
        {
            AvailableMods.Clear();

            var parameters = new Dictionary<string, object> { { "gameVersion", gameVersion }, { "status", "approved" } };
            var mods = await ModsAsync(parameters);
            AvailableMods.AddRange(mods);
        }

        /// <summary>
        ///     fetch registered mod list, supported parameters are category, status, name, version, gameVersion, author and sort
        /// </summary>
        private async Task<IEnumerable<Mod>> ModsAsync(Dictionary<string, object> parameters = null)
        {
            return await GetAsync<IEnumerable<Mod>>("mod", parameters);
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