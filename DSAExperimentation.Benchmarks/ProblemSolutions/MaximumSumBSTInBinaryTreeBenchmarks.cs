using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Maximum Sum BST in Binary Tree (LC 1373): a naive per-node approach revalidates
// and re-sums each node's whole subtree independently (like ValidateBinarySearchT
// reeBenchmarks' own Validate), O(n) work at every one of n nodes - O(n^2) overall
// on a tree where every subtree really is BST-valid, forcing the full walk every
// time - vs. this repo's own BinaryTreeNode<int> walked with a single bottom-up
// post-order pass (the same "compute everything a node needs from its two already-
// -folded children" shape TreeMetrics/DiameterAlgebra use, just hand-rolled here
// since BinaryTreeChildren's compaction can't preserve the left/right identity
// this check needs), O(n). Fixtures.BinaryTrees.Skewed's strictly increasing
// right-only chain is itself a valid BST end to end, the same reason DiameterOfBi
// naryTreeBenchmarks reuses a skewed shape to keep the naive baseline's cost real
// instead of hidden behind O(log n) depth.
[MemoryDiagnoser]
public class MaximumSumBSTInBinaryTreeBenchmarks
{
    [Params(200, 2_000)]
    public int NodeCount;

    private BinaryTreeNode<int> _root = null!;

    [GlobalSetup]
    public void Setup() => _root = BinaryTrees.Skewed(NodeCount);

    [Benchmark(Baseline = true)]
    public int RevalidatePerNode()
    {
        var best = int.MinValue;
        Visit(_root);
        return best;

        void Visit(BinaryTreeNode<int>? node)
        {
            if (node is null)
            {
                return;
            }

            if (IsValidBst(node, null, null))
            {
                best = Math.Max(best, Sum(node));
            }

            Visit(node.Left);
            Visit(node.Right);
        }
    }

    private static bool IsValidBst(BinaryTreeNode<int>? node, int? min, int? max)
        => node is null || ((min is null || node.Value > min) && (max is null || node.Value < max)
            && IsValidBst(node.Left, min, node.Value) && IsValidBst(node.Right, node.Value, max));

    private static int Sum(BinaryTreeNode<int>? node)
        => node is null ? 0 : node.Value + Sum(node.Left) + Sum(node.Right);

    [Benchmark]
    public int OnePassBottomUpScan()
    {
        var best = int.MinValue;
        Scan(_root);
        return best;

        Summary Scan(BinaryTreeNode<int>? node)
        {
            if (node is null)
            {
                return new Summary(true, int.MaxValue, int.MinValue, 0);
            }

            var left = Scan(node.Left);
            var right = Scan(node.Right);
            var isBst = left.IsBst && right.IsBst && node.Value > left.Max && node.Value < right.Min;

            if (!isBst)
            {
                return new Summary(false, 0, 0, 0);
            }

            var sum = left.Sum + right.Sum + node.Value;
            best = Math.Max(best, sum);

            return new Summary(true, Math.Min(node.Value, left.Min), Math.Max(node.Value, right.Max), sum);
        }
    }

    private readonly record struct Summary(bool IsBst, int Min, int Max, int Sum);
}
