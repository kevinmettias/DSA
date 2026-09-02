using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Folding;
using DSAExperimentation.Algorithms.Folding.Dags.Trees;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// House Robber III (LC 337): a hand-rolled post-order recursion returning the
// (Robbed, NotRobbed) pair directly vs. this repo's generic TreeFold engine closed
// over the same pair as an IFoldAlgebra witness - the same TreeFold composition
// FoldTests already exercises, over BinaryTreeNode<int> instead of a synthetic n-ary
// test tree.
[MemoryDiagnoser]
public class HouseRobberIIIBenchmarks
{
    private const int MaxNodeValue = 100;

    [Params(10, 15)]
    public int Depth;

    private BinaryTreeNode<int> _root = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _root = BuildTree(Depth, random);
    }

    private static BinaryTreeNode<int> BuildTree(int depth, Random random)
    {
        var node = new BinaryTreeNode<int>(random.Next(1, MaxNodeValue));

        if (depth > 0)
        {
            node.Left = BuildTree(depth - 1, random);
            node.Right = BuildTree(depth - 1, random);
        }

        return node;
    }

    [Benchmark(Baseline = true)]
    public int RecursivePair()
    {
        var (robbed, notRobbed) = Gain(_root);
        return Math.Max(robbed, notRobbed);
    }

    private static (int Robbed, int NotRobbed) Gain(BinaryTreeNode<int>? node)
    {
        if (node is null)
        {
            return (0, 0);
        }

        var left = Gain(node.Left);
        var right = Gain(node.Right);

        return (
            node.Value + left.NotRobbed + right.NotRobbed,
            Math.Max(left.Robbed, left.NotRobbed) + Math.Max(right.Robbed, right.NotRobbed));
    }

    [Benchmark]
    public int TreeFoldAlgebra()
    {
        var (robbed, notRobbed) = TreeFold.Fold<
            BinaryTreeNode<int>,
            BinaryTreeTopology<int>,
            BinaryTreeChildren<int>,
            NaturalChildOrder<BinaryTreeNode<int>, BinaryTreeChildren<int>>,
            BinaryTreeChildren<int>,
            RobFoldAlgebra,
            (int Robbed, int NotRobbed)>(_root);

        return Math.Max(robbed, notRobbed);
    }

    private readonly struct RobFoldAlgebra : IFoldAlgebra<BinaryTreeNode<int>, (int Robbed, int NotRobbed)>
    {
        public static (int Robbed, int NotRobbed) Empty => (0, 0);

        public static (int Robbed, int NotRobbed) Combine(
            BinaryTreeNode<int> node, IReadOnlyList<(int Robbed, int NotRobbed)> children)
        {
            var robbed = node.Value;
            var notRobbed = 0;

            foreach (var child in children)
            {
                robbed += child.NotRobbed;
                notRobbed += Math.Max(child.Robbed, child.NotRobbed);
            }

            return (robbed, notRobbed);
        }
    }
}
