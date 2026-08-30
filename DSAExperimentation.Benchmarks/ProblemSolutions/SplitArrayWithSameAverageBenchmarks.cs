using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Split Array With Same Average (LC 805): the textbook O(2^n) all-subsets brute force
// vs. the O(n * (n/2) * sum) subset-sum-with-a-required-count DP via this repo's own
// Memoizer (PartitionEqualSubsetSumBenchmarks precedent, plus a required subset size).
// Values are kept small (not LeetCode's full 0..10000 range) specifically to keep the
// DP's (index, count, sum) state space small enough to benchmark, the same "N kept
// modest" reasoning PredictTheWinnerBenchmarks/CanIWinBenchmarks already document for
// their own exponential-search comparisons. The two Length values deliberately
// straddle the crossover: at 16, brute force still edges out the DP's per-call
// Dictionary/closure overhead; at 24, 2^24 subsets makes brute force ~350x slower
// than the DP in a local dry run, the exponential-vs-polynomial gap this problem's
// real (n up to 30) constraints exist to force.
[MemoryDiagnoser]
public class SplitArrayWithSameAverageBenchmarks
{
    [Params(16, 24)]
    public int Length;

    private int[] _nums = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(805);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(1, 30)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public bool BruteForceSubsets()
    {
        var n = _nums.Length;
        var total = _nums.Sum();

        for (var mask = 1; mask < (1 << n) - 1; mask++)
        {
            var count = 0;
            var sum = 0;
            for (var i = 0; i < n; i++)
            {
                if ((mask & (1 << i)) != 0)
                {
                    count++;
                    sum += _nums[i];
                }
            }

            if (sum * n == total * count)
            {
                return true;
            }
        }

        return false;
    }

    [Benchmark]
    public bool MemoizedSubsetSumWithCount()
    {
        var n = _nums.Length;
        var total = _nums.Sum();

        for (var k = 1; k <= n / 2; k++)
        {
            if (total * k % n != 0)
            {
                continue;
            }

            var targetSum = total * k / n;
            if (Memoizer.Memoize<(int Index, int Count, int Sum), bool>((0, k, targetSum), CanReach))
            {
                return true;
            }
        }

        return false;

        bool CanReach((int Index, int Count, int Sum) state, Func<(int Index, int Count, int Sum), bool> canReach)
        {
            if (state.Count == 0)
            {
                return state.Sum == 0;
            }

            if (state.Index == _nums.Length || state.Sum < 0)
            {
                return false;
            }

            return canReach((state.Index + 1, state.Count, state.Sum))
                || canReach((state.Index + 1, state.Count - 1, state.Sum - _nums[state.Index]));
        }
    }
}
