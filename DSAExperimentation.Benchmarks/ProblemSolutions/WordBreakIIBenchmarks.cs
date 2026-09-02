using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;
using DSAExperimentation.DataStructures.Trie;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Word Break II (LC 140): a plain HashSet-membership scan (every start index
// rescans candidate end positions all the way to s.Length, with no way to
// know a prefix is already dead) vs. this repo's own Trie<bool> + Memoizer,
// where Trie.HasPrefix lets the scan break out the moment no dictionary word
// starts with the current prefix. _s tiles a single short dictionary word so
// both strategies reach the identical unique sentence, isolating the
// segmentation-scan cost itself rather than sentence-construction cost.
[MemoryDiagnoser]
public class WordBreakIIBenchmarks
{
    private const string RepeatedWord = "cat";
    private const string WordSeparator = " ";

    private static readonly string[] Dictionary = [RepeatedWord];

    [Params(600, 3000)]
    public int Length;

    private string _s = null!;

    [GlobalSetup]
    public void Setup()
    {
        var repeatedWords = Enumerable.Repeat(RepeatedWord, Length / RepeatedWord.Length);
        _s = string.Concat(repeatedWords);
    }

    [Benchmark(Baseline = true)]
    public int HashSetUnboundedScan()
    {
        var words = Dictionary.ToHashSet();
        var memo = new Dictionary<int, List<string>>();

        return BuildSentencesHashSet(0, words, memo).Count;
    }

    private List<string> BuildSentencesHashSet(int start, HashSet<string> words, Dictionary<int, List<string>> memo)
    {
        if (memo.TryGetValue(start, out var cached))
        {
            return cached;
        }

        if (start == _s.Length)
        {
            return [string.Empty];
        }

        var sentences = CollectSentencesForStart(start, words, memo);

        return memo[start] = sentences;
    }

    private List<string> CollectSentencesForStart(int start, HashSet<string> words, Dictionary<int, List<string>> memo)
    {
        var sentences = new List<string>();

        for (var end = start + 1; end <= _s.Length; end++)
        {
            var word = _s[start..end];

            if (!words.Contains(word))
            {
                continue;
            }

            foreach (var suffix in BuildSentencesHashSet(end, words, memo))
            {
                AppendSentence(sentences, word, suffix);
            }
        }

        return sentences;
    }

    [Benchmark]
    public int TriePrunedMemoized()
    {
        var trie = new Trie<bool>();
        foreach (var word in Dictionary)
        {
            trie.Set(word, true);
        }

        List<string> From(int start, Func<int, List<string>> from) => BuildSentencesTrie(start, from, trie);

        return Memoizer.Memoize<int, List<string>>(0, From).Count;
    }

    private List<string> BuildSentencesTrie(int start, Func<int, List<string>> from, Trie<bool> trie)
    {
        if (start == _s.Length)
        {
            return [string.Empty];
        }

        var sentences = new List<string>();

        for (var end = start + 1; end <= _s.Length; end++)
        {
            if (TryExtendMatch(trie, (start, end), from, sentences))
            {
                break;
            }
        }

        return sentences;
    }

    private bool TryExtendMatch(Trie<bool> trie, (int Start, int End) span, Func<int, List<string>> from, List<string> sentences)
    {
        var piece = _s[span.Start..span.End];

        if (!trie.HasPrefix(piece))
        {
            return true;
        }

        if (trie.HasKey(piece))
        {
            foreach (var suffix in from(span.End))
            {
                AppendSentence(sentences, piece, suffix);
            }
        }

        return false;
    }

    private static void AppendSentence(List<string> sentences, string word, string suffix)
        => sentences.Add(suffix.Length == 0 ? word : word + WordSeparator + suffix);
}
