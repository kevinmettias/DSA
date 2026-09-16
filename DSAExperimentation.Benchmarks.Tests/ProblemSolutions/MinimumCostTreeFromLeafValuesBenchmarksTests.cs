using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumCostTreeFromLeafValuesBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - the un-memoized interval recursion that re-solves every
// subinterval against the O(n) monotonic-decreasing sweep - so a harness whose arms disagree is timing
// two different arrays. Both arms answer with an int minimum cost, which they compare directly. Setup
// draws the leaf values from one seeded stream, so the same Length must rebuild the same array.
public sealed partial class MinimumCostTreeFromLeafValuesBenchmarksTests
{
    private const int SmallestLength = 10;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameArray() =>
        Assert.Equal(BuildHarness().UnmemoizedRecursion(), BuildHarness().UnmemoizedRecursion());

    [Fact]
    public void UnmemoizedRecursion_RandomLeafValues_AgreesWithMonotonicStack()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MonotonicStack(), harness.UnmemoizedRecursion());
    }

    [Fact]
    public void MonotonicStack_RandomLeafValues_AgreesWithUnmemoizedRecursion()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.UnmemoizedRecursion(), harness.MonotonicStack());
    }

    private static MinimumCostTreeFromLeafValuesBenchmarks BuildHarness()
    {
        var harness = new MinimumCostTreeFromLeafValuesBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
