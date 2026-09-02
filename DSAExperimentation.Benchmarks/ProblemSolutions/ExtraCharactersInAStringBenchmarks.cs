using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;
using DSAExperimentation.DataStructures.Trie;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Extra Characters in a String (LC 2707): both arms fill the same
// fewest-leftover-characters recurrence bottom-up, differing only in how a start
// position finds the dictionary words beginning there. HashSetFullScan checks every
// end position up to the string's own length against a HashSet<string>, extracting
// and hashing a full substring even when no dictionary word could possibly match.
// TriePrunedScan instead walks a Trie<bool> one character at a time and stops the
// instant no dictionary word shares that prefix, bounding the inner loop by the
// longest dictionary word instead of by the remaining string length.
[MemoryDiagnoser]
public class ExtraCharactersInAStringBenchmarks
{
    private static readonly string[] Dictionary = ["ab", "cd", "ef", "gh", "ij"];

    [Params(300, 1_500)]
    public int Length;

    private string _s = null!;

    [GlobalSetup]
    public void Setup()
    {
        // A repeating run of characters that never form a dictionary word, so
        // neither arm gets an early exact-match shortcut - both are forced through
        // their full per-start scan strategy.
        var chars = new char[Length];
        for (var i = 0; i < Length; i++)
        {
            chars[i] = (char)('k' + i % 5);
        }

        _s = new string(chars);
    }

    [Benchmark(Baseline = true)]
    public int HashSetFullScan()
    {
        var dictionary = new HashSet<string>(Dictionary);
        var n = _s.Length;
        var dp = new int[n + 1];

        for (var start = n - 1; start >= 0; start--)
        {
            var best = 1 + dp[start + 1];

            for (var end = start + 1; end <= n; end++)
            {
                if (dictionary.Contains(_s[start..end]))
                {
                    best = Math.Min(best, dp[end]);
                }
            }

            dp[start] = best;
        }

        return dp[0];
    }

    [Benchmark]
    public int TriePrunedScan()
    {
        var trie = new Trie<bool>();
        foreach (var word in Dictionary)
        {
            trie.Set(word, true);
        }

        return Memoizer.Memoize<int, int>(0, From);

        int From(int start, Func<int, int> min)
        {
            if (start == _s.Length)
            {
                return 0;
            }

            var best = 1 + min(start + 1);

            for (var end = start + 1; end <= _s.Length; end++)
            {
                var piece = _s[start..end];
                if (!trie.HasPrefix(piece))
                {
                    break;
                }

                if (trie.HasKey(piece))
                {
                    best = Math.Min(best, min(end));
                }
            }

            return best;
        }
    }
}
