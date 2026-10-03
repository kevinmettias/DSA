using DSAExperimentation.Algorithms.Metrics;
using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ConstructBinaryTreeFromPreorderAndPostorderTraversalBenchmarks (ARCHITECTURE
// 17.9), for what BenchmarkArmsTests cannot pin: the rebuilt tree's shape, known from Setup's
// construction rather than from either arm. Setup builds the degenerate chain the comment names:
// descending preorder against ascending postorder, the same tree in opposite walks. A NodeCount-node
// chain has height exactly NodeCount, so each arm's rebuilt root - returned as object?, the node type
// being internal (CS0050) - must reach that height.
public sealed partial class ConstructBinaryTreeFromPreorderAndPostorderTraversalBenchmarksTests
{
    private const int SmallestNodeCount = 200;

    [Fact]
    public void LinearRescan_LeftSkewedChain_RebuildsAChainOfEveryNode() =>
        Assert.Equal(SmallestNodeCount, HeightOf(BuildHarness().LinearRescan()));

    [Fact]
    public void HashMapIndexed_LeftSkewedChain_RebuildsAChainOfEveryNode() =>
        Assert.Equal(SmallestNodeCount, HeightOf(BuildHarness().HashMapIndexed()));

    private static int HeightOf(object? answer) =>
        TreeMetrics.Height<
            BinaryTreeNode<int>, BinaryTreeTopology<int>, BinaryTreeChildren<int>,
            NaturalChildOrder<BinaryTreeNode<int>, BinaryTreeChildren<int>>, BinaryTreeChildren<int>>(
            Assert.IsType<BinaryTreeNode<int>>(answer));

    private static ConstructBinaryTreeFromPreorderAndPostorderTraversalBenchmarks BuildHarness()
    {
        var harness = new ConstructBinaryTreeFromPreorderAndPostorderTraversalBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
