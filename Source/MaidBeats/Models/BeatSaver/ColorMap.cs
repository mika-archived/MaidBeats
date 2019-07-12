using Newtonsoft.Json;

namespace MaidBeats.Models.BeatSaver
{
    public class ColorMap
    {
        [JsonProperty("b")]
        public double B { get; set; }

        [JsonProperty("g")]
        public double G { get; set; }

        [JsonProperty("r")]
        public double R { get; set; }
    }
}