namespace MusicReco;

internal sealed class CliOptions
{
    public int Count { get; }
    public int Bias { get; }
    public bool Open { get; }

    private CliOptions(int count, int bias, bool open)
    {
        Count = count;
        Bias = bias;
        Open = open;
    }

    public static CliOptions Parse(string[] args)
    {
        var count = ArgParsing.GetIntArg(args, "--count", 5, min: 1, max: 20);
        var bias = ArgParsing.GetIntArg(args, "--bias", 5, min: 1, max: 20);
        var open = args.Contains("--open", StringComparer.OrdinalIgnoreCase);

        return new CliOptions(count, bias, open);
    }
}