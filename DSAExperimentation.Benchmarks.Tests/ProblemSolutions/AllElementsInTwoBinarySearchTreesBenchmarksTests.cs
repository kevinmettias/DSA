using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for AllElementsInTwoBinarySearchTreesBenchmarks (ARCHITECTURE 17.9): its two arms are
// AllElementsInTwoBinarySearchTreesSolution's competing strategies for the same question - dump both trees and
// sort against collect each tree's ascending run and merge - so a harness whose arms disagree is timing two
// different forests. Setup grows both trees from the same shuffled 0..n-1 permutation under two seeds, so each
// tree holds every value once and the merged answer is strictly ascending with two values per node count; both
// facts are asserted directly, since identical arms could still agree on a wrongly shaped answer.
public sealed partial class AllElementsInTwoBinarySearchTreesBenchmarksTests
{
    // The smaller of Setup's [Params(300, 5_000)] node counts.
    private const int SmallestNodeCount = 300;

    // Each tree holds its own permutation of 0..NodeCount-1, so the merged answer carries two
    // values per node count.
    private const int ExpectedValueCount = 2 * SmallestNodeCount;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().CollectAllThenSort()),
            AnswerText.Of(BuildHarness().CollectAllThenSort()));

    [Fact]
    public void CollectAllThenSort_ThreeHundredNodeTrees_AgreesWithInOrderTraversalMerge()
    {
        var harness = BuildHarness();
        var answer = harness.CollectAllThenSort();

        Assert.Equal(ExpectedValueCount, answer.Length);
        Assert.True(IsAscending(answer));
        Assert.Equal(AnswerText.Of(harness.InOrderTraversalMerge()), AnswerText.Of(answer));
    }

    [Fact]
    public void InOrderTraversalMerge_ThreeHundredNodeTrees_AgreesWithCollectAllThenSort()
    {
        var harness = BuildHarness();
        var answer = harness.InOrderTraversalMerge();

        Assert.Equal(ExpectedValueCount, answer.Length);
        Assert.True(IsAscending(answer));
        Assert.Equal(AnswerText.Of(harness.CollectAllThenSort()), AnswerText.Of(answer));
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
