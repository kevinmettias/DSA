using DSAExperimentation.Algorithms.Reducing;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Benchmarks.StrategySwaps;

// One reduce over one balanced tree, entered once through each topology tier: Reduce.Tree wires in
// the unguarded visit a tree's unique ancestry allows, Reduce.Graph the tracked visit an arbitrary
// graph needs. Same order, same algebra, same answer, so the ratio is the cost of the graph tier's
// visited-set guard and nothing else.
public class VisitGuardBenchmarks
{
    private BinaryTreeNode<int> _root = null!;

    [Params(10_000, 200_000)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup() => _root = BinaryTrees.Balanced(NodeCount);

    [Benchmark(Baseline = true)]
    public int Unguarded()
        => Reduce.Tree<
            BinaryTreeNode<int>, BinaryTreeTopology<int>, BinaryTreeChildren<int>,
            DepthFirstReduceOrder<BinaryTreeNode<int>>, DistanceMapReduceAlgebra<BinaryTreeNode<int>>,
            Dictionary<BinaryTreeNode<int>, int>>(_root).Count;

    [Benchmark]
    public int Tracked()
        => Reduce.Graph<
            BinaryTreeNode<int>, BinaryTreeTopology<int>, BinaryTreeChildren<int>,
            DepthFirstReduceOrder<BinaryTreeNode<int>>, DistanceMapReduceAlgebra<BinaryTreeNode<int>>,
            Dictionary<BinaryTreeNode<int>, int>>(_root).Count;
}
