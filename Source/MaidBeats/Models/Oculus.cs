using System.Collections.ObjectModel;

using Microsoft.Win32;

namespace MaidBeats.Models
{
    public class Oculus
    {
        public ObservableCollection<string> LibraryPaths { get; }

        public Oculus()
        {
            LibraryPaths = new ObservableCollection<string>();
        }

        public void GetLibraryPaths()
        {
            using var registry = Registry.CurrentUser.OpenSubKey(@"Software\Oculus VR, LLC\Oculus\Libraries", false);
            if (registry == null)
                return;

            foreach (var name in registry.GetSubKeyNames())
            {
                using var sub = registry.OpenSubKey(name, false);
                var value = sub?.GetValue("OriginalPath") as string;
                if (!string.IsNullOrWhiteSpace(value))
                    LibraryPaths.Add(value);
                sub?.Close();
            }
            registry.Close();
        }
    }
}