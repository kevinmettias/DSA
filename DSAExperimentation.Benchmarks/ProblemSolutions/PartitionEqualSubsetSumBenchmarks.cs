using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Partition Equal Subset Sum (LC 416): plain bottom-up bool[] tabulation vs.
// this repo's Memoizer-based top-down recursion (CoinChange/CombinationSumIV
// precedent) - both solve the same 0/1-knapsack subset-sum recurrence in
// O(nums.Length * half), just walking it from opposite directions.
[MemoryDiagnoser]
public class PartitionEqualSubsetSumBenchmarks
{
    private const int RandomSeed = 416; // LC problem number
    private const int MaxElementValue = 100;
    private const int SubsetSumDivisor = 2;

    [Params(50, 400)]
    public int Length;

    private int[] _nums = null!;
    private int _half;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxElementValue)).ToArray();
        _half = _nums.Sum() / SubsetSumDivisor;
    }

    [Benchmark(Baseline = true)]
    public bool Tabulation()
    {
        var dp = new bool[_half + 1];
        dp[0] = true;

        foreach (var num in _nums)
        {
            for (var remaining = _half; remaining >= num; remaining--)
            {
                dp[remaining] = dp[remaining] || dp[remaining - num];
            }
        }

        return dp[_half];
    }

    [Benchmark]
    public bool Memoized()
    {
        return Memoizer.Memoize<(int Index, int Remaining), bool>((0, _half), CanReach);

        bool CanReach((int Index, int Remaining) state, Func<(int Index, int Remaining), bool> canReach)
        {
            if (state.Remaining == 0)
            {
                return true;
            }

            if (state.Remaining < 0 || state.Index == _nums.Length)
            {
                return false;
            }

            return canReach((state.Index + 1, state.Remaining - _nums[state.Index]))
                || canReach((state.Index + 1, state.Remaining));
        }
    }
}
