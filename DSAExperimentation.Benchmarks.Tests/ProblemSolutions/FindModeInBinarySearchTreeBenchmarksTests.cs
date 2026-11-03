using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.LeetCode.Solutions.Conventions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindModeInBinarySearchTreeBenchmarks (ARCHITECTURE 17.9): the generic arms
// check only compares the arms with each other, so this pins each arm's modes to the set Setup's
// construction fixes independently of any arm.
//
// AnswerGraphText.OfUnordered, not Of, because the outer order here is genuinely unfixed: LC 501
// accepts the modes in any order, the hash arm reports them in its dictionary's iteration order and
// the in-order arm reports them ascending, so the elements are compared as a set while each mode's
// own value is still compared exactly.
//
// Setup's tree holds NodeCount / NodesPerDistinctValue distinct values, each repeated
// NodesPerDistinctValue times, so every one of them ties for the mode and the answer is decisive: the
// set of all distinct values, not a seed-dependent subset.
public sealed partial class FindModeInBinarySearchTreeBenchmarksTests
{
    // The smaller of Setup's [Params(500, 10_000)] node counts.
    private const int SmallestNodeCount = 500;

    // The fixture's own copies per distinct value, restated so the expected mode set is derived
    // rather than observed.
    private const int NodesPerDistinctValue = 20;

    [Fact]
    public void HashMapFrequencyCount_EveryValueTiedForTheMode_ReturnsEveryDistinctValue() =>
        Assert.Equal(AnswerGraphText.OfUnordered(ExpectedModes()), AnswerGraphText.OfUnordered(BuildHarness().HashMapFrequencyCount()));

    [Fact]
    public void InOrderTraversalStreak_EveryValueTiedForTheMode_ReturnsEveryDistinctValue() =>
        Assert.Equal(AnswerGraphText.OfUnordered(ExpectedModes()), AnswerGraphText.OfUnordered(BuildHarness().InOrderTraversalStreak()));

    private static FindModeInBinarySearchTreeBenchmarks BuildHarness()
    {
        var harness = new FindModeInBinarySearchTreeBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }

    // Setup assigns value i % distinctValues to node i before shuffling, so each of the
    // nodeCount / nodesPerDistinctValue distinct values appears exactly nodesPerDistinctValue times.
    private static int[] ExpectedModes() =>
        Enumerable.Range(0, SmallestNodeCount / NodesPerDistinctValue).ToArray();
}
