using Newtonsoft.Json;

namespace MaidBeats.Models.BeatSaver
{
    public class DifficultyBeatmap
    {
        [JsonProperty("_beatmapFilename")]
        public string BeatmapFilename { get; set; }

        [JsonProperty("_difficulty")]
        public string Difficulty { get; set; }

        [JsonProperty("_difficultyRank")]
        public long DifficultyRank { get; set; }

        [JsonProperty("_noteJumpMovementSpeed")]
        public long NoteJumpMovementSpeed { get; set; }

        [JsonProperty("_noteJumpStartBeatOffset")]
        public long NoteJumpStartBeatOffset { get; set; }
    }
}