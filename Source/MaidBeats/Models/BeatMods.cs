using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace MaidBeats.Models
{
    public class BeatMods
    {
        private readonly string _host = "https://beatmods.com";
        private readonly HttpClient _httpClient;
        public ObservableCollection<Mod> Mods { get; }

        public BeatMods()
        {
            Mods = new ObservableCollection<Mod>();
            _httpClient = new HttpClient();
        }

        public async Task FetchAsync(string version = "1.0.0")
        {
            Mods.Clear();

            var uri = new Uri($"{_host}/api/v1/mod?search=&status=approved&sort=&sortDirection=1");
            var response = await _httpClient.GetAsync(uri);
            foreach (var mod in JsonConvert.DeserializeObject<IEnumerable<Mod>>(await response.Content.ReadAsStringAsync()).Where(w => w.GameVersion == version))
                Mods.Add(mod);
        }
    }
}