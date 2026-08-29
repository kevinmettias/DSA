using DSAExperimentation.Algorithms.Folding;
using DSAExperimentation.Algorithms.Folding.Dags.Trees;
using DSAExperimentation.Algorithms.Metrics;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.Benchmarks.StrategySwaps;

internal static class TreeFoldBenchmarkHelpers
{
    public static int Fold<TStrategy>(BinaryTreeNode<int> root)
        where TStrategy : struct, IFoldEvaluationStrategy<BinaryTreeNode<int>>
        => TreeFold.Fold<
            BinaryTreeNode<int>, BinaryTreeTopology<int>, BinaryTreeChildren<int>,
            NaturalChildOrder<BinaryTreeNode<int>, BinaryTreeChildren<int>>, BinaryTreeChildren<int>,
            TStrategy, SizeAlgebra<BinaryTreeNode<int>>, int>(root);
}
