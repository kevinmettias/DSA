using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.SearchSuggestionsSystem;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SearchSuggestionsSystemSolution's, the same methods
// SearchSuggestionsSystemTests proves correct. Catalog size (ProductCount) is held
// fixed and only searchWord length (WordLength, [Params]) varies, because that is
// the axis this gap is actually on: with a short searchWord the per-keystroke
// catalog rescan is already just O(products), no worse than the one-time
// O(products log products) sort - it is a long searchWord (many keystrokes) that
// makes paying for every one of them with a fresh full-catalog scan expensive,
// which the sort-once approach avoids. Every product deliberately shares the
// entire searchWord as a literal prefix (plus a short random distinguishing
// suffix) so every keystroke's scan matches the whole catalog instead of failing
// fast on an early character mismatch - the same "force the real worst case"
// convention TwoSumBenchmarks uses, here applied to StartsWith instead of a sum
// target.
[MemoryDiagnoser]
public class SearchSuggestionsSystemBenchmarks
{
    private const int ProductCount = 2_000;
    private const int SuffixLength = 5;
    private const int CatalogSeed = 1;
    private static readonly char[] Alphabet = ['a', 'b', 'c', 'd'];

    private string[] _products = [];

    private string _searchWord = "";
    [Params(50, 400)]
    public int WordLength { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(CatalogSeed);
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
    public List<string[]> LinearScanPerKeystroke() =>
        SearchSuggestionsSystemSolution.SuggestedProductsByCatalogScan(_products, _searchWord);

    [Benchmark]
    public List<string[]> SortOnceThenBinarySearchPerKeystroke() =>
        SearchSuggestionsSystemSolution.SuggestedProductsBySortedPrefixSearch(_products, _searchWord);
}
