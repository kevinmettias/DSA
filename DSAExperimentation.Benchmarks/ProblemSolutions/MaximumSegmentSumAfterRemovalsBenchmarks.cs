using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.DisjointSet;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Maximum Segment Sum After Removals (LC 2382): rescanning the whole array for the
// best remaining contiguous segment after every single removal - O(n^2) - against
// this repo's own DisjointSet driving the reverse-time trick: replay the removals
// backward as insertions into an initially-empty array, unioning each restored
// index with any already-standing neighbor while a per-root running sum (the same
// "own scratch state beside Find/Union" shape BricksFallingWhenHitBenchmarks'
// per-root size already establishes) tracks each segment's total. Both benchmarks
// return the sum of the full answer[] array so a real full pass is forced on every
// invocation rather than one index's worth of work.
[MemoryDiagnoser]
public class MaximumSegmentSumAfterRemovalsBenchmarks
{
    private const int RandomSeed = 2382; // LC problem number
    private const int MaxValueExclusive = 1_000;

    [Params(200, 5_000)]
    public int Length;

    private int[] _nums = null!;
    private int[] _removeQueries = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxValueExclusive)).ToArray();
        _removeQueries = Shuffle(random, Enumerable.Range(0, Length).ToArray());
    }

    private static int[] Shuffle(Random random, int[] values)
    {
        for (var i = values.Length - 1; i > 0; i--)
        {
            var j = random.Next(i + 1);
            (values[i], values[j]) = (values[j], values[i]);
        }

        return values;
    }

    [Benchmark(Baseline = true)]
    public long RescanAfterEachRemoval()
    {
        var n = _nums.Length;
        var removed = new bool[n];
        var total = 0L;

        for (var i = 0; i < n; i++)
        {
            removed[_removeQueries[i]] = true;
            total += MaxSegmentSum(removed);
        }

        return total;
    }

    private long MaxSegmentSum(bool[] removed)
    {
        var best = 0L;
        var current = 0L;

        for (var i = 0; i < removed.Length; i++)
        {
            if (removed[i])
            {
                current = 0;
                continue;
            }

            current += _nums[i];
            best = Math.Max(best, current);
        }

        return best;
    }

    [Benchmark]
    public long ReverseTimeDisjointSet()
    {
        var n = _nums.Length;
        var answer = new long[n];
        var present = new bool[n];
        var sum = new long[n];
        var components = new DisjointSet(n);
        var maxSum = 0L;

        for (var i = n - 1; i >= 1; i--)
        {
            var index = _removeQueries[i];
            Insert(components, present, sum, index);
            maxSum = Math.Max(maxSum, sum[components.Find(index)]);
            answer[i - 1] = maxSum;
        }

        var total = 0L;
        foreach (var value in answer)
        {
            total += value;
        }

        return total;
    }

    private void Insert(DisjointSet components, bool[] present, long[] sum, int index)
    {
        present[index] = true;
        sum[index] = _nums[index];

        if (index > 0 && present[index - 1])
        {
            Merge(components, sum, index, index - 1);
        }

        if (index < _nums.Length - 1 && present[index + 1])
        {
            Merge(components, sum, index, index + 1);
        }
    }

    private static void Merge(DisjointSet components, long[] sum, int first, int second)
    {
        var firstRoot = components.Find(first);
        var secondRoot = components.Find(second);

        components.Union(first, second);
        var mergedRoot = components.Find(first);
        sum[mergedRoot] = sum[firstRoot] + sum[secondRoot];
    }
}
