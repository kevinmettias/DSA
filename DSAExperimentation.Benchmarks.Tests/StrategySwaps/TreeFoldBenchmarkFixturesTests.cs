using DSAExperimentation.Algorithms.Folding;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.Benchmarks.StrategySwaps;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Benchmarks.Tests.StrategySwaps;

// Harness coverage for TreeFoldBenchmarkFixtures, the Fold helper both tree-fold benchmarks call.
// Fold is the one surface that invokes the fold with the injected strategy and algebra, so what it
// owes is the folded value itself: SizeAlgebra answers with the node count, which the seeded
// fixture pins to the tree's own node count independently of whichever strategy was injected.
public sealed partial class TreeFoldBenchmarkFixturesTests
{
    private const int NodeCount = 64;

    [Fact]
    public void Fold_RecursiveStrategyOnABalancedTree_CountsEveryNode() =>
        Assert.Equal(NodeCount, FoldWithRecursiveStrategy());

    [Fact]
    public void Fold_IterativeStrategyOnABalancedTree_CountsEveryNode() =>
        Assert.Equal(NodeCount, FoldWithIterativeStrategy());

    [Fact]
    public void Fold_RecursiveStrategyOnASkewedChain_CountsEveryNode() =>
        Assert.Equal(NodeCount, FoldSkewedWithRecursiveStrategy());

    [Fact]
    public void Fold_BothEvaluationStrategies_AgreeOnTheSameTree() =>
        Assert.Equal(
            TreeFoldBenchmarkFixtures.Fold<RecursiveFoldEvaluation<BinaryTreeNode<int>>>(BinaryTrees.Balanced(NodeCount)),
            TreeFoldBenchmarkFixtures.Fold<IterativeFoldEvaluation<BinaryTreeNode<int>>>(BinaryTrees.Balanced(NodeCount)));

    private static int FoldWithRecursiveStrategy() =>
        TreeFoldBenchmarkFixtures.Fold<RecursiveFoldEvaluation<BinaryTreeNode<int>>>(BinaryTrees.Balanced(NodeCount));

    private static int FoldWithIterativeStrategy() =>
        TreeFoldBenchmarkFixtures.Fold<IterativeFoldEvaluation<BinaryTreeNode<int>>>(BinaryTrees.Balanced(NodeCount));

    private static int FoldSkewedWithRecursiveStrategy() =>
        TreeFoldBenchmarkFixtures.Fold<RecursiveFoldEvaluation<BinaryTreeNode<int>>>(BinaryTrees.Skewed(NodeCount));
}
