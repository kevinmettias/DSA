using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DesignHashMapBenchmarks (ARCHITECTURE 17.9): the generic arm check compares
// the arms only with each other, so what stays here is a count derived from the workload's
// construction rather than from either arm: the workload has to split its probes evenly between hits
// and misses.
public sealed partial class DesignHashMapBenchmarksTests
{
    private const int SmallestLength = 200;

    // Mirrors the harness's own alternation modulus: the probes alternate between a populated key and
    // an absent one, so the hits are this share of the length.
    private const int HitShareDivisor = 2;

    // Keys 0..Length-1 are all populated with their own value, and the probes alternate between one of
    // those keys and one past the populated range - which has to report the absent-key sentinel rather
    // than a value, since every value stored equals its own key and so is never negative. Half of
    // SmallestLength hits is what both halves together allow.
    [Fact]
    public void LinearScanList_HalfHitHalfMissProbes_FindsExactlyTheHits() =>
        Assert.Equal(SmallestLength / HitShareDivisor, BuildHarness().LinearScanList());

    private static DesignHashMapBenchmarks BuildHarness()
    {
        var harness = new DesignHashMapBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
