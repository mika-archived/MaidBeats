using System.Collections.Generic;
using System.Linq;

namespace MaidBeats.Models
{
    public class CompatTable
    {
        private readonly Dictionary<string, string> _compatTable;

        public CompatTable()
        {
            _compatTable = new Dictionary<string, string>
            {
                { "1.0.1", "1.0.0" }
            };
        }

        public string As(string version)
        {
            return _compatTable.ContainsKey(version) ? _compatTable[version] : version;
        }

        public bool Has(string version)
        {
            return !string.IsNullOrWhiteSpace(version) && _compatTable.ContainsKey(version);
        }

        public List<string> With(string version)
        {
            return _compatTable.ContainsValue(version) ? _compatTable.Where(w => w.Value == version).Select(w => w.Key).ToList() : null;
        }
    }
}