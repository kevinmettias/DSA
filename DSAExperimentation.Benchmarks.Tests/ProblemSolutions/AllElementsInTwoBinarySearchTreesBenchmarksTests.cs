using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for AllElementsInTwoBinarySearchTreesBenchmarks (ARCHITECTURE 17.9): the generic check holds
// the arms to each other, and this pins the baseline's answer to the shape the workload fixes. Setup grows both
// trees from a shuffled 0..n-1 permutation under two seeds, so each tree holds every value once and the merged
// answer is ascending with two values per node count; both facts are asserted directly, since arms that agree
// could still agree on a wrongly shaped answer.
public sealed partial class AllElementsInTwoBinarySearchTreesBenchmarksTests
{
    // The smaller of Setup's [Params(300, 5_000)] node counts.
    private const int SmallestNodeCount = 300;

    // Each tree holds its own permutation of 0..NodeCount-1, so the merged answer carries two
    // values per node count.
    private const int ExpectedValueCount = 2 * SmallestNodeCount;

    [Fact]
    public void CollectAllThenSort_ThreeHundredNodeTrees_ReturnsBothTreesValuesInAscendingOrder()
    {
        var answer = BuildHarness().CollectAllThenSort();

        Assert.Equal(ExpectedValueCount, answer.Length);
        Assert.True(IsAscending(answer));
    }

    private static AllElementsInTwoBinarySearchTreesBenchmarks BuildHarness()
    {
        var harness = new AllElementsInTwoBinarySearchTreesBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }

    // Setup builds both trees over the same shuffled 0..NodeCount-1 permutation under two
    // different seeds, so both hold the same distinct values and the merged answer carries each
    // one twice: ascending here means non-decreasing, not strictly increasing. An out-of-order
    // pair is still wrong regardless of what the other arm says.
    private static bool IsAscending(int[] values) =>
        values.Zip(values.Skip(1)).All(pair => pair.First <= pair.Second);
}
