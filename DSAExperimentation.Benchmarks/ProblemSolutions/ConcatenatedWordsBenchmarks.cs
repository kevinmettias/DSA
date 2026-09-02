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
    private const string DictionaryWord = "cat";
    private const int DictionaryWordLength = 3;

    private static readonly string[] Dictionary = [DictionaryWord];

    [Params(600, 3000)]
    public int Length;

    private string _word = null!;

    [GlobalSetup]
    public void Setup()
    {
        var tiles = Enumerable.Repeat(DictionaryWord, Length / DictionaryWordLength);
        _word = string.Concat(tiles);
    }

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
        var trie = BuildDictionaryTrie();
        return Memoizer.Memoize<int, bool>(0, (start, can) => From(start, can, trie));
    }

    private static Trie<bool> BuildDictionaryTrie()
    {
        var trie = new Trie<bool>();
        foreach (var word in Dictionary)
        {
            trie.Set(word, true);
        }

        return trie;
    }

    private bool From(int start, Func<int, bool> can, Trie<bool> trie)
    {
        if (start == _word.Length)
        {
            return true;
        }

        for (var end = start + 1; end <= _word.Length; end++)
        {
            var outcome = ProbePiece(trie, start, end, can);
            if (outcome == PieceOutcome.NoDictionaryPrefix)
            {
                break;
            }

            if (outcome == PieceOutcome.RemainderDecomposes)
            {
                return true;
            }
        }

        return false;
    }

    // The per-candidate-end decision from TriePrunedMemoized's inner loop: whether to
    // keep extending the current piece, give up on this start index entirely (no
    // dictionary word begins with this prefix), or a valid split was just found.
    private PieceOutcome ProbePiece(Trie<bool> trie, int start, int end, Func<int, bool> can)
    {
        if (start == 0 && end == _word.Length)
        {
            return PieceOutcome.SkipWholeWord;
        }

        var piece = _word[start..end];

        if (!trie.HasPrefix(piece))
        {
            return PieceOutcome.NoDictionaryPrefix;
        }

        return trie.HasKey(piece) && can(end) ? PieceOutcome.RemainderDecomposes : PieceOutcome.SkipWholeWord;
    }

    private enum PieceOutcome
    {
        SkipWholeWord,
        NoDictionaryPrefix,
        RemainderDecomposes
    }
}
