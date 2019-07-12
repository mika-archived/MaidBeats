using System.Collections.Generic;

using Newtonsoft.Json;

namespace MaidBeats.Models.BeatSaver
{
    public class SongCustomData
    {
        [JsonProperty("_contributors")]
        public IEnumerable<Contributor> Contributors { get; set; }

        [JsonProperty("customEnvironment")]
        public string CustomEnvironment { get; set; }

        [JsonProperty("customEnvironmentHash")]
        public string CustomEnvironmentHash { get; set; }
    }
}