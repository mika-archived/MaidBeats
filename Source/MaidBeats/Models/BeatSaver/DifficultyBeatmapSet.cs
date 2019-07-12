using System.Collections.Generic;

using Newtonsoft.Json;

namespace MaidBeats.Models.BeatSaver
{
    public class DifficultyBeatmapSet
    {
        [JsonProperty("beatmapCharacteristicName")]
        public string BeatmapCharacteristicName { get; set; }

        [JsonProperty("_difficultyBeatmaps")]
        public IEnumerable<DifficultyBeatmap> DifficultyBeatmaps { get; set; }
    }
}