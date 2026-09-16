using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindElementsInAContaminatedBinaryTreeBenchmarks (ARCHITECTURE 17.9): its
// two arms are competing strategies for the same question - recover a contaminated tree into a
// BCL List<int> and answer each Find by linear scan, or recover it into this repo's Set<int> and
// answer in O(1) - so a harness whose arms disagree is answering two different recovery rules.
//
// Each arm returns only how many of the fixed target sample it found, which is a proxy for the
// per-query verdicts rather than the verdicts themselves: two arms whose errors cancel would still
// report one count. It is not a length proxy - a single query answered differently moves the count
// - but a per-query disagreement is only witnessed here through its effect on the total.
//
// The tree is Fixtures' BinaryTrees.Balanced, a complete tree in heap layout, so node i recovers
// exactly the value i and the found set is [0, NodeCount). Setup draws its targets from
// [0, 2 * NodeCount), so some targets must be found and some must miss on every run.
public sealed partial class FindElementsInAContaminatedBinaryTreeBenchmarksTests
{
    // The smaller of Setup's [Params(200, 2_000)] node counts.
    private const int SmallestNodeCount = 200;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameFoundCount() =>
        Assert.Equal(
            BuildHarness().ListRecoverThenLinearScan(),
            BuildHarness().ListRecoverThenLinearScan());

    [Fact]
    public void ListRecoverThenLinearScan_HalfMissingTargetSample_AgreesWithTopDownRecoverThenSetLookup()
    {
        var harness = BuildHarness();

        Assert.InRange(harness.ListRecoverThenLinearScan(), 1, SmallestNodeCount - 1);
        Assert.Equal(harness.TopDownRecoverThenSetLookup(), harness.ListRecoverThenLinearScan());
    }

    [Fact]
    public void TopDownRecoverThenSetLookup_HalfMissingTargetSample_AgreesWithListRecoverThenLinearScan()
    {
        var harness = BuildHarness();

        Assert.InRange(harness.TopDownRecoverThenSetLookup(), 1, SmallestNodeCount - 1);
        Assert.Equal(harness.ListRecoverThenLinearScan(), harness.TopDownRecoverThenSetLookup());
    }

    private static FindElementsInAContaminatedBinaryTreeBenchmarks BuildHarness()
    {
        var harness = new FindElementsInAContaminatedBinaryTreeBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
