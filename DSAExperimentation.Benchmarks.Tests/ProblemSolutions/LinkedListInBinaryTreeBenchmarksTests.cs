using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for LinkedListInBinaryTreeBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - re-slicing the flattened pattern at every step against
// walking the node chain - so a harness whose arms disagree is timing two different problems. The
// needle is the skewed chain's own values 0..NodeCount/2-1 followed by a mismatch value the chain
// never carries, so no downward path matches and both arms must answer false; the same NodeCount must
// rebuild that pair, otherwise two published numbers were never comparable in the first place.
//
// A bool is the whole observable here, so the verdict the workload pins is asserted alongside the
// agreement. The weaker half is reported with the batch: this workload cannot tell an arm that
// answers false for the right reason from one that answers false always, because the single genuine
// candidate path breaks on its final value.
public sealed partial class LinkedListInBinaryTreeBenchmarksTests
{
    private const int SmallestNodeCount = 200;
    private const bool ExpectedSubPathPresent = false;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameWorkload() =>
        Assert.Equal(
            BuildHarness().IsSubPathByArraySliceWalk(),
            BuildHarness().IsSubPathByArraySliceWalk());

    [Fact]
    public void IsSubPathByArraySliceWalk_BrokenNeedleTail_AgreesWithLinkedNodeWalk()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedSubPathPresent, harness.IsSubPathByArraySliceWalk());
        Assert.Equal(harness.IsSubPathByLinkedNodeWalk(), harness.IsSubPathByArraySliceWalk());
    }

    [Fact]
    public void IsSubPathByLinkedNodeWalk_BrokenNeedleTail_AgreesWithArraySliceWalk()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedSubPathPresent, harness.IsSubPathByLinkedNodeWalk());
        Assert.Equal(harness.IsSubPathByLinkedNodeWalk(), harness.IsSubPathByArraySliceWalk());
    }

    private static LinkedListInBinaryTreeBenchmarks BuildHarness()
    {
        var harness = new LinkedListInBinaryTreeBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
