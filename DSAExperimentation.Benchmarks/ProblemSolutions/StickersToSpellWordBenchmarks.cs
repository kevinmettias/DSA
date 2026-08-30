using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Stickers to Spell Word (LC 691): naive unmemoized recursion over the remaining
// target-letter multiset vs this repo's own Memoizer.Memoize<string,int>, the same
// Memoized-vs-hand-rolled-recursion comparison DecodeWaysBenchmarks makes, but for a
// recurrence that branches over stickers instead of a fixed left-to-right split.
// "ab"/"ba" are deliberately equal-effect stickers (both cover exactly one 'a' and
// one 'b') applied to a target built from repeated "ab" pairs, so every choice at
// every step lands on the identical resulting state - maximal overlap, Length+1
// distinct states total - while naive recomputes all 2^Length equivalent choice
// paths from scratch.
[MemoryDiagnoser]
public class StickersToSpellWordBenchmarks
{
    private static readonly string[] Stickers = ["ab", "ba"];

    [Params(10, 16)]
    public int PairCount;

    private int[][] _stickerCounts = null!;
    private string _sortedTarget = null!;

    [GlobalSetup]
    public void Setup()
    {
        _stickerCounts = Stickers.Select(BuildCounts).ToArray();
        var target = string.Concat(Enumerable.Repeat("ab", PairCount));
        _sortedTarget = string.Concat(target.OrderBy(c => c));
    }

    [Benchmark(Baseline = true)]
    public int Naive() => Solve(_sortedTarget, NaiveMinFor);

    private int NaiveMinFor(string state) => Solve(state, NaiveMinFor);

    [Benchmark]
    public int Memoized() => Memoizer.Memoize<string, int>(_sortedTarget, Solve);

    private int Solve(string state, Func<string, int> minFor)
    {
        if (state.Length == 0)
        {
            return 0;
        }

        var best = int.MaxValue;

        foreach (var counts in _stickerCounts)
        {
            if (counts[state[0] - 'a'] == 0)
            {
                continue;
            }

            var sub = minFor(ApplySticker(state, counts));

            if (sub != int.MaxValue)
            {
                best = Math.Min(best, sub + 1);
            }
        }

        return best;
    }

    private static string ApplySticker(string state, int[] counts)
    {
        var remainingCounts = (int[])counts.Clone();
        var leftover = new List<char>();

        foreach (var c in state)
        {
            if (remainingCounts[c - 'a'] > 0)
            {
                remainingCounts[c - 'a']--;
            }
            else
            {
                leftover.Add(c);
            }
        }

        return string.Concat(leftover);
    }

    private static int[] BuildCounts(string sticker)
    {
        var counts = new int[26];

        foreach (var c in sticker)
        {
            counts[c - 'a']++;
        }

        return counts;
    }
}
