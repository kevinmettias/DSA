using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Minimum Absolute Difference Between Elements With Constraint (LC 2817): the
// brute-force baseline checks every (i, j) pair with j - i >= x directly, O(n^2).
// BstSlidingWindow instead slides j across nums, inserting nums[j - x] into this
// repo's own BinarySearchTree<int> exactly when it becomes eligible, then asking
// FindClosest.TryFind (the same nearest-value primitive
// ClosestNodesQueriesInABinarySearchTreeBenchmarks already proves out) for the
// tree's closest value to nums[j] - O(n log n).
[MemoryDiagnoser]
public class MinimumAbsoluteDifferenceBetweenElementsWithConstraintBenchmarks
{
    private const int RandomSeed = 2817; // LeetCode problem number
    private const int MaxValueExclusive = 1_000_000;

    [Params(200, 5_000)]
    public int Length;

    private int[] _nums = null!;
    private int _x;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxValueExclusive)).ToArray();
        _x = Length / 4;
    }

    [Benchmark(Baseline = true)]
    public int BruteForcePairScan()
    {
        var minDifference = int.MaxValue;

        for (var i = 0; i < _nums.Length; i++)
        {
            for (var j = i + _x; j < _nums.Length; j++)
            {
                minDifference = Math.Min(minDifference, Math.Abs(_nums[i] - _nums[j]));
            }
        }

        return minDifference;
    }

    [Benchmark]
    public int BstSlidingWindow()
    {
        var tree = new BinarySearchTree<int>();
        var minDifference = int.MaxValue;

        for (var j = 0; j < _nums.Length; j++)
        {
            if (j >= _x)
            {
                tree.Insert(_nums[j - _x]);
            }

            if (FindClosest.TryFind(tree.Root, _nums[j], out var closest))
            {
                minDifference = Math.Min(minDifference, Math.Abs(closest - _nums[j]));
            }
        }

        return minDifference;
    }
}
