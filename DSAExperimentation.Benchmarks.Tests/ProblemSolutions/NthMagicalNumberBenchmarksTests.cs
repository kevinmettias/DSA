using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for NthMagicalNumberBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - counting candidates one at a time up to the answer against
// BinarySearch.LowerBound over the monotone "count(x) >= rank" sequence - so a harness whose arms
// disagree is reporting two different ranks. The class carries no [GlobalSetup]: Rank and the two
// factors are the whole workload, passed straight through to both arms, so the same Rank must answer
// both.
//
// The factors share a factor (6 and 10), which is what keeps the inclusion-exclusion term the binary
// search's counting predicate depends on from collapsing to zero. Both arms return an int, so they
// are compared directly.
public sealed partial class NthMagicalNumberBenchmarksTests
{
    private const int SmallestRank = 2_000;

    [Fact]
    public void BruteForceCount_AgreesWithBinarySearchOnCount()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BinarySearchOnCount(), harness.BruteForceCount());
    }

    [Fact]
    public void BinarySearchOnCount_AgreesWithBruteForceCount()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForceCount(), harness.BinarySearchOnCount());
    }

    private static NthMagicalNumberBenchmarks BuildHarness() => new() { Rank = SmallestRank };
}
