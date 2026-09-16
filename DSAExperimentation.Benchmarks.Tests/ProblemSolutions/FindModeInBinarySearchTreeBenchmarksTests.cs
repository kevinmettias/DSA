using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindModeInBinarySearchTreeBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - a dictionary frequency count over the whole tree
// against an in-order walk that exploits the BST ordering - so a harness whose arms disagree has
// found two different sets of modes.
//
// AnswerText.OfUnorderedSet, not Of, because the outer order here is genuinely unfixed: LC 501
// accepts the modes in any order, the hash arm reports them in its dictionary's iteration order and
// the in-order arm reports them ascending. Neither order is promised by the problem, and the arms'
// disagreement about it is not a disagreement about the answer - which is why the elements are
// compared as a set while each mode's own value is still compared exactly.
//
// Setup's tree holds NodeCount / NodesPerDistinctValue distinct values, each repeated
// NodesPerDistinctValue times, so every one of them ties for the mode and the answer is decisive: the
// set of all distinct values, not a seed-dependent subset.
public sealed partial class FindModeInBinarySearchTreeBenchmarksTests
{
    // The smaller of Setup's [Params(500, 20_000)] node counts.
    private const int SmallestNodeCount = 500;

    // The fixture's own copies per distinct value, restated so the expected mode set is derived
    // rather than observed.
    private const int NodesPerDistinctValue = 20;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameModes() =>
        Assert.Equal(
            AnswerText.OfUnorderedSet(BuildHarness().HashMapFrequencyCount()),
            AnswerText.OfUnorderedSet(BuildHarness().HashMapFrequencyCount()));

    [Fact]
    public void HashMapFrequencyCount_EveryValueTiedForTheMode_AgreesWithInOrderTraversalStreak()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.OfUnorderedSet(ExpectedModes()), AnswerText.OfUnorderedSet(harness.HashMapFrequencyCount()));
        Assert.Equal(
            AnswerText.OfUnorderedSet(harness.InOrderTraversalStreak()),
            AnswerText.OfUnorderedSet(harness.HashMapFrequencyCount()));
    }

    [Fact]
    public void InOrderTraversalStreak_EveryValueTiedForTheMode_AgreesWithHashMapFrequencyCount()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.OfUnorderedSet(ExpectedModes()), AnswerText.OfUnorderedSet(harness.InOrderTraversalStreak()));
        Assert.Equal(
            AnswerText.OfUnorderedSet(harness.HashMapFrequencyCount()),
            AnswerText.OfUnorderedSet(harness.InOrderTraversalStreak()));
    }

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
