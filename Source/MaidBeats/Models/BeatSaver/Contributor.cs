using Newtonsoft.Json;

namespace MaidBeats.Models.BeatSaver
{
    public class Contributor
    {
        [JsonProperty("_iconPath")]
        public string IconPath { get; set; }

        [JsonProperty("_name")]
        public string Name { get; set; }

        [JsonProperty("_role")]
        public string Role { get; set; }
    }
}