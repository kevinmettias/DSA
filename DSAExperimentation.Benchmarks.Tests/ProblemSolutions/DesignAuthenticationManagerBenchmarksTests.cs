using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DesignAuthenticationManagerBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - a flat List<T> renew probe walks against this
// repo's own HashMap - so a harness whose arms disagree is timing two different problems. Setup
// derives every token id and renew probe from Length, so the same Length must rebuild the same
// script, and that script's clock has to stay inside the time-to-live until it counts.
public sealed partial class DesignAuthenticationManagerBenchmarksTests
{
    private const int SmallestLength = 200;

    // The clock ticks once per call - Length generates, Length renews, then the count - and the
    // benchmark's time-to-live of 2000 outlasts all of them, so every generated token survives.
    private const int ExpectedSurvivorCount = SmallestLength;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameRenewScript()
    {
        // The replay's last call comes at time 2 * Length + 1, still inside every token's time to
        // live. Half the probes name an unknown id, which renews nothing, and the other half renew a
        // live token, which keeps it live: exactly the Length generated tokens survive.
        Assert.Equal(ExpectedSurvivorCount, BuildHarness().LinearScanList());
        Assert.Equal(BuildHarness().LinearScanList(), BuildHarness().LinearScanList());
    }

    [Fact]
    public void LinearScanList_HalfHitHalfMissRenewProbes_AgreesWithRepoHashMap()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.RepoHashMap(), harness.LinearScanList());
    }

    [Fact]
    public void RepoHashMap_HalfHitHalfMissRenewProbes_AgreesWithLinearScanList()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.LinearScanList(), harness.RepoHashMap());
    }

    private static DesignAuthenticationManagerBenchmarks BuildHarness()
    {
        var harness = new DesignAuthenticationManagerBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
