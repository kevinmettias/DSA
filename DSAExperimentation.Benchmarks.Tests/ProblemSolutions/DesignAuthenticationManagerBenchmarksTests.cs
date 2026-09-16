using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DesignAuthenticationManagerBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - a flat List<T> renew probe walks against this
// repo's own HashMap - so a harness whose arms disagree is timing two different problems. Setup
// derives every token id and renew probe from Length, so the same Length must rebuild the same
// script, and that script has to retire all but the newest TimeToLive tokens by the time it counts.
public sealed partial class DesignAuthenticationManagerBenchmarksTests
{
    private const int SmallestLength = 200;

    // Mirrors the benchmark's own time-to-live: the reading is a count of the tokens still inside
    // it, so the test has to state the window it is checking.
    private const int TimeToLive = 100;

    // Token i expires at i + TimeToLive strictly after the count's time, so the survivors of the
    // window are the ids that start past Length - TimeToLive.
    private const int ExpectedSurvivorCount = SmallestLength - TimeToLive - 1;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameRenewScript()
    {
        // Token i is generated at time i, so it expires at i + TimeToLive and only the strict
        // survivors of that window are left when the replay counts at time Length. Half the probes
        // name an unknown id and the other half name an existing but long-expired one, so neither
        // renew path may revive anything: exactly Length - TimeToLive - 1 tokens survive.
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
