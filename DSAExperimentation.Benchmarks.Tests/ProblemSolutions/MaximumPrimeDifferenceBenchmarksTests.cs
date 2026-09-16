using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaximumPrimeDifferenceBenchmarks (ARCHITECTURE 17.9): both arms are
// MaximumPrimeDifferenceSolution's competing strategies for one question - the pairwise scan over
// every prime pair against the two-endpoint scan backed by a sieve - so a harness whose arms
// disagree is timing two different problems. Both answer with a single index distance, compared
// directly.
public sealed partial class MaximumPrimeDifferenceBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameValueArray()
    {
        var first = BuildHarness();
        var second = BuildHarness();

        // The value array is private, so the rebuild is pinned through the distance it produces:
        // the same Length must draw the same seeded values from LeetCode's own [1, 100] range and
        // distance the outer primes identically.
        Assert.Equal(first.BruteForcePairs(), second.BruteForcePairs());
        Assert.Equal(first.EndpointScanWithSieve(), second.EndpointScanWithSieve());
    }

    [Fact]
    public void BruteForcePairs_SeededValueArray_AgreesWithEndpointScanWithSieve()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.EndpointScanWithSieve(), harness.BruteForcePairs());
    }

    [Fact]
    public void EndpointScanWithSieve_SeededValueArray_AgreesWithBruteForcePairs()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForcePairs(), harness.EndpointScanWithSieve());
    }

    private static MaximumPrimeDifferenceBenchmarks BuildHarness()
    {
        var harness = new MaximumPrimeDifferenceBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
