using DSAExperimentation.Benchmarks.Baseline;

namespace DSAExperimentation.Benchmarks.Tests.Baseline;

// The command end to end, from arguments to exit code, over reports that already exist.
//
// The exit code is the part worth testing hardest: it is the only thing a caller can act on,
// and the one failure that matters is a comparison which could not be made reporting success.
// Every refusal below therefore asserts the code rather than the message, and the message only
// to say which refusal it was.
public sealed partial class BaselineCommandTests
{
    private const int Success = 0;
    private const int Regressed = 1;
    private const int Refused = 2;

    private const string Slow = "DSAExperimentation.Benchmarks.StrategySwaps.ReduceOrderBenchmarks.Recursive(Size: 10000)";
    private const string Fast = "DSAExperimentation.Benchmarks.StrategySwaps.ReduceOrderBenchmarks.Iterative(Size: 10000)";
    private const double OneMillisecond = 1_000_000;

    [Fact]
    public void Handles_ClaimsItsOwnVerbAndNothingElse()
    {
        Assert.True(BaselineCommand.Handles(["baseline", "record"]));
        Assert.False(BaselineCommand.Handles(["--filter", "*Sort*"]));
        Assert.False(BaselineCommand.Handles([]));
    }

    [Fact]
    public void Run_AnUnusableCommandLine_RefusesAndShowsTheUsage()
    {
        var (code, _, error) = Run("baseline", "record", "--tolerance", "0.5");

        Assert.Equal(Refused, code);
        Assert.Contains("usage:", error, StringComparison.Ordinal);
    }

    [Fact]
    public void Run_CompareWithNoBaselineRecorded_RefusesRatherThanPasses()
    {
        using var scratch = new ScratchDirectory();
        var report = scratch.Write("first-report-full.json", ReportText.Of((Slow, OneMillisecond, 64)));

        var (code, _, error) = Run(
            "baseline", "compare", "--report", report, "--baseline", scratch.At("absent.tsv"));

        Assert.Equal(Refused, code);
        Assert.Contains("record", error, StringComparison.Ordinal);
    }

    [Fact]
    public void Run_RecordThenCompareTheSameReport_AgreesWithItself()
    {
        using var scratch = new ScratchDirectory();
        var report = scratch.Write("first-report-full.json", ReportText.Of((Slow, OneMillisecond, 64), (Fast, 2_000, 64)));
        var baseline = scratch.At("baseline.tsv");

        var (recorded, recordedOutput, _) = Run("baseline", "record", "--report", report, "--baseline", baseline);
        var (compared, comparedOutput, _) = Run("baseline", "compare", "--report", report, "--baseline", baseline);

        Assert.Equal(Success, recorded);
        Assert.Contains("recorded 2 benchmarks", recordedOutput, StringComparison.Ordinal);
        Assert.True(File.Exists(baseline));
        Assert.Equal(Success, compared);
        Assert.Contains("no regression", comparedOutput, StringComparison.Ordinal);
    }

    // The one the whole mechanism exists for: an arm made three times slower, caught by
    // comparing a fresh report against the recorded baseline, and named.
    [Fact]
    public void Run_Compare_ADeliberatelySlowedArm_IsCaughtAndNamed()
    {
        using var scratch = new ScratchDirectory();
        var baseline = scratch.At("baseline.tsv");
        var before = scratch.Write("before-report-full.json", ReportText.Of((Slow, OneMillisecond, 64), (Fast, 2_000, 64)));
        var after = scratch.Write("after-report-full.json", ReportText.Of((Slow, 3 * OneMillisecond, 64), (Fast, 2_000, 64)));

        Assert.Equal(Success, Run("baseline", "record", "--report", before, "--baseline", baseline).Code);

        var (code, output, _) = Run("baseline", "compare", "--report", after, "--baseline", baseline);

        Assert.Equal(Regressed, code);
        Assert.Contains("REGRESSED", output, StringComparison.Ordinal);
        Assert.Contains(Slow, output, StringComparison.Ordinal);
        Assert.DoesNotContain($"  {Fast}", output, StringComparison.Ordinal);
    }

