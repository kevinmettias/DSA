using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.MinimumAbsoluteDifferenceInBST;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumAbsoluteDifferenceInBSTSolution's, the same
// methods MinimumAbsoluteDifferenceInBSTTests proves correct.
[MemoryDiagnoser]
public class MinimumAbsoluteDifferenceInBSTBenchmarks
{
    private const int MidpointDivisor = 2;

    [Params(100, 5_000)]
    public int Size;

    private BinaryTreeNode<int> _root = null!;

    [GlobalSetup]
    public void Setup() => _root = BuildBalancedBst(Size);

    [Benchmark(Baseline = true)]
    public int RecursiveScan() =>
        MinimumAbsoluteDifferenceInBSTSolution.GetMinimumDifferenceByRecursiveScan(_root);

    [Benchmark]
    public int InOrderTraversalHooks() =>
        MinimumAbsoluteDifferenceInBSTSolution.GetMinimumDifferenceByInOrderHooks(_root);

    // A balanced BST over 0..size-1 - every adjacent in-order pair differs by
    // exactly 1, so the minimum difference is size-independent and never trivially
    // short-circuits after one comparison.
    private static BinaryTreeNode<int> BuildBalancedBst(int size)
    {
        var values = Enumerable.Range(0, size).ToArray();
        return BuildBalanced(values, 0, size - 1)!;
    }

    private static BinaryTreeNode<int>? BuildBalanced(int[] values, int low, int high)
    {
        if (low > high)
        {
            return null;
        }

        var mid = low + ((high - low) / MidpointDivisor);

        return new BinaryTreeNode<int>(values[mid])
        {
            Left = BuildBalanced(values, low, mid - 1),
            Right = BuildBalanced(values, mid + 1, high),
        };
    }
}
