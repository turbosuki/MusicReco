namespace MusicReco.Cli;

internal static class ArgParsing
{
    public static int GetIntArg(string[] args, string name, int fallback, int min, int max)
    {
        for (var i = 0; i < args.Length - 1; i++)
        {
            if (string.Equals(args[i], name, StringComparison.OrdinalIgnoreCase))
            {
                if (int.TryParse(args[i + 1], out var v))
                    return Math.Clamp(v, min, max);
            }
        }

        return fallback;
    }
}