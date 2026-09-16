using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaximumNumberOfGroupsWithIncreasingLengthBenchmarks (ARCHITECTURE 17.9):
// both arms are MaximumNumberOfGroupsWithIncreasingLengthSolution's competing strategies for one
// question - an in-place insertion sort against this repo's MergeSort, in front of the identical
// shared greedy sweep - so a harness whose arms disagree is timing two different problems. Both
// answer with a single group count, compared directly.
public sealed partial class MaximumNumberOfGroupsWithIncreasingLengthBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameUsageLimits()
    {
        var first = BuildHarness();
        var second = BuildHarness();

        // The usage limits are private, so the rebuild is pinned through the count they produce:
        // the same Length must draw the same seeded limits and sweep them identically.
        Assert.Equal(first.InsertionSortThenGreedySweep(), second.InsertionSortThenGreedySweep());
        Assert.Equal(first.MergeSortThenGreedySweep(), second.MergeSortThenGreedySweep());
    }

    [Fact]
    public void InsertionSortThenGreedySweep_SeededUsageLimits_AgreesWithMergeSortThenGreedySweep()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MergeSortThenGreedySweep(), harness.InsertionSortThenGreedySweep());
    }

    [Fact]
    public void MergeSortThenGreedySweep_SeededUsageLimits_AgreesWithInsertionSortThenGreedySweep()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.InsertionSortThenGreedySweep(), harness.MergeSortThenGreedySweep());
    }

    private static MaximumNumberOfGroupsWithIncreasingLengthBenchmarks BuildHarness()
    {
        var harness = new MaximumNumberOfGroupsWithIncreasingLengthBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
