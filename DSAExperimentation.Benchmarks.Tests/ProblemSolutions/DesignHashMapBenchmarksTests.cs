using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DesignHashMapBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - a flat list scanned for the key against this repo's own
// HashMap - so a harness whose arms disagree is timing two different problems. Setup derives both
// the populated keys and the probe keys from Length, so the same Length must rebuild the same
// workload, and that workload has to split its probes evenly between hits and misses.
public sealed partial class DesignHashMapBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameKeyAndProbeWorkload()
    {
        // Keys 0..Length-1 are all populated with their own value, and the probes alternate
        // between one of those keys and one past the populated range - which has to report the
        // absent-key sentinel rather than a value, since every value stored equals its own key and
        // so is never negative. Half of SmallestLength hits is what both halves together allow.
        Assert.Equal(SmallestLength / 2, BuildHarness().LinearScanList());
        Assert.Equal(BuildHarness().LinearScanList(), BuildHarness().LinearScanList());
    }

    [Fact]
    public void LinearScanList_HalfHitHalfMissProbes_AgreesWithHashMapBacked()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.HashMapBacked(), harness.LinearScanList());
    }

    [Fact]
    public void HashMapBacked_HalfHitHalfMissProbes_AgreesWithLinearScanList()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.LinearScanList(), harness.HashMapBacked());
    }

    private static DesignHashMapBenchmarks BuildHarness()
    {
        var harness = new DesignHashMapBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
