namespace MaidBeats.Models.Platform
{
    public interface IPlatform
    {
        string Name { get; }

        string TryToDetectInstallationPath();
    }
}