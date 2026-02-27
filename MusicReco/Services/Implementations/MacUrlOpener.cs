using System.Diagnostics;

namespace MusicReco.Services.Implementations;

internal sealed class MacUrlOpener : IUrlOpener
{
    public void OpenUrl(string url)
    {
        var psi = new ProcessStartInfo
        {
            FileName = "open",
            UseShellExecute = false
        };

        psi.ArgumentList.Add(url);
        Process.Start(psi);
    }
}
