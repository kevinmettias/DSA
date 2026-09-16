using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountTheNumberOfSquareFreeSubsetsBenchmarks (ARCHITECTURE 17.9): its two arms
// are competing strategies for the same question - enumerating all 2^n subsets against the bitmask
// memo over the 18 square-free values of [1, 30] - so a harness whose arms disagree is timing two
// different problems, not two ways of answering one. Setup draws the array from one fixed seed, so
// the same Length must rebuild the same array; otherwise two published numbers were never comparable
// in the first place.
//
// The generated array is private and the subset count is the only thing either arm reports, so the
// documented shape is asserted through that: an array of Length values has 2^Length subsets, and the
// empty subset the problem excludes is the one that is never counted.
public sealed partial class CountTheNumberOfSquareFreeSubsetsBenchmarksTests
{
    private const int SmallestLength = 15;

    private const long NonEmptySubsetCount = (1L << SmallestLength) - 1;

    // The problem counts non-empty subsets, and the seeded array holds values in [1, 30) that
    // are square-free, so at least one subset qualifies.
    private const long FewestSquareFreeSubsets = 1;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameArray()
    {
        Assert.InRange(BuildHarness().BruteForce(), FewestSquareFreeSubsets, NonEmptySubsetCount);
        Assert.Equal(BuildHarness().BruteForce(), BuildHarness().BruteForce());
    }

    [Fact]
    public void BruteForce_ValuesInOneToThirty_AgreesWithBitmaskMemo()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BitmaskMemo(), harness.BruteForce());
    }

    [Fact]
    public void BitmaskMemo_ValuesInOneToThirty_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForce(), harness.BitmaskMemo());
    }

    private static CountTheNumberOfSquareFreeSubsetsBenchmarks BuildHarness()
    {
        var harness = new CountTheNumberOfSquareFreeSubsetsBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