    // A smoke run recorded by accident, then compared against a real one. The numbers would
    // have been fine to look at and wrong to believe, so the job is what is checked first.
    [Fact]
    public void Run_Compare_AReportFromADifferentJob_RefusesRatherThanCompares()
    {
        using var scratch = new ScratchDirectory();
        var baseline = scratch.At("baseline.tsv");
        var pinned = scratch.Write("pinned-report-full.json", ReportText.Of((Slow, OneMillisecond, 64)));
        var dry = scratch.Write("dry-report-full.json", ReportText.WithDisplayJob("Dry", (Slow, OneMillisecond, 64)));

        Assert.Equal(Success, Run("baseline", "record", "--report", pinned, "--baseline", baseline).Code);

        var (code, output, _) = Run("baseline", "compare", "--report", dry, "--baseline", baseline);

        Assert.Equal(Refused, code);
        Assert.Contains("NO VERDICT", output, StringComparison.Ordinal);
    }

    [Fact]
    public void Run_Compare_ABaselineArmMissingFromTheRun_RefusesRatherThanCompares()
    {
        using var scratch = new ScratchDirectory();
        var baseline = scratch.At("baseline.tsv");
        var both = scratch.Write("both-report-full.json", ReportText.Of((Slow, OneMillisecond, 64), (Fast, 2_000, 64)));
        var one = scratch.Write("one-report-full.json", ReportText.Of((Slow, OneMillisecond, 64)));

        Assert.Equal(Success, Run("baseline", "record", "--report", both, "--baseline", baseline).Code);

        var (code, output, _) = Run("baseline", "compare", "--report", one, "--baseline", baseline);

        Assert.Equal(Refused, code);
        Assert.Contains(Fast, output, StringComparison.Ordinal);
        Assert.Contains("not comparable", output, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Run_Compare_WithALooseTolerance_IsSilentAboutNoise()
    {
        using var scratch = new ScratchDirectory();
        var baseline = scratch.At("baseline.tsv");
        var before = scratch.Write("before-report-full.json", ReportText.Of((Slow, OneMillisecond, 64)));
        var after = scratch.Write("after-report-full.json", ReportText.Of((Slow, OneMillisecond * 1.4, 64)));

        Assert.Equal(Success, Run("baseline", "record", "--report", before, "--baseline", baseline).Code);

        Assert.Equal(Regressed, Run("baseline", "compare", "--report", after, "--baseline", baseline).Code);
        Assert.Equal(Success, Run("baseline", "compare", "--tolerance", "0.5", "--report", after, "--baseline", baseline).Code);
    }

    // A directory is a run: BenchmarkDotNet writes one report per benchmark class, so a
    // comparison of anything larger than one class is always a comparison of several files.
    [Fact]
    public void Run_Record_TakesARunOfSeveralReportFilesAsOneRun()
    {
        using var scratch = new ScratchDirectory();
        var reports = Path.Combine(scratch.Root, "results");
        _ = Directory.CreateDirectory(reports);
        _ = scratch.Write(Path.Combine("results", "one-report-full.json"), ReportText.Of((Slow, OneMillisecond, 64)));
        _ = scratch.Write(Path.Combine("results", "two-report-full.json"), ReportText.Of((Fast, 2_000, 64)));

        var baseline = scratch.At("baseline.tsv");

        var (code, output, _) = Run("baseline", "record", "--report", reports, "--baseline", baseline);

        Assert.Equal(Success, code);
        Assert.Contains("recorded 2 benchmarks", output, StringComparison.Ordinal);
    }

    private static (int Code, string Output, string Error) Run(params string[] args)
    {
        using var output = new StringWriter();
        using var error = new StringWriter();

        var code = BaselineCommand.Run(args, output, error);

        return (code, output.ToString(), error.ToString());
    }
}
