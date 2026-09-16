using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ConstructBinaryTreeFromPreorderAndPostorderTraversalBenchmarks (ARCHITECTURE
// 17.9): its two arms are competing strategies for the same question - locating each node's
// left-subtree root by rescanning the live postorder range against a HashMap index built once - so a
// harness whose arms disagree is timing two different problems.
//
// Setup builds the degenerate left-skewed chain the comment names: descending preorder against
// ascending postorder, the same tree in opposite walks. Rebuilding it and measuring its height is
// what each arm reports, and a NodeCount-node chain has height exactly NodeCount, so the workload
// shape is decisive rather than merely repeated: the same NodeCount must rebuild the same chain.
//
// The height is a proxy for the rebuilt tree, not the tree itself - agreement witnesses that both
// arms produced a chain of the same depth, not that every parent and child match. The arms' own
// comment says the comparison is about how each locates a node's left-subtree root, which is the
// cost this harness times; the value is only there to keep the walk from being optimized away.
public sealed partial class ConstructBinaryTreeFromPreorderAndPostorderTraversalBenchmarksTests
{
    private const int SmallestNodeCount = 200;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameLeftSkewedChain()
    {
        Assert.Equal(SmallestNodeCount, BuildHarness().LinearRescan());
        Assert.Equal(BuildHarness().LinearRescan(), BuildHarness().LinearRescan());
    }

    [Fact]
    public void LinearRescan_LeftSkewedChain_AgreesWithHashMapIndexed()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.HashMapIndexed(), harness.LinearRescan());
    }

    [Fact]
    public void HashMapIndexed_LeftSkewedChain_AgreesWithLinearRescan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.LinearRescan(), harness.HashMapIndexed());
    }

    private static ConstructBinaryTreeFromPreorderAndPostorderTraversalBenchmarks BuildHarness()
    {
        var harness = new ConstructBinaryTreeFromPreorderAndPostorderTraversalBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
