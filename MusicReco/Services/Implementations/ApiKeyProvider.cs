namespace MusicReco;

internal sealed class ApiKeyProvider : IApiKeyProvider
{
    private const string KeychainService = "lastfm-api-key";

    public string GetLastFmApiKey()
    {
        var key = TryGetApiKeyFromKeychain();
        if (!string.IsNullOrWhiteSpace(key))
            return key.Trim();

        return (Environment.GetEnvironmentVariable("LASTFM_API_KEY") ?? string.Empty).Trim();
    }

    private static string? TryGetApiKeyFromKeychain()
    {
        try
        {
            var user = Environment.UserName;
            var output = ProcessHelpers.RunProcessCaptureStdout(
                "security",
                ["find-generic-password", "-a", user, "-s", KeychainService, "-w"]);

            var key = output.Trim();
            return string.IsNullOrWhiteSpace(key) ? null : key;
        }
        catch
        {
            return null;
        }
    }
}
