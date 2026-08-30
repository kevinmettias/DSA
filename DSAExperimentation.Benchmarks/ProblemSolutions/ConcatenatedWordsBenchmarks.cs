using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;
using DSAExperimentation.DataStructures.Trie;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Concatenated Words (LC 472): a plain HashSet-membership scan (every start index
// rescans candidate end positions all the way to the word's length, with no way to
// know a prefix is already dead) vs. this repo's own Trie<bool> + Memoizer, where
// Trie.HasPrefix lets the scan break out the moment no dictionary word starts with
// the current prefix - the WordBreakII precedent, with one extra rule: the first
// piece can never consume the whole word (forcing at least two pieces). _word tiles
// a single short dictionary word so both strategies reach the identical
// classification, isolating the segmentation-scan cost itself.
[MemoryDiagnoser]
public class ConcatenatedWordsBenchmarks
{
    private static readonly string[] Dictionary = ["cat"];

    [Params(600, 3000)]
    public int Length;

    private string _word = null!;

    [GlobalSetup]
    public void Setup() => _word = string.Concat(Enumerable.Repeat("cat", Length / 3));

    [Benchmark(Baseline = true)]
    public bool HashSetUnboundedScan()
    {
        var words = Dictionary.ToHashSet();
        var dp = new bool[_word.Length + 1];
        dp[0] = true;

        for (var end = 1; end <= _word.Length; end++)
        {
            for (var start = 0; start < end; start++)
            {
                if (start == 0 && end == _word.Length)
                {
                    continue;
                }

                if (dp[start] && words.Contains(_word[start..end]))
                {
                    dp[end] = true;
                    break;
                }
            }
        }

        return dp[_word.Length];
    }

    [Benchmark]
    public bool TriePrunedMemoized()
    {
        var trie = new Trie<bool>();
        foreach (var word in Dictionary)
        {
            trie.Set(word, true);
        }

        return Memoizer.Memoize<int, bool>(0, From);

        bool From(int start, Func<int, bool> can)
        {
            if (start == _word.Length)
            {
                return true;
            }

            for (var end = start + 1; end <= _word.Length; end++)
            {
                if (start == 0 && end == _word.Length)
                {
                    continue;
                }

                var piece = _word[start..end];

                if (!trie.HasPrefix(piece))
                {
                    break;
                }

                if (trie.HasKey(piece) && can(end))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
