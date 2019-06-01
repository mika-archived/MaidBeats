using System;
using System.Diagnostics;
using System.IO;

namespace MaidBeats.Models.Installers
{
    // ReSharper disable once InconsistentNaming
    public class BSIPA : InstallerBase
    {
        private const string Executable = "IPA.exe";

        public override string TargetPattern => "BSIPA";

        public override void Install(string root, Mod mod)
        {
            if (!IsExistsIPA(root))
                throw new InvalidOperationException();

            var path = Path.Combine(root, Executable);
            var process = Process.Start(new ProcessStartInfo(path, "--nowait") { WorkingDirectory = root });
            process?.WaitForExit();

            base.Install(root, mod);
        }

        public override void Uninstall(string root, Mod mod)
        {
            if (!IsExistsIPA(root))
                throw new InvalidOperationException();

            var path = Path.Combine(root, Executable);
            var process = Process.Start(new ProcessStartInfo(path, "--revert --nowait") { WorkingDirectory = root });
            process?.WaitForExit();

            base.Uninstall(root, mod);
        }

        private bool IsExistsIPA(string root)
        {
            return File.Exists(Path.Combine(root, Executable));
        }
    }
}