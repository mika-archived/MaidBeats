using Newtonsoft.Json;

namespace MaidBeats.Models.BeatMods
{
    public class DifficultyLevel
    {
        [JsonProperty("audioPath")]
        public string AudioPath { get; set; }

        [JsonProperty("difficulty")]
        public string Difficulty { get; set; }

        [JsonProperty("difficultyRank")]
        public int DifficultyRank { get; set; }

        [JsonProperty("jsonPath")]
        public string JsonPath { get; set; }

        [JsonProperty("offset")]
        public int Offset { get; set; }
    }
}