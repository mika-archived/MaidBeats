using System;

namespace MaidBeats.Models.Platform
{
    public class Steam : IPlatform
    {
        public string Name => "Steam";

        public string TryToDetectInstallationPath()
        {
            throw new NotImplementedException();
        }
    }
}