using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for IncreasingOrderSearchTreeBenchmarks (ARCHITECTURE 17.9), for what BenchmarkArmsTests cannot
// pin: the relinked chain itself, derived independently from the fixture rather than from either arm. Each arm
// returns the relinked chain's root as object? (the node type is internal, CS0050). [GlobalSetup] shuffles the
// values 1..NodeCount with a fixed seed, so an in-order relink must produce exactly 1..NodeCount ascending with no
// left child, rooted at the lowest shuffled value - LC 897 makes the smallest value the root.
public sealed partial class IncreasingOrderSearchTreeBenchmarksTests
{
    private const int SmallestNodeCount = 10;

    // SeededSequences.ShuffledOneTo starts at one, so the ascending chain starts here too.
    private const int LowestShuffledValue = 1;

    [Fact]
    public void RecursiveRelink_ShuffledValues_RelinksEveryValueIntoOneAscendingChain() =>
        Assert.Equal(AscendingValues(SmallestNodeCount), ChainValues(BuildHarness().RecursiveRelink()));

    [Fact]
    public void InOrderTraversalHooks_ShuffledValues_RelinksEveryValueIntoOneAscendingChain() =>
        Assert.Equal(AscendingValues(SmallestNodeCount), ChainValues(BuildHarness().InOrderTraversalHooks()));

    private static IncreasingOrderSearchTreeBenchmarks BuildHarness()
    {
        var harness = new IncreasingOrderSearchTreeBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }

    // LC 897's answer read straight off the fixture's documented values, not out of the walk.
    private static int[] AscendingValues(int nodeCount) => [.. Enumerable.Range(LowestShuffledValue, nodeCount)];

    private static int[] ChainValues(object? answer)
    {
        var values = new List<int>();

        for (var node = Assert.IsType<BinaryTreeNode<int>>(answer); node is not null; node = node.Right)
        {
            values.Add(node.Value);
            Assert.Null(node.Left);
        }

        return [.. values];
    }
}
