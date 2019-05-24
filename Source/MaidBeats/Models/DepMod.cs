using System.Collections.Generic;

using Newtonsoft.Json;

namespace MaidBeats.Models
{
    public class DepMod : Mod
    {
        [JsonProperty("dependencies")]
        public new IEnumerable<string> Dependencies { get; }
    }
}