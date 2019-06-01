using System.Text.RegularExpressions;

namespace MaidBeats.Models.Installers
{
    // If specified mod requires some processes, implement install process
    public class InstallerBase
    {
        private Regex _targetPattern;
        public virtual string TargetPattern => ".*";
        public virtual int Priority => GetType() == typeof(InstallerBase) ? 0 : 1;

        public bool ShouldProcess(string name)
        {
            if (_targetPattern == null)
                _targetPattern = new Regex($"^{TargetPattern}$");
            return _targetPattern.IsMatch(name);
        }

        public virtual void Install(string root, Mod mod)
        {
            // EMPTY
        }

        public virtual void Uninstall(string root, Mod mod)
        {
            // EMPTY
        }
    }
}