using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MergeBSTsToCreateSingleBSTBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - rescanning the tree list for the root matching each leaf
// against a HashMap<int, BinaryTreeNode<int>> index of the same roots - so a harness whose arms disagree
// is timing two different problems. Splicing consumes the trees, so each arm clones the shared template
// inside its own call; the clone is identically charged to both arms and one harness is therefore safe
// to read twice in either order.
//
// Agreement here is weak by construction, and honestly so. Both arms return only "is not null" of the
// merged root - a bool standing in for the tree they built, because the measured method's result has to
// be consumed without walking it - so a green pair witnesses that both strategies found SOME single
// merged root for the chain, not that they built the same tree. The chain is designed to merge, so both
// verdicts are true for every [Params] TreeCount. Setup builds the chain from its own shape rather than a
// seed, so the same TreeCount must rebuild the same template and with it the same verdict.
public sealed partial class MergeBSTsToCreateSingleBSTBenchmarksTests
{
    private const int SmallestTreeCount = 50;

    [Fact]
    public void Setup_SameTreeCount_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().CanMergeByLinearScan(), BuildHarness().CanMergeByLinearScan());

    [Fact]
    public void CanMergeByLinearScan_MatchingChain_AgreesWithCanMergeByHashMapIndex()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.CanMergeByHashMapIndex(), harness.CanMergeByLinearScan());
    }

    [Fact]
    public void CanMergeByHashMapIndex_MatchingChain_AgreesWithCanMergeByLinearScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.CanMergeByLinearScan(), harness.CanMergeByHashMapIndex());
    }

    private static MergeBSTsToCreateSingleBSTBenchmarks BuildHarness()
    {
        var harness = new MergeBSTsToCreateSingleBSTBenchmarks { TreeCount = SmallestTreeCount };
        harness.Setup();

        return harness;
    }
}
