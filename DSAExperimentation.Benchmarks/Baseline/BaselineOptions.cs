using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace DSAExperimentation.Benchmarks.Baseline;

// The command line of `baseline record` and `baseline compare`, parsed once and then
// answered as one value.
//
// Every option is validated here rather than at the point of use, including the ones
// whose absence is legal, so that the rest of the command never has to ask whether a
// default was written down or merely left out. An option that cannot mean anything for
// the mode it was given to is refused instead of ignored: silently accepting `--tolerance`
// on record would tell the caller it had loosened a comparison that record never makes.
internal sealed record BaselineOptions(
    string Mode,
    string Filter,
    string? Job,
    string? Report,
    string? Baseline,
    double Tolerance)
{
    public const string RecordMode = "record";
    public const string CompareMode = "compare";
    public const string AnyFilter = "*";
    public const double DefaultTolerance = 0.10;

    private const string FilterOption = "--filter";
    private const string JobOption = "--job";
    private const string ReportOption = "--report";
    private const string BaselineOption = "--baseline";
    private const string ToleranceOption = "--tolerance";

    public bool Recording => string.Equals(Mode, RecordMode, StringComparison.Ordinal);

    public static bool TryParse(
        string[] args,
        [NotNullWhen(true)] out BaselineOptions? options,
        [NotNullWhen(false)] out string? problem)
    {
        options = null;
        problem = null;

        if (args.Length < 2)
        {
            problem = "baseline needs a mode: record or compare.";

            return false;
        }

        var mode = args[1];

        if (mode is not (RecordMode or CompareMode))
        {
            problem = $"'{mode}' is not a baseline mode. Expected {RecordMode} or {CompareMode}.";

            return false;
        }

        var filter = AnyFilter;
        var tolerance = DefaultTolerance;
        string? job = null;
        string? report = null;
        string? baseline = null;

        for (var index = 2; index < args.Length; index++)
        {
            var (name, inlineValue) = Split(args[index]);

            if (!IsKnownOption(name, mode))
            {
                problem = $"'{name}' is not an option of baseline {mode}.";

                return false;
            }

            if (!TryReadValue(args, ref index, name, inlineValue, out var value))
            {
                problem = $"'{name}' needs a value, next to it or after it.";

                return false;
            }

            switch (name)
            {
                case FilterOption:
                    filter = value;
                    break;
                case JobOption:
                    job = value;
                    break;
                case ReportOption:
                    report = value;
                    break;
                case BaselineOption:
                    baseline = value;
                    break;
                case ToleranceOption when !double.TryParse(
                    value, NumberStyles.Float, CultureInfo.InvariantCulture, out tolerance):
                    problem = $"'{value}' is not a tolerance. Expected a ratio, such as 0.10 for ten per cent.";

                    return false;
                default:
                    break;
            }
        }

        if (tolerance < 0)
        {
            problem = $"A tolerance of {tolerance} would report every improvement as a regression.";

            return false;
        }

        options = new BaselineOptions(mode, filter, job, report, baseline, tolerance);

        return true;
    }

    // `--filter` and `--job` are the two BenchmarkDotNet would accept itself, and the rest
    // are this command's. `--baseline` is read by compare and written by record: one name,
    // because a second one for the same path would be a second thing to explain.
    private static bool IsKnownOption(string name, string mode)
    {
        if (name is FilterOption or JobOption or ReportOption or BaselineOption)
        {
            return true;
        }

        return string.Equals(name, ToleranceOption, StringComparison.Ordinal)
            && string.Equals(mode, CompareMode, StringComparison.Ordinal);
    }

    // "--filter=*Sort*" and "--filter *Sort*" both arrive here, because BenchmarkDotNet
    // accepts both spellings and a caller who has learnt one from it will type that one.
    private static (string Name, string? InlineValue) Split(string argument)
    {
        if (!argument.StartsWith("--", StringComparison.Ordinal))
        {
            return (argument, null);
        }

        var separator = argument.IndexOf('=');

        return separator < 0
            ? (argument, null)
            : (argument[..separator], argument[(separator + 1)..]);
    }

    private static bool TryReadValue(
        string[] args,
        ref int index,
        string name,
        string? inlineValue,
        [NotNullWhen(true)] out string? value)
    {
        if (inlineValue is not null)
        {
            value = inlineValue;

            return true;
        }

        if (index + 1 >= args.Length)
        {
            value = null;

            return false;
        }

        index++;
        value = args[index];

        return true;
    }
}
