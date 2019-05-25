using System.Collections.Generic;

using Newtonsoft.Json;

namespace MaidBeats.Models.BeatMods
{
    public class Download
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("hashMd5")]
        public IEnumerable<HashMD5> HashMd5 { get; set; }
    }
}