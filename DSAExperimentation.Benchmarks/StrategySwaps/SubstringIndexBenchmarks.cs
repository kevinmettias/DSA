using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.Domain.Science.Dna;
using SuffixArrayIndex = DSAExperimentation.DataStructures.SuffixArray.SuffixArrayIndex;
using SuffixAutomatonStructure = DSAExperimentation.DataStructures.SuffixAutomaton.SuffixAutomaton;
using SuffixTreeStructure = DSAExperimentation.DataStructures.SuffixTree.SuffixTree;

namespace DSAExperimentation.Benchmarks.StrategySwaps;

// The three substring indexes over one random DNA text, compared on the operations all three answer
// (ARCHITECTURE §8): presence and counting for a batch of patterns, the longest repeat, and
// building the index itself. The suffix array searches in O(m log n) per pattern over two int
// arrays and the text; the tree and the automaton walk in O(m), each node through a hash map. Half
// the patterns are drawn from the text and half at random from the same four letters, so most of
// the long random ones are absent and end their walk or search early.
//
// The arms are named for what each structure is: the suffixes sorted, the trie of them compressed,
// and the minimal automaton of the text's substrings. Each arm calls its concrete type, so each
// batch loop is written once per structure, overloaded by the structure's type. One loop generic
// over the contract read worse in the tier-1 code, even inlined into the arms: the structures are
// classes, so the loop is shared generic code, and each call went through the interface, guarded by
// a profile-guided type check at best.
public class SubstringIndexBenchmarks
{
    internal const int TextSeed = 1044;
    private const int PatternSeed = 3213;
    private const int PatternCount = 1_000;
    private const int LongestPattern = 16;

    private string _text = string.Empty;
    private string[] _patterns = [];
    private SuffixArrayIndex _suffixArray = null!;
    private SuffixTreeStructure _suffixTree = null!;
    private SuffixAutomatonStructure _suffixAutomaton = null!;

    [Params(
        SubstringIndexQuery.HasSubstring,
        SubstringIndexQuery.CountOccurrences,
        SubstringIndexQuery.FindLongestRepeat,
        SubstringIndexQuery.Build)]
    public SubstringIndexQuery Query { get; set; }

    [Params(1_000, 100_000)]
    public int TextLength { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _text = DnaSequenceWorkloads.BuildSequence(TextLength, TextSeed);
        _patterns = DrawPatterns(_text);
        _suffixArray = new SuffixArrayIndex(_text);
        _suffixTree = new SuffixTreeStructure(_text);
        _suffixAutomaton = new SuffixAutomatonStructure(_text);
    }

    [Benchmark(Baseline = true)]
    public long SortedSuffixArray() => Query switch
    {
        SubstringIndexQuery.HasSubstring => CountPresent(_suffixArray),
        SubstringIndexQuery.CountOccurrences => SumOccurrences(_suffixArray),
        SubstringIndexQuery.FindLongestRepeat =>
            _suffixArray.SuffixArray.FindLongestRepeat().Length,
        SubstringIndexQuery.Build => new SuffixArrayIndex(_text).CountOccurrences(string.Empty),
        _ => Unsupported(),
    };

    [Benchmark]
    public long CompressedSuffixTree() => Query switch
    {
        SubstringIndexQuery.HasSubstring => CountPresent(_suffixTree),
        SubstringIndexQuery.CountOccurrences => SumOccurrences(_suffixTree),
        SubstringIndexQuery.FindLongestRepeat => _suffixTree.FindLongestRepeat().Length,
        SubstringIndexQuery.Build => new SuffixTreeStructure(_text).CountOccurrences(string.Empty),
        _ => Unsupported(),
    };

    [Benchmark]
    public long MinimalSuffixAutomaton() => Query switch
    {
        SubstringIndexQuery.HasSubstring => CountPresent(_suffixAutomaton),
        SubstringIndexQuery.CountOccurrences => SumOccurrences(_suffixAutomaton),
        SubstringIndexQuery.FindLongestRepeat => _suffixAutomaton.FindLongestRepeat().Length,
        SubstringIndexQuery.Build =>
            new SuffixAutomatonStructure(_text).CountOccurrences(string.Empty),
        _ => Unsupported(),
    };

    // Even draws are substrings of the text at a random place, odd draws random letters; both take
    // a random length from 1 to LongestPattern.
    internal static string[] DrawPatterns(string text)
    {
        var random = new Random(PatternSeed);
        var patterns = new string[PatternCount];

        for (var i = 0; i < PatternCount; i++)
        {
            var longest = Math.Min(LongestPattern, text.Length);
            var length = 1 + random.Next(longest);
            patterns[i] = int.IsEvenInteger(i)
                ? Substring(random, text, length)
                : RandomLetters(random, length);
        }

        return patterns;
    }

    private static string Substring(Random random, string text, int length) =>
        text.Substring(random.Next(text.Length - length + 1), length);

    private static string RandomLetters(Random random, int length)
    {
        var letters = new char[length];

        for (var i = 0; i < length; i++)
        {
            letters[i] = DnaBases.Letters[random.Next(DnaBases.Letters.Length)];
        }

        return new string(letters);
    }

    private static long Unsupported() =>
        throw new InvalidOperationException("Every SubstringIndexQuery has an arm.");

    private long CountPresent(SuffixArrayIndex index)
    {
        long present = 0;

        foreach (var pattern in _patterns)
        {
            if (index.HasSubstring(pattern))
            {
                present++;
            }
        }

        return present;
    }

    private long CountPresent(SuffixTreeStructure index)
    {
        long present = 0;

        foreach (var pattern in _patterns)
        {
            if (index.HasSubstring(pattern))
            {
                present++;
            }
        }

        return present;
    }

    private long CountPresent(SuffixAutomatonStructure index)
    {
        long present = 0;

        foreach (var pattern in _patterns)
        {
            if (index.HasSubstring(pattern))
            {
                present++;
            }
        }

        return present;
    }

    private long SumOccurrences(SuffixArrayIndex index)
    {
        long occurrences = 0;

        foreach (var pattern in _patterns)
        {
            occurrences += index.CountOccurrences(pattern);
        }

        return occurrences;
    }

    private long SumOccurrences(SuffixTreeStructure index)
    {
        long occurrences = 0;

        foreach (var pattern in _patterns)
        {
            occurrences += index.CountOccurrences(pattern);
        }

        return occurrences;
    }

    private long SumOccurrences(SuffixAutomatonStructure index)
    {
        long occurrences = 0;

        foreach (var pattern in _patterns)
        {
            occurrences += index.CountOccurrences(pattern);
        }

        return occurrences;
    }
}
