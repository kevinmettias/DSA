using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Subarrays Distinct Element Sum of Squares I (LC 2913): the naive baseline
// re-derives each subarray's distinct count from scratch with an O(length) linear
// scan per element (O(n) per subarray, O(n^2) subarrays => O(n^3) total). The
// primitive-based arm grows one Set<int> per start index as the end index sweeps
// rightward, so each new element costs one O(1)-amortized TryAdd instead of a fresh
// scan - O(n^2) total, the same complexity split TwoSumBenchmarks' brute-force-vs-
// HashMap arms already demonstrate. Params stay at or under 100, this problem's own
// constraint on nums.Length.
[MemoryDiagnoser]
public class SubarraysDistinctElementSumOfSquaresIBenchmarks
{
    [Params(20, 100)]
    public int Length;

    private int[] _nums = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(1, 101)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public long BruteForce()
    {
        long total = 0;

        for (var start = 0; start < _nums.Length; start++)
        {
            for (var end = start; end < _nums.Length; end++)
            {
                var distinct = CountDistinctByLinearScan(start, end);
                total += (long)distinct * distinct;
            }
        }

        return total;
    }

    private int CountDistinctByLinearScan(int start, int end)
    {
        var distinct = 0;

        for (var i = start; i <= end; i++)
        {
            var isFirstOccurrence = true;

            for (var j = start; j < i; j++)
            {
                if (_nums[j] == _nums[i])
                {
                    isFirstOccurrence = false;
                    break;
                }
            }

            if (isFirstOccurrence)
            {
                distinct++;
            }
        }

        return distinct;
    }

    [Benchmark]
    public long GrowingSet()
    {
        long total = 0;

        for (var start = 0; start < _nums.Length; start++)
        {
            var distinct = new Set<int>();

            for (var end = start; end < _nums.Length; end++)
            {
                distinct.TryAdd(_nums[end]);
                total += (long)distinct.Count * distinct.Count;
            }
        }

        return total;
    }
}
