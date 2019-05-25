using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace MaidBeats.Models
{
    public class BeatMods
    {
        private readonly string _host = "https://beatmods.com";
        private readonly HttpClient _httpClient;
        public ObservableCollection<Mod> AvailableMods { get; }
        public ObservableCollection<Mod> Mods { get; }

        public BeatMods()
        {
            AvailableMods = new ObservableCollection<Mod>();
            Mods = new ObservableCollection<Mod>();
            _httpClient = new HttpClient();
        }

        public async Task FetchAsync(string gameVersion)
        {
            AvailableMods.Clear();
            Mods.Clear();

            var uri = new Uri($"{_host}/api/v1/mod");
            var response = await _httpClient.GetAsync(uri);
            foreach (var mod in JsonConvert.DeserializeObject<IEnumerable<Mod>>(await response.Content.ReadAsStringAsync()))
            {
                if (mod.GameVersion == gameVersion && mod.Status == "approved")
                    AvailableMods.Add(mod);
                Mods.Add(mod);
            }
        }
    }
}