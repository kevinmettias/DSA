using DSAExperimentation.Benchmarks.Baseline;

namespace DSAExperimentation.Benchmarks.Tests.Baseline;

// The reader, the writer and the comparison, over hand-written reports. Every case here is
// one of the ways a comparison can answer the wrong thing: a job that drifted, an arm that
// stopped being measured, an allocation whose absence was read as a zero, or a ratio small
// enough to be the machine rather than the code.
public sealed partial class BenchmarkBaselineTests
{
    private const string Slow = "DSAExperimentation.Benchmarks.StrategySwaps.ReduceOrderBenchmarks.Recursive(Size: 10000)";
    private const string Fast = "DSAExperimentation.Benchmarks.StrategySwaps.ReduceOrderBenchmarks.Iterative(Size: 10000)";
    private const double OneMillisecond = 1_000_000;
    private const double Tolerance = 0.10;
    private static readonly DateOnly CapturedOn = new(2026, 10, 1);

    [Fact]
    public void Read_ReportWithTwoArms_YieldsBothWithTheirNumbers()
    {
        var run = BenchmarkBaseline.Read(ReportText.Of((Slow, OneMillisecond, 64), (Fast, 2_000, 0)));

        Assert.Equal(ReportText.PinnedJob, run.Job);
        Assert.Equal("Test Processor | .NET 10.0.0", run.Host);
        Assert.Equal(
            [new BenchmarkReading(Slow, OneMillisecond, 64), new BenchmarkReading(Fast, 2_000, 0)],
            run.Readings);
    }

    // The two halves of the same claim: a report with no memory section cannot say an arm
    // allocated nothing, and reading it as zero would turn every missing diagnoser into an
    // improvement.
    [Fact]
    public void Read_ReportWithoutMemory_LeavesAllocationUnknown()
    {
        var run = BenchmarkBaseline.Read(ReportText.Of((Slow, OneMillisecond, null)));

        Assert.Null(Assert.Single(run.Readings).AllocatedBytes);
    }

    [Fact]
    public void Read_TwoRunsOfTheSameJob_AgreeOnItDespiteTheGeneratedHash()
    {
        var first = BenchmarkBaseline.Read(ReportText.Of((Slow, OneMillisecond, 64)));
        var second = BenchmarkBaseline.Read(ReportText.WithRehashedJob(ReportText.PinnedJob, (Slow, OneMillisecond, 64)));

        Assert.Equal(first.Job, second.Job);
    }

    [Fact]
    public void Read_BareJobName_IsItsOwnDescription()
    {
        var run = BenchmarkBaseline.Read(ReportText.WithDisplayJob("Dry", (Slow, OneMillisecond, 64)));

        Assert.Equal("Dry", run.Job);
    }

    // Two jobs in one report is what a merged pair of runs arrives as, and a baseline built
    // from one would carry numbers measured two different ways under a single heading.
    [Fact]
    public void Read_ReportMixingJobs_IsRefused()
    {
        var exception = Assert.Throws<InvalidDataException>(() => BenchmarkBaseline.Read(MixedJobReport));

        Assert.Contains("2 jobs", exception.Message, StringComparison.Ordinal);
    }

