namespace MusicReco.Domain;

internal static class LastFmLinks
{
    public static string BuildAlbumUrl(string artist, string album)
    {
        var encodedArtist = Uri.EscapeDataString(artist);
        var encodedAlbum = Uri.EscapeDataString(album);
        return $"https://www.last.fm/music/{encodedArtist}/{encodedAlbum}";
    }
}
