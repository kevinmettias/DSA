using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using NodeStack = DSAExperimentation.DataStructures.Stack.Stack<DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.BinaryTreeNode<int>>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Maximum Binary Tree (LC 654): the textbook "find the max, split, recurse"
// construction re-scans its shrinking subarray for the max at every level -
// O(n^2) on an ascending input, where the max is always the last element and the
// left half never shrinks by more than one node per call, mirroring
// DiameterOfBinaryTreeBenchmarks' degenerate-chain framing - vs. the O(n)
// monotonic-stack construction built on this repo's own LIFO Stack<T>.
[MemoryDiagnoser]
public class MaximumBinaryTreeBenchmarks
{
    [Params(200, 2_000)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup() => _values = Enumerable.Range(0, Length).ToArray();

    [Benchmark(Baseline = true)]
    public int RescanForMax() => Height(RescanBuild(_values, 0, _values.Length - 1));

    private static BinaryTreeNode<int>? RescanBuild(int[] nums, int low, int high)
    {
        if (low > high)
        {
            return null;
        }

        var maxIndex = low;

        for (var i = low + 1; i <= high; i++)
        {
            if (nums[i] > nums[maxIndex])
            {
                maxIndex = i;
            }
        }

        return new BinaryTreeNode<int>(nums[maxIndex])
        {
            Left = RescanBuild(nums, low, maxIndex - 1),
            Right = RescanBuild(nums, maxIndex + 1, high),
        };
    }

    [Benchmark]
    public int MonotonicStack() => Height(StackBuild(_values));

    private static int Height(BinaryTreeNode<int>? node)
        => node is null ? 0 : 1 + Math.Max(Height(node.Left), Height(node.Right));

    private static BinaryTreeNode<int>? StackBuild(int[] nums)
    {
        var stack = new NodeStack();

        foreach (var num in nums)
        {
            var node = new BinaryTreeNode<int>(num);

            while (stack.TryPeek(out var smaller) && smaller.Value < num)
            {
                stack.TryPop(out _);
                node.Left = smaller;
            }

            if (stack.TryPeek(out var parent))
            {
                parent.Right = node;
            }

            stack.Push(node);
        }

        BinaryTreeNode<int>? root = null;

        while (stack.TryPop(out var remaining))
        {
            root = remaining;
        }

        return root;
    }
}
