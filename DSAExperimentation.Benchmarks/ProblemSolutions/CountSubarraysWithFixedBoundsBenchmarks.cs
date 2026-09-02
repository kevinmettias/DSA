using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.SegmentTree;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Count Subarrays With Fixed Bounds (LC 2444): the textbook triple-nested-loop
// baseline rescans each subarray from scratch to recompute its own min/max (O(subarray
// length) per (start,end) pair, paid again from zero every time) vs. this repo's own
// SegmentTree<int,MinOperation<int>>/SegmentTree<int,MaxOperation<int>> pair
// answering each subarray's min/max in O(log n) instead,
// CountSubarraysWithFixedBoundsTests' exact composition.
[MemoryDiagnoser]
public class CountSubarraysWithFixedBoundsBenchmarks
{
    private const int RandomSeed = 2444; // LC problem number
    private const int MinK = 2;
    private const int MaxK = 8;
    private const int MaxValueExclusive = 10;

    [Params(20, 100)]
    public int Length;

    private int[] _nums = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxValueExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public long RescanEachSubarray()
    {
        long count = 0;

        for (var start = 0; start < _nums.Length; start++)
        {
            for (var end = start; end < _nums.Length; end++)
            {
                var min = int.MaxValue;
                var max = int.MinValue;

                for (var i = start; i <= end; i++)
                {
                    min = Math.Min(min, _nums[i]);
                    max = Math.Max(max, _nums[i]);
                }

                if (min == MinK && max == MaxK)
                {
                    count++;
                }
            }
        }

        return count;
    }

    [Benchmark]
    public long SegmentTreeRangeQueries()
    {
        var minTree = new SegmentTree<int, MinOperation<int>>(_nums);
        var maxTree = new SegmentTree<int, MaxOperation<int>>(_nums);
        long count = 0;

        for (var start = 0; start < _nums.Length; start++)
        {
            for (var end = start; end < _nums.Length; end++)
            {
                if (minTree.Query(start, end) == MinK && maxTree.Query(start, end) == MaxK)
                {
                    count++;
                }
            }
        }

        return count;
    }
}
