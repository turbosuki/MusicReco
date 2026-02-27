using System.Text.Json;

namespace MusicReco;

internal sealed class LastFmClient(HttpClient http, string apiKey) : ILastFmClient
{
    private const string ApiRoot = "https://ws.audioscrobbler.com/2.0/";

    public async Task<List<string>> GetSimilarArtists(string artist, int limit)
    {
        var url = BuildApiUrl(new Dictionary<string, string>
        {
            ["method"] = "artist.getsimilar",
            ["artist"] = artist,
            ["limit"] = limit.ToString(),
            ["autocorrect"] = "1",
            ["api_key"] = apiKey,
            ["format"] = "json"
        });

        using var doc = await GetJson(url);

        if (!doc.RootElement.TryGetProperty("similarartists", out var similarArtists) ||
            !similarArtists.TryGetProperty("artist", out var artistsElem))
        {
            return [];
        }

        var list = new List<string>();
        foreach (var item in EnumerateAsArray(artistsElem))
        {
            if (!item.TryGetProperty("name", out var nameElem))
                continue;

            var name = (nameElem.GetString() ?? string.Empty).Trim();
            if (!string.IsNullOrWhiteSpace(name))
                list.Add(name);
        }

        return DistinctPreserveOrder(list);
    }

    public async Task<List<string>> GetTopAlbums(string artist, int limit)
    {
        var url = BuildApiUrl(new Dictionary<string, string>
        {
            ["method"] = "artist.gettopalbums",
            ["artist"] = artist,
            ["limit"] = limit.ToString(),
            ["autocorrect"] = "1",
            ["api_key"] = apiKey,
            ["format"] = "json"
        });

        using var doc = await GetJson(url);
        if (!doc.RootElement.TryGetProperty("topalbums", out var topAlbums) ||
            !topAlbums.TryGetProperty("album", out var albumsElem))
        {
            return [];
        }

        var list = new List<string>();
        foreach (var item in EnumerateAsArray(albumsElem))
        {
            if (!item.TryGetProperty("name", out var nameElem))
                continue;

            var name = (nameElem.GetString() ?? string.Empty).Trim();
            if (!string.IsNullOrWhiteSpace(name))
                list.Add(name);
        }

        return DistinctPreserveOrder(list);
    }

    private static string BuildApiUrl(Dictionary<string, string> query)
    {
        var qs = string.Join("&", query.Select(kvp =>
            $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));

        return $"{ApiRoot}?{qs}";
    }

    private async Task<JsonDocument> GetJson(string url)
    {
        var json = await http.GetStringAsync(url);
        var doc = JsonDocument.Parse(json);

        if (doc.RootElement.TryGetProperty("error", out var err))
        {
            var msg = doc.RootElement.TryGetProperty("message", out var message)
                ? message.GetString()
                : "Unknown Last.fm error";

            throw new InvalidOperationException($"Last.fm error {err.GetInt32()}: {msg}");
        }

        return doc;
    }

    private static IEnumerable<JsonElement> EnumerateAsArray(JsonElement elem)
    {
        if (elem.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in elem.EnumerateArray())
                yield return item;
            yield break;
        }

        if (elem.ValueKind == JsonValueKind.Object)
            yield return elem;
    }

    private static List<string> DistinctPreserveOrder(IEnumerable<string> input)
    {
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        return input.Where(seen.Add).ToList();
    }
}
