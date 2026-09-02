using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Count Array Pairs Divisible by K (LC 2183): the textbook O(n^2) pairwise
// product-mod check vs. this repo's own HashMap<TKey,TValue> grouping every index by
// Gcd(nums[i], k) first (CountArrayPairsDivisibleByKTests' own reasoning: divisibility
// of nums[i] * nums[j] by k depends only on those two gcds), then checking only pairs
// of the k-divisor-bounded distinct GROUPS instead of every elementwise pair -
// O(n + d^2), d = the number of distinct Gcd(value, k) groups (at most k's own divisor
// count), instead of O(n^2).
[MemoryDiagnoser]
public class CountArrayPairsDivisibleByKBenchmarks
{
    private const int K = 100;
    private const int MaxValueExclusive = 1_000;
    private const int RandomSeed = 2183; // LC problem number
    private const int DistinctPairDivisor = 2; // n choose 2 = n*(n-1)/2

    [Params(200, 5_000)]
    public int Length;

    private int[] _nums = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxValueExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public long BruteForcePairwiseCheck()
    {
        long pairs = 0;

        for (var i = 0; i < _nums.Length; i++)
        {
            for (var j = i + 1; j < _nums.Length; j++)
            {
                if ((long)_nums[i] * _nums[j] % K == 0)
                {
                    pairs++;
                }
            }
        }

        return pairs;
    }

    [Benchmark]
    public long GcdGroupedHashMapCount()
    {
        var groupCounts = new HashMap<int, int>();

        foreach (var num in _nums)
        {
            var group = Gcd(num, K);
            groupCounts.TryGetValue(group, out var existing);
            groupCounts.Set(group, existing + 1);
        }

        var groups = groupCounts.Keys.ToList();
        long pairs = 0;

        for (var i = 0; i < groups.Count; i++)
        {
            pairs += CountPairsForGroup(groups, groupCounts, i);
        }

        return pairs;
    }

    private static long CountPairsForGroup(List<int> groups, HashMap<int, int> groupCounts, int i)
    {
        long pairs = 0;

        for (var j = i; j < groups.Count; j++)
        {
            if ((long)groups[i] * groups[j] % K != 0)
            {
                continue;
            }

            groupCounts.TryGetValue(groups[i], out var countI);
            groupCounts.TryGetValue(groups[j], out var countJ);

            pairs += i == j ? (long)countI * (countI - 1) / DistinctPairDivisor : (long)countI * countJ;
        }

        return pairs;
    }

    private static int Gcd(int a, int b) => b == 0 ? a : Gcd(b, a % b);
}
