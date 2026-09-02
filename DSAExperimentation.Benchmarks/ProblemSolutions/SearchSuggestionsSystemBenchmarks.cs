using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Search Suggestions System (LC 1268): a linear scan of every product per typed
// character (each keystroke re-walks the whole catalog, keeping only the three
// smallest matches seen) vs. sorting the catalog once with this repo's own
// MergeSort.Sort<string, ArrayIndexedSequence<string>> and then using
// BinarySearch.LowerBound<string, ArraySequence<string>> to jump straight to where
// each growing prefix starts. Catalog size (ProductCount) is held fixed and only
// searchWord length (WordLength, [Params]) varies, because that is the axis this
// gap is actually on: with a short searchWord the O(products) linear rescan is
// already just O(products), no worse than the O(products log products) one-time
// sort - it's a long searchWord (many keystrokes) that makes paying for every one
// of them with a fresh full-catalog scan expensive, which the sort-once approach
// avoids. Every product deliberately shares the entire searchWord as a literal
// prefix (plus a short random distinguishing suffix) so every keystroke's scan
// matches the whole catalog instead of failing fast on an early character
// mismatch - the same "force the real worst case" convention TwoSumBenchmarks
// uses, here applied to StartsWith instead of a sum target.
[MemoryDiagnoser]
public class SearchSuggestionsSystemBenchmarks
{
    private const int ProductCount = 2_000;
    private const int SuffixLength = 5;
    private const int MaxSuggestions = 3;
    private static readonly char[] Alphabet = ['a', 'b', 'c', 'd'];

    [Params(50, 400)]
    public int WordLength;

    private string[] _products = null!;
    private string _searchWord = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        var sharedPrefix = RandomWord(random, WordLength);
        _products = new string[ProductCount];

        for (var i = 0; i < ProductCount; i++)
        {
            _products[i] = sharedPrefix + RandomWord(random, SuffixLength);
        }

        _searchWord = sharedPrefix;
    }

    private static string RandomWord(Random random, int length)
    {
        var chars = new char[length];

        for (var i = 0; i < length; i++)
        {
            chars[i] = Alphabet[random.Next(Alphabet.Length)];
        }

        return new string(chars);
    }

    [Benchmark(Baseline = true)]
    public List<string[]> LinearScanPerKeystroke()
    {
        var result = new List<string[]>();
        var prefix = string.Empty;

        foreach (var ch in _searchWord)
        {
            prefix += ch;
            result.Add(TopThreeMatches(prefix));
        }

        return result;
    }

    private string[] TopThreeMatches(string prefix)
    {
        var top = new List<string>(MaxSuggestions);

        foreach (var product in _products)
        {
            if (product.StartsWith(prefix, StringComparison.Ordinal))
            {
                InsertIfAmongSmallestThree(top, product);
            }
        }

        return [.. top];
    }

    private static void InsertIfAmongSmallestThree(List<string> top, string candidate)
    {
        var insertAt = top.Count;

        while (insertAt > 0 && string.CompareOrdinal(top[insertAt - 1], candidate) > 0)
        {
            insertAt--;
        }

        if (insertAt >= MaxSuggestions)
        {
            return;
        }

        top.Insert(insertAt, candidate);

        if (top.Count > MaxSuggestions)
        {
            top.RemoveAt(MaxSuggestions);
        }
    }

    [Benchmark]
    public List<string[]> SortOnceThenBinarySearchPerKeystroke()
    {
        var sorted = (string[])_products.Clone();
        MergeSort.Sort<string, ArrayIndexedSequence<string>>(new ArrayIndexedSequence<string>(sorted), StringComparer.Ordinal);

        var sequence = new ArraySequence<string>(sorted);
        var result = new List<string[]>();
        var prefix = string.Empty;

        foreach (var ch in _searchWord)
        {
            prefix += ch;
            var matches = MatchesForPrefix(sorted, sequence, prefix);
            result.Add(matches);
        }

        return result;
    }

    private static string[] MatchesForPrefix(string[] sorted, ArraySequence<string> sequence, string prefix)
    {
        var start = BinarySearch.LowerBound(sequence, prefix, StringComparer.Ordinal);
        var matches = new List<string>(MaxSuggestions);

        for (var i = start; i < sorted.Length && matches.Count < MaxSuggestions; i++)
        {
            if (!sorted[i].StartsWith(prefix, StringComparison.Ordinal))
            {
                break;
            }

            matches.Add(sorted[i]);
        }

        return [.. matches];
    }
}
