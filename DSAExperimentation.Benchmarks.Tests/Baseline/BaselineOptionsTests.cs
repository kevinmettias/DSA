using DSAExperimentation.Benchmarks.Baseline;

namespace DSAExperimentation.Benchmarks.Tests.Baseline;

// The command line, and mainly the refusals. An option that is accepted by the mode it
// cannot affect is worse than one that is rejected, because the caller goes on believing
// they changed something - `--tolerance` on record being the one that would cost a whole
// re-recorded baseline before anyone noticed.
public sealed partial class BaselineOptionsTests
{
    [Fact]
    public void TryParse_Record_TakesTheRunItShouldAndLeavesTheRestDefault()
    {
        var options = Parsed("baseline", "record", "--filter", "*Sort*");

        Assert.True(options.Recording);
        Assert.Equal("*Sort*", options.Filter);
        Assert.Equal(BaselineOptions.DefaultTolerance, options.Tolerance);
        Assert.Null(options.Job);
        Assert.Null(options.Report);
        Assert.Null(options.Baseline);
    }

    [Fact]
    public void TryParse_Record_WithoutAFilter_RunsEverything()
    {
        Assert.Equal(BaselineOptions.AnyFilter, Parsed("baseline", "record").Filter);
    }

    [Fact]
    public void TryParse_Compare_TakesEveryOption()
    {
        var options = Parsed(
            "baseline", "compare",
            "--filter", "*Sort*",
            "--job", "dry",
            "--report", "reports",
            "--baseline", "baseline.tsv",
            "--tolerance", "0.25");

        Assert.False(options.Recording);
        Assert.Equal("dry", options.Job);
        Assert.Equal("reports", options.Report);
        Assert.Equal("baseline.tsv", options.Baseline);
        Assert.Equal(0.25, options.Tolerance);
    }

    // Both spellings, because BenchmarkDotNet accepts both and a caller who has learnt one
    // from it will type that one here.
    [Fact]
    public void TryParse_EqualsSpelling_ReadsTheSameValue()
    {
        Assert.Equal("*Sort*", Parsed("baseline", "record", "--filter=*Sort*").Filter);
    }

    [Fact]
    public void TryParse_ToleranceOnRecord_IsRefused() =>
        Assert.Contains("--tolerance", Refused("baseline", "record", "--tolerance", "0.5"), StringComparison.Ordinal);

    [Fact]
    public void TryParse_UnknownOption_IsRefused() =>
        Assert.Contains("--depth", Refused("baseline", "record", "--depth", "3"), StringComparison.Ordinal);

    [Fact]
    public void TryParse_OptionWithNoValue_IsRefused() =>
        Assert.Contains("--filter", Refused("baseline", "record", "--filter"), StringComparison.Ordinal);

    [Fact]
    public void TryParse_UnknownMode_IsRefused() =>
        Assert.Contains("record or compare", Refused("baseline", "report"), StringComparison.Ordinal);

    [Fact]
    public void TryParse_NoMode_IsRefused() =>
        Assert.Contains("record or compare", Refused("baseline"), StringComparison.Ordinal);

    [Fact]
    public void TryParse_NonNumericTolerance_IsRefused() =>
        Assert.Contains("0.10", Refused("baseline", "compare", "--tolerance", "ten"), StringComparison.Ordinal);

    // A negative tolerance would put the improvement threshold above the regression one, so
    // every arm would be reported both ways at once.
    [Fact]
    public void TryParse_NegativeTolerance_IsRefused() =>
        Assert.Contains("improvement", Refused("baseline", "compare", "--tolerance", "-0.5"), StringComparison.Ordinal);

    private static BaselineOptions Parsed(params string[] args)
    {
        Assert.True(BaselineOptions.TryParse(args, out var options, out var problem), problem ?? "refused");

        return options;
    }

    private static string Refused(params string[] args)
    {
        Assert.False(BaselineOptions.TryParse(args, out var options, out var problem));
        Assert.Null(options);

        return problem ?? throw new InvalidOperationException("A refusal has to say what was wrong.");
    }
}
