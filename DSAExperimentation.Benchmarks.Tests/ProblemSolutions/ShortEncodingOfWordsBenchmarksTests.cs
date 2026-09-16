using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ShortEncodingOfWordsBenchmarks (ARCHITECTURE 17.9): all three arms answer the
// same minimum-encoding-length question about the same word list, so a harness whose arms disagree is
// timing two different lists. Setup builds the list from one fixed seed over a small shared-suffix
// pool, so real suffix redundancy exists for all three strategies to exploit and the same Length
// must rebuild the same list; none of the arms writes to it, so one harness instance is safe to call
// twice in either order.
public sealed partial class ShortEncodingOfWordsBenchmarksTests
{
    private const int SmallestLength = 500;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().PairwiseSuffixScan(), BuildHarness().PairwiseSuffixScan());

    [Fact]
    public void PairwiseSuffixScan_SharedSuffixPool_AgreesWithSuffixEviction()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.SuffixEviction(), harness.PairwiseSuffixScan());
    }

    [Fact]
    public void SuffixEviction_SharedSuffixPool_AgreesWithReversedTrieLeaves()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ReversedTrieLeaves(), harness.SuffixEviction());
    }

    [Fact]
    public void ReversedTrieLeaves_SharedSuffixPool_AgreesWithPairwiseSuffixScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.PairwiseSuffixScan(), harness.ReversedTrieLeaves());
    }

    private static ShortEncodingOfWordsBenchmarks BuildHarness()
    {
        var harness = new ShortEncodingOfWordsBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
