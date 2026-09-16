using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumXorSumOfTwoArraysBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - the textbook unmemoized bitmask recursion against the same recurrence
// routed through this repo's own Memoizer - so a harness whose arms disagree is timing two different
// problems. Both arms only read the two generated arrays, so one harness instance is safe to call twice in
// either order. Setup draws both arrays from one fixed seed, so the same Length must rebuild the same pair
// of arrays; otherwise two published numbers were never comparable in the first place.
public sealed partial class MinimumXorSumOfTwoArraysBenchmarksTests
{
    private const int SmallestLength = 4;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameArrays() =>
        Assert.Equal(BuildHarness().BruteForceRecursion(), BuildHarness().BruteForceRecursion());

    [Fact]
    public void BruteForceRecursion_SeededNumberPairs_AgreesWithMemoizedRecursion()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MemoizedRecursion(), harness.BruteForceRecursion());
    }

    [Fact]
    public void MemoizedRecursion_SeededNumberPairs_AgreesWithBruteForceRecursion()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForceRecursion(), harness.MemoizedRecursion());
    }

    private static MinimumXorSumOfTwoArraysBenchmarks BuildHarness()
    {
        var harness = new MinimumXorSumOfTwoArraysBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
