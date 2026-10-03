namespace DSAExperimentation.Benchmarks.Tests;

// Coverage for BenchmarkConfig, and specifically for the one decision in it that is invisible
// when it goes wrong: whether the pinned recording job applies. BenchmarkDotNet treats a
// command-line --job as an additional job rather than a replacement, so if the check below ever
// stopped recognising a spelling, `--job dry` would run the dry job and the full recording job
// back to back - and the only symptom would be a smoke run that took an hour.
public sealed partial class BenchmarkConfigTests
{
    private const int ExpectedWarmupCount = 6;
    private const int ExpectedIterationCount = 15;
    private const int ExpectedLaunchCount = 1;

    [Fact]
    public void For_NoJobArgument_YieldsThePinnedRecordingJob()
    {
        var job = Assert.Single(BenchmarkConfig.For([]).GetJobs());

        Assert.Equal(ExpectedWarmupCount, job.Run.WarmupCount);
        Assert.Equal(ExpectedIterationCount, job.Run.IterationCount);
        Assert.Equal(ExpectedLaunchCount, job.Run.LaunchCount);
    }

    // All three spellings, because BenchmarkDotNet accepts all three and one it did not recognise
    // would look exactly like a harness that is merely slow.
    [Theory]
    [InlineData("--job", "dry")]
    [InlineData("--job=dry", null)]
    [InlineData("-j", "dry")]
    public void For_JobAskedOnTheCommandLine_YieldsNoJobOfItsOwn(string jobArgument, string? jobValue)
    {
        var args = jobValue is null ? new[] { jobArgument } : [jobArgument, jobValue];

        Assert.Empty(BenchmarkConfig.For(args).GetJobs());
    }

    // Without the discoverer every benchmark has no category at all and `--anyCategories` selects
    // nothing, which looks like an empty run rather than a missing hook.
    [Fact]
    public void For_AnyArguments_CategorisesWithTheLibraryDiscoverer() =>
        Assert.Same(LibraryCategoryDiscoverer.Instance, BenchmarkConfig.For([]).CategoryDiscoverer);
}
