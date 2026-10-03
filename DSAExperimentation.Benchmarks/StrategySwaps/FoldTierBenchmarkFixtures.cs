using DSAExperimentation.Algorithms.Folding;
using DSAExperimentation.Algorithms.Folding.Dags;
using DSAExperimentation.Algorithms.Folding.Dags.Trees;
using DSAExperimentation.Algorithms.Metrics;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Benchmarks.StrategySwaps;

// One fold, one algebra, one tree, three topology tiers. A binary tree satisfies every tier's
// bound (ITreeTopology refines IDagTopology refines IGraphTopology), so each entry point can be
// handed the same input and must give the same answer; what differs is only the defence each tier
// pays for - none for TreeFold, a memo for DagFold, a memo plus an in-progress set for CheckedFold.
internal static class FoldTierBenchmarkFixtures
{
    public static int TreeTier(BinaryTreeNode<int> root)
        => TreeFold.Fold<
            BinaryTreeNode<int>, BinaryTreeTopology<int>, BinaryTreeChildren<int>,
            SizeAlgebra<BinaryTreeNode<int>>, int>(root);

    public static int DagTier(BinaryTreeNode<int> root)
        => DagFold.Fold<
            BinaryTreeNode<int>, BinaryTreeTopology<int>, BinaryTreeChildren<int>,
            SizeAlgebra<BinaryTreeNode<int>>, int>(root);

    // A tree has no cycle for CheckedFold to report, so its success flag is always true here and
    // the size is the whole answer.
    public static int GraphTier(BinaryTreeNode<int> root)
    {
        CheckedFold.TryFold<
            BinaryTreeNode<int>, BinaryTreeTopology<int>, BinaryTreeChildren<int>,
            SizeAlgebra<BinaryTreeNode<int>>, int>(root, out var size);

        return size;
    }
}
