using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.RollingHash;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Maximum Deletions on a String (LC 2430): dp[i] = 1 + the best dp[i+k] over every
// valid duplicate-prefix half-length k in [1, (n-i)/2] (or 1 alone when none exist,
// matching a single "delete the rest of s" operation) - the baseline recomputes
// each half-equality with .Substring + == (paying O(k) allocation+compare on every
// candidate, match or not), the RollingHash version screens with an O(1) hash
// compare first and only pays for a real SequenceEqual once the screen passes,
// MaximumDeletionsOnAStringTests' exact composition (DistinctEchoSubstringsBenchmarks'
// same screen-then-verify shape, applied to a DP transition instead of a dedup set).
// _text is random lowercase letters, so most candidate k's fail fast - the shape
// where the screen avoids the baseline's per-candidate substring allocation instead
// of merely relocating the same cost.
[MemoryDiagnoser]
public class MaximumDeletionsOnAStringBenchmarks
{
    private const int RandomSeed = 2430; // LC problem number
    private const int LowercaseAlphabetSize = 26;

    [Params(80, 400)]
    public int Length;

    private string _text = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var chars = new char[Length];

        for (var i = 0; i < Length; i++)
        {
            chars[i] = (char)('a' + random.Next(LowercaseAlphabetSize));
        }

        _text = new string(chars);
    }

    [Benchmark(Baseline = true)]
    public int NaiveSubstringComparison()
    {
        var n = _text.Length;
        var dp = new int[n + 1];

        for (var i = n - 1; i >= 0; i--)
        {
            var best = 0;

            for (var k = 1; i + (2 * k) <= n; k++)
            {
                var first = _text.Substring(i, k);
                var second = _text.Substring(i + k, k);

                if (first == second)
                {
                    best = Math.Max(best, dp[i + k]);
                }
            }

            dp[i] = 1 + best;
        }

        return dp[0];
    }

    [Benchmark]
    public int RollingHashScreenedDp()
    {
        var n = _text.Length;
        var hash = new RollingHash(_text);
        var dp = new int[n + 1];

        for (var i = n - 1; i >= 0; i--)
        {
            var best = 0;

            for (var k = 1; i + (2 * k) <= n; k++)
            {
                var firstHalf = _text.AsSpan(i, k);
                var secondHalf = _text.AsSpan(i + k, k);

                if (hash.Hash(i, k) == hash.Hash(i + k, k) && firstHalf.SequenceEqual(secondHalf))
                {
                    best = Math.Max(best, dp[i + k]);
                }
            }

            dp[i] = 1 + best;
        }

        return dp[0];
    }
}
