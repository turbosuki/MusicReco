namespace MusicReco;

internal sealed class MusicNowPlayingProvider : INowPlayingProvider
{
    public NowPlaying GetNowPlaying()
    {
        const string script = """

                              tell application "Music"
                                  if not running then return ""
                                  if player state is not playing then return ""
                                  set tName to name of current track
                                  set tArtist to artist of current track
                                  return tName & "||" & tArtist
                              end tell

                              """;

        var output = ProcessHelpers.RunProcessCaptureStdout("osascript", ["-e", script]).Trim();
        if (string.IsNullOrWhiteSpace(output))
            throw new InvalidOperationException("Music is not playing. Start a song in Apple Music and try again.");

        var parts = output.Split("||", 2, StringSplitOptions.None);
        if (parts.Length != 2 || string.IsNullOrWhiteSpace(parts[1]))
            throw new InvalidOperationException("Could not read track/artist from Apple Music.");

        return new NowPlaying(parts[0].Trim(), parts[1].Trim());
    }
}
