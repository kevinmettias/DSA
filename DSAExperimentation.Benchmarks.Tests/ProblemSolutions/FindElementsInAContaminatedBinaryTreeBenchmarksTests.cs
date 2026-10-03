using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindElementsInAContaminatedBinaryTreeBenchmarks (ARCHITECTURE 17.9), for what
// BenchmarkArmsTests cannot pin: that the target sample both hits and misses, which follows from Setup's
// construction rather than from either arm. Each arm returns every Find verdict, in target order.
//
// The tree is a complete tree in heap layout with every value -1, LC 1261's contamination, so node i
// recovers exactly the value i and the found set is [0, NodeCount). Setup draws its targets from
// [0, 2 * NodeCount), so some targets must be found and some must miss on every run.
public sealed partial class FindElementsInAContaminatedBinaryTreeBenchmarksTests
{
    // The smaller of Setup's [Params(200, 2_000)] node counts.
    private const int SmallestNodeCount = 200;

    [Fact]
    public void ListRecoverThenLinearScan_HalfMissingTargetSample_FindsSomeTargetsButNotAll() =>
        Assert.InRange(BuildHarness().ListRecoverThenLinearScan().Count(found => found), 1, SmallestNodeCount - 1);

    [Fact]
    public void TopDownRecoverThenSetLookup_HalfMissingTargetSample_FindsSomeTargetsButNotAll() =>
        Assert.InRange(BuildHarness().TopDownRecoverThenSetLookup().Count(found => found), 1, SmallestNodeCount - 1);

    private static FindElementsInAContaminatedBinaryTreeBenchmarks BuildHarness()
    {
        var harness = new FindElementsInAContaminatedBinaryTreeBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
