using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for NumberOfSubarraysWithANDValueOfKBenchmarks (ARCHITECTURE 17.9): both arms count
// the subarrays whose AND equals K - the O(n^2) brute force against the compressed distinct-AND-value
// sweep - so a harness whose arms disagree is timing two different questions. The count is the
// problem's whole answer rather than a proxy, and neither arm short-circuits on a match, so the fixed K
// only makes the workload deterministic rather than biasing the comparison. Setup draws nums from a
// fixed seed, so the same Length must rebuild the same array.
public sealed partial class NumberOfSubarraysWithANDValueOfKBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameParameters_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().BruteForce(), BuildHarness().BruteForce());

    [Fact]
    public void BruteForce_AgreesWithAndValueCompression()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.AndValueCompression(), harness.BruteForce());
    }

    [Fact]
    public void AndValueCompression_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForce(), harness.AndValueCompression());
    }

    private static NumberOfSubarraysWithANDValueOfKBenchmarks BuildHarness()
    {
        var harness = new NumberOfSubarraysWithANDValueOfKBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