    // A benchmark that did not run has no statistics rather than a slow one, and recording it
    // as a row would put a number in the baseline that nothing measured.
    [Fact]
    public void Read_ArmWithNoMean_IsRefused()
    {
        var exception = Assert.Throws<InvalidDataException>(() => BenchmarkBaseline.Read(ReportWithoutAMean));

        Assert.Contains(Slow, exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void Record_ThenParse_RoundTripsTheRun()
    {
        var run = BenchmarkBaseline.Read(ReportText.Of((Slow, OneMillisecond, 64), (Fast, 2_000, null)));

        var parsed = BenchmarkBaseline.Parse(BenchmarkBaseline.Record(run, "*ReduceOrder*", CapturedOn));

        Assert.Equal(run.Job, parsed.Job);
        Assert.Equal(run.Host, parsed.Host);
        Assert.Equal(
            run.Readings.OrderBy(reading => reading.Name, StringComparer.Ordinal),
            parsed.Readings.OrderBy(reading => reading.Name, StringComparer.Ordinal));
    }

    // A committed baseline is read in a diff, and a diff is only a diff if the order is a
    // consequence of the names rather than of whichever arm finished first.
    [Fact]
    public void Record_SortsRowsByName()
    {
        var run = BenchmarkBaseline.Read(ReportText.Of((Slow, OneMillisecond, 64), (Fast, 2_000, 64)));

        var rows = Rows(BenchmarkBaseline.Record(run, "*", CapturedOn));

        Assert.Equal([Fast, Slow], rows.Select(row => row[0]));
    }

    [Fact]
    public void Record_HeaderCarriesTheCommandsThatReproduceIt()
    {
        var run = BenchmarkBaseline.Read(ReportText.Of((Slow, OneMillisecond, 64)));

        var text = BenchmarkBaseline.Record(run, "*ReduceOrder*", CapturedOn);

        Assert.Contains("baseline record --filter \"*ReduceOrder*\"", text, StringComparison.Ordinal);
        Assert.Contains("baseline compare --filter \"*ReduceOrder*\"", text, StringComparison.Ordinal);
        Assert.Contains($"# job\t{ReportText.PinnedJob}", text, StringComparison.Ordinal);
        Assert.Contains("# captured\t2026-10-01", text, StringComparison.Ordinal);
    }

    [Fact]
    public void Parse_ProseIsNotReadAsMetadata()
    {
        var run = BenchmarkBaseline.Read(ReportText.Of((Slow, OneMillisecond, 64)));
        var text = BenchmarkBaseline.Record(run, "*", CapturedOn);

        // The header's prose lines all start with '#' and none of them carry a recognised
        // key, so a row parsed out of them would mean the metadata test had gone wrong.
        Assert.Single(Rows(text));
    }

    [Fact]
    public void Parse_BaselineWithNoRows_IsRefused()
    {
        var exception = Assert.Throws<InvalidDataException>(
            () => BenchmarkBaseline.Parse("# job\tDry\n# host\tnowhere\n"));

        Assert.Contains("no rows", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Parse_RowWithAMissingField_NamesTheLine()
    {
        var exception = Assert.Throws<InvalidDataException>(() => BenchmarkBaseline.Parse($"{Slow}\t12.5\n"));

        Assert.Contains("line 1", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void Merge_ReportsFromOneRun_AreOneRunInNameOrder()
    {
        var first = BenchmarkBaseline.Read(ReportText.Of((Slow, OneMillisecond, 64)));
        var second = BenchmarkBaseline.Read(ReportText.Of((Fast, 2_000, 64)));

        var merged = BenchmarkBaseline.Merge([first, second]);

        Assert.Equal([Fast, Slow], merged.Readings.Select(reading => reading.Name));
    }

    [Fact]
    public void Merge_ReportsFromDifferentJobs_AreRefused()
    {
        var pinned = BenchmarkBaseline.Read(ReportText.Of((Slow, OneMillisecond, 64)));
        var dry = BenchmarkBaseline.Read(ReportText.WithDisplayJob("Dry", (Fast, 2_000, 64)));

        Assert.Throws<InvalidDataException>(() => BenchmarkBaseline.Merge([pinned, dry]));
    }

    [Fact]
    public void Merge_TheSameArmTwice_IsRefused()
    {
        var run = BenchmarkBaseline.Read(ReportText.Of((Slow, OneMillisecond, 64)));

        var exception = Assert.Throws<InvalidDataException>(() => BenchmarkBaseline.Merge([run, run]));

        Assert.Contains(Slow, exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void Compare_AnArmThreeTimesSlower_IsARegression()
    {
        var comparison = Compared(
            ReportText.Of((Slow, OneMillisecond, 64)),
            ReportText.Of((Slow, 3 * OneMillisecond, 64)));

        var regression = Assert.Single(comparison.Regressions);

        Assert.Equal(Slow, regression.Name);
        Assert.Equal(3, regression.Ratio, 3);
        Assert.True(comparison.Regressed);
        Assert.True(comparison.IsUsable);
    }

    [Fact]
    public void Compare_AnArmInsideTheTolerance_IsNeither()
    {
        var comparison = Compared(
            ReportText.Of((Slow, OneMillisecond, 64)),
            ReportText.Of((Slow, OneMillisecond * 1.05, 64)));

        Assert.Empty(comparison.Regressions);
        Assert.Empty(comparison.Improvements);
        Assert.Equal(1, comparison.WithinTolerance);
        Assert.False(comparison.Regressed);
    }

    [Fact]
    public void Compare_AnArmFasterByMoreThanTheTolerance_IsAnImprovement()
    {
        var comparison = Compared(
            ReportText.Of((Slow, OneMillisecond, 64)),
            ReportText.Of((Slow, OneMillisecond / 2, 64)));

        Assert.Empty(comparison.Regressions);
        Assert.Single(comparison.Improvements);
    }

    [Fact]
    public void Compare_MoreAllocation_IsARegressionEvenWhenTheTimeImproved()
    {
        var comparison = Compared(
            ReportText.Of((Slow, OneMillisecond, 64)),
            ReportText.Of((Slow, OneMillisecond / 2, 128)));

        var regression = Assert.Single(comparison.Regressions);

        Assert.Equal(64, regression.AllocationChange);
        Assert.Empty(comparison.Improvements);
    }

    // Measured on this repository, not invented: an unmodified arm reported 17,537,358 bytes
    // and then 17,537,486, because BenchmarkDotNet divides the run's total by an operation
    // count it chooses afresh each time. Holding allocation to no tolerance made that a
    // regression on an arm whose time had improved, which is the false alarm this pins down.
    [Fact]
    public void Compare_AllocationMovementWithinTolerance_IsNotARegression()
    {
        var comparison = Compared(
            ReportText.Of((Slow, OneMillisecond, 17_537_358)),
            ReportText.Of((Slow, OneMillisecond * 0.988, 17_537_486)));

        Assert.Empty(comparison.Regressions);
        Assert.Empty(comparison.Improvements);
        Assert.Equal(1, comparison.WithinTolerance);
    }

    [Fact]
    public void Compare_AllocationThatWasNeverMeasured_IsNotReadAsZero()
    {
        var comparison = Compared(
            ReportText.Of((Slow, OneMillisecond, null)),
            ReportText.Of((Slow, OneMillisecond * 1.5, null)));

        var regression = Assert.Single(comparison.Regressions);

        Assert.True(regression.AllocationUnknown);
        Assert.Null(regression.AllocationChange);
    }

    // An arm that allocated nothing per operation and now allocates something is the one
    // allocation change a relative tolerance cannot see, because there is no baseline for
    // it to be relative to.
    [Fact]
    public void Compare_AllocationRisingFromNothing_IsARegression()
    {
        var comparison = Compared(
            ReportText.Of((Slow, OneMillisecond, 0)),
            ReportText.Of((Slow, OneMillisecond, 24)));

        Assert.Single(comparison.Regressions);
    }

    // A baseline missing an arm does not describe this run, and "no regressions" would be the
    // most misleading answer available for it - so it is reported as no verdict at all.
    [Fact]
    public void Compare_AnArmTheBaselineHasAndTheRunDoesNot_MakesTheComparisonUnusable()
    {
        var comparison = Compared(ReportText.Of((Slow, OneMillisecond, 64), (Fast, 2_000, 64)), ReportText.Of((Slow, OneMillisecond, 64)));

        Assert.Equal([Fast], comparison.Missing);
        Assert.False(comparison.DescribesTheRun);
        Assert.False(comparison.IsUsable);
    }

    [Fact]
    public void Compare_AnArmNewerThanTheBaseline_IsAddedAndNotARegression()
    {
        var comparison = Compared(ReportText.Of((Slow, OneMillisecond, 64)), ReportText.Of((Slow, OneMillisecond, 64), (Fast, 2_000, 64)));

        Assert.Equal([Fast], comparison.Added);
        Assert.False(comparison.Regressed);
    }

    [Fact]
    public void Compare_ARunUnderADifferentJob_IsNotComparable()
    {
        var baseline = BenchmarkBaseline.Read(ReportText.Of((Slow, OneMillisecond, 64)));
        var current = BenchmarkBaseline.Read(ReportText.WithDisplayJob("Dry", (Slow, OneMillisecond, 64)));

        var comparison = BenchmarkBaseline.Compare(baseline, current, Tolerance);

        Assert.False(comparison.JobsMatch);
        Assert.False(comparison.IsUsable);
        Assert.False(comparison.Regressed);
    }

    // Every arm of the baseline lands in exactly one bucket. Without this a comparison could
    // stop counting arms - by a reader that skipped a row, say - and still report "no
    // regression" over the ones it never looked at.
    [Fact]
    public void Compare_AccountsForEveryArmOfTheBaseline()
    {
        var baseline = BenchmarkBaseline.Read(ReportText.Of((Slow, OneMillisecond, 64), (Fast, 2_000, 64)));
        var current = BenchmarkBaseline.Read(ReportText.Of((Slow, 5 * OneMillisecond, 64), (Fast, 2_010, 64)));

        var comparison = BenchmarkBaseline.Compare(baseline, current, Tolerance);

        Assert.Equal(
            baseline.Readings.Count,
            comparison.Regressions.Count
            + comparison.Improvements.Count
            + comparison.Missing.Count
            + comparison.WithinTolerance);
    }

    private static BaselineComparison Compared(string baseline, string current) =>
        BenchmarkBaseline.Compare(
            BenchmarkBaseline.Read(baseline),
            BenchmarkBaseline.Read(current),
            Tolerance);

    private static List<string[]> Rows(string baselineText) =>
        [.. baselineText
            .Split('\n')
            .Select(line => line.TrimEnd('\r'))
            .Where(line => line.Length > 0 && !line.StartsWith('#'))
            .Select(line => line.Split('\t'))];

    // Hand-written rather than built through ReportText, because what these two are about is
    // a report that is malformed in a way the builder will not produce.
    private static readonly string MixedJobReport = $$$"""
        {"HostEnvironmentInfo":{},"Benchmarks":[
          {"FullName":"{{{Slow}}}","DisplayInfo":"{{{ReportText.WithoutParameters(Slow)}}}: Job-ABCDEF{{{ReportText.PinnedJob}}}","Statistics":{"Mean":1000000}},
          {"FullName":"{{{Fast}}}","DisplayInfo":"{{{ReportText.WithoutParameters(Fast)}}}: Dry","Statistics":{"Mean":2000}}]}
        """;

    private static readonly string ReportWithoutAMean = $$$"""
        {"HostEnvironmentInfo":{},"Benchmarks":[
          {"FullName":"{{{Slow}}}","DisplayInfo":"{{{ReportText.WithoutParameters(Slow)}}}: Dry"}]}
        """;
}
