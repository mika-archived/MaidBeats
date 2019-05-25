using System.IO;

using Microsoft.Win32;

namespace MaidBeats.Models.Platform
{
    public class Oculus : IPlatform
    {
        public string Name => "Oculus";

        public string TryToDetectInstallationPath()
        {
            using var registry = Registry.CurrentUser.OpenSubKey(@"Software\Oculus VR, LLC\Oculus\Libraries", false);
            if (registry == null)
                return null;

            string path = null;
            foreach (var name in registry.GetSubKeyNames())
            {
                using var sub = registry.OpenSubKey(name, false);
                var value = sub?.GetValue("OriginalPath") as string;
                if (string.IsNullOrWhiteSpace(value))
                    continue;

                if (!Directory.Exists(value))
                    continue;

                var beatSaber = Path.Combine(value, "Software", "hyperbolic-magnetism-beat-saber");
                if (!Directory.Exists(value))
                    continue;

                // hit
                path = beatSaber;
                sub.Close();
                break;
            }
            registry.Close();
            return path;
        }
    }
}