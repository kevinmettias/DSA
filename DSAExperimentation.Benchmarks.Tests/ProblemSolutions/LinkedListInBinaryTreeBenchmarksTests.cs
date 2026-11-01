using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for LinkedListInBinaryTreeBenchmarks (ARCHITECTURE 17.9): the needle is the
// skewed chain's own opening values 1..99 followed by a value the chain does not carry at that depth,
// so no downward path matches and the baseline must answer false - a verdict the workload's
// construction pins independently of the arms, which the generic agreement check cannot supply.
//
// The weaker half is reported with the batch: this workload cannot tell an arm that answers false
// for the right reason from one that answers false always, because the single genuine candidate path
// breaks on its final value.
public sealed partial class LinkedListInBinaryTreeBenchmarksTests
{
    private const int SmallestNodeCount = 200;
    private const bool ExpectedSubPathPresent = false;

    [Fact]
    public void IsSubPathByArraySliceWalk_BrokenNeedleTail_FindsNoSubPath() =>
        Assert.Equal(ExpectedSubPathPresent, BuildHarness().IsSubPathByArraySliceWalk());

    private static LinkedListInBinaryTreeBenchmarks BuildHarness()
    {
        var harness = new LinkedListInBinaryTreeBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
