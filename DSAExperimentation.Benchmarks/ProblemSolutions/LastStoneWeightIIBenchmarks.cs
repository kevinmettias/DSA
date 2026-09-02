using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Last Stone Weight II (LC 1049): plain bottom-up int[] tabulation vs. this
// repo's Memoizer-based top-down recursion (PartitionEqualSubsetSum precedent)
// - both solve the same 0/1-knapsack "closest subset sum to half the total"
// recurrence in O(stones.Length * half), just walking it from opposite
// directions.
[MemoryDiagnoser]
public class LastStoneWeightIIBenchmarks
{
    private const int RandomSeed = 1049; // LC problem number
    private const int StoneWeightUpperBound = 100;
    private const int HalfDivisor = 2;
    private const int PartitionDifferenceMultiplier = 2;

    [Params(30, 200)]
    public int Length;

    private int[] _stones = null!;
    private int _total;
    private int _half;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _stones = Enumerable.Range(0, Length).Select(_ => random.Next(1, StoneWeightUpperBound)).ToArray();
        _total = _stones.Sum();
        _half = _total / HalfDivisor;
    }

    [Benchmark(Baseline = true)]
    public int Tabulation()
    {
        var dp = new int[_half + 1];

        foreach (var stone in _stones)
        {
            for (var capacity = _half; capacity >= stone; capacity--)
            {
                dp[capacity] = Math.Max(dp[capacity], dp[capacity - stone] + stone);
            }
        }

        return _total - PartitionDifferenceMultiplier * dp[_half];
    }

    [Benchmark]
    public int Memoized()
    {
        var closestToHalf = Memoizer.Memoize<(int Index, int Capacity), int>((0, _half), BestReachableSum);
        return _total - PartitionDifferenceMultiplier * closestToHalf;

        int BestReachableSum((int Index, int Capacity) state, Func<(int Index, int Capacity), int> bestReachableSum)
        {
            if (state.Index == _stones.Length)
            {
                return 0;
            }

            var skip = bestReachableSum((state.Index + 1, state.Capacity));

            if (_stones[state.Index] > state.Capacity)
            {
                return skip;
            }

            var take = _stones[state.Index] + bestReachableSum((state.Index + 1, state.Capacity - _stones[state.Index]));
            return Math.Max(skip, take);
        }
    }
}
