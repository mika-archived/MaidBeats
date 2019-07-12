using System.Collections.Generic;

using Newtonsoft.Json;

namespace MaidBeats.Models.BeatSaver
{
    public class DifficultyBeatmapCustomData
    {
        [JsonProperty("_colorLeft")]
        public ColorMap ColorLeft { get; set; }

        [JsonProperty("_colorRight")]
        public ColorMap ColorRight { get; set; }

        [JsonProperty("_difficultyLevel")]
        public string DifficultyLevel { get; set; }

        [JsonProperty("_editorOffset")]
        public long EditorOffset { get; set; }

        [JsonProperty("_editorOldOffset")]
        public long EditorOldOffset { get; set; }

        [JsonProperty("_information")]
        public IEnumerable<string> Information { get; set; }

        [JsonProperty("_requirements")]
        public IEnumerable<string> Requirements { get; set; }

        [JsonProperty("_suggestions")]
        public IEnumerable<string> Suggestions { get; set; }

        [JsonProperty("_warnings")]
        public IEnumerable<string> Warnings { get; set; }
    }
}