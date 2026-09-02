using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Longest Well-Performing Interval (LC 1124): the O(n^2) brute force (every start/end
// pair, re-summing the +1/-1 tiring score as it extends) vs. this repo's O(n)
// HashMap<int,int> first-occurrence-of-a-prefix-score approach (the
// ContinuousSubarraySumBenchmarks precedent, applied to "> 0" instead of "% K == 0").
// Hours alternate 9 ("tiring", +1) vs. 6 ("not tiring", -1) by an unweighted coin flip,
// which keeps the running score oscillating near zero for its whole length - the
// scenario that makes the HashMap actually earn repeated lookups instead of only ever
// growing, and forces BruteForce's inner loop through its full O(n^2) worst case since
// no early exit is possible either way (both strategies must scan every candidate
// interval to find the longest, unlike TwoSumBenchmarks' find-any-pair shape).
[MemoryDiagnoser]
public class LongestWellPerformingIntervalBenchmarks
{
    private const int CoinFlipUpperBoundExclusive = 2;

    private const int TiringHour = 9;

    private const int NotTiringHour = 6;

    private const int TiringThreshold = 8;

    [Params(200, 5_000)]
    public int Length;

    private int[] _hours = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _hours = Enumerable.Range(0, Length)
            .Select(_ => random.Next(0, CoinFlipUpperBoundExclusive) == 1 ? TiringHour : NotTiringHour)
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BruteForce()
    {
        var longest = 0;
        for (var start = 0; start < _hours.Length; start++)
        {
            var score = 0;
            for (var end = start; end < _hours.Length; end++)
            {
                score += _hours[end] > TiringThreshold ? 1 : -1;
                if (score > 0 && end - start + 1 > longest)
                {
                    longest = end - start + 1;
                }
            }
        }

        return longest;
    }

    private readonly record struct PrefixScoreState(int Score, int Longest);

    [Benchmark]
    public int HashMapPrefixScore()
    {
        var firstIndexByScore = new HashMap<int, int>();
        var state = new PrefixScoreState(0, 0);

        for (var i = 0; i < _hours.Length; i++)
        {
            state = AdvancePrefixScore(state, firstIndexByScore, i);
        }

        return state.Longest;
    }

    private PrefixScoreState AdvancePrefixScore(PrefixScoreState state, HashMap<int, int> firstIndexByScore, int i)
    {
        var score = state.Score + (_hours[i] > TiringThreshold ? 1 : -1);

        if (score > 0)
        {
            return state with { Score = score, Longest = i + 1 };
        }

        var longest = state.Longest;

        if (firstIndexByScore.TryGetValue(score - 1, out var priorIndex))
        {
            longest = Math.Max(longest, i - priorIndex);
        }

        if (!firstIndexByScore.HasKey(score))
        {
            firstIndexByScore.Set(score, i);
        }

        return state with { Score = score, Longest = longest };
    }
}
