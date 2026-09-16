using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumCostToConnectTwoGroupsOfPointsBenchmarks (ARCHITECTURE 17.9): its two
// arms are competing strategies for the same question - the textbook unmemoized
// (index, connectedMask) recursion against the same recurrence routed through this repo's Memoizer -
// so a harness whose arms disagree is timing two different recurrences. Setup draws the cost matrix
// from one seeded stream, so the same GroupSize must rebuild the same costs.
public sealed partial class MinimumCostToConnectTwoGroupsOfPointsBenchmarksTests
{
    private const int SmallestGroupSize = 4;

    [Fact]
    public void Setup_SameGroupSize_RebuildsTheSameCostMatrix() =>
        Assert.Equal(BuildHarness().BruteForceRecursion(), BuildHarness().BruteForceRecursion());

    [Fact]
    public void BruteForceRecursion_RandomCostMatrix_AgreesWithMemoizedRecursion()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MemoizedRecursion(), harness.BruteForceRecursion());
    }

    [Fact]
    public void MemoizedRecursion_RandomCostMatrix_AgreesWithBruteForceRecursion()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForceRecursion(), harness.MemoizedRecursion());
    }

    private static MinimumCostToConnectTwoGroupsOfPointsBenchmarks BuildHarness()
    {
        var harness = new MinimumCostToConnectTwoGroupsOfPointsBenchmarks { GroupSize = SmallestGroupSize };
        harness.Setup();

        return harness;
    }
}
