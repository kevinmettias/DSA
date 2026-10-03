using DSAExperimentation.LeetCode.SearchSuggestionsSystem;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SearchSuggestionsSystemSolution's, the same methods
// SearchSuggestionsSystemSolutionTests proves correct. Catalog size (ProductCount) is held
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
//
// WordLength stops at LC 1268's 1,000-letter search word. LC 1268 also caps the
// catalog's total length at 2 * 10^4 letters, so it holds 19 products: 1,005 letters
// each at the longest word, 19,095 in all. Its products are unique, so a suffix drawn
// a second time is drawn again rather than kept.
public class SearchSuggestionsSystemBenchmarks
{
    private const int ProductCount = 19;
    private const int SuffixLength = 5;
    private const int CatalogSeed = 1;
    private static readonly char[] Alphabet = ['a', 'b', 'c', 'd'];

    private string[] _products = [];

    private string _searchWord = "";
    [Params(50, 1_000)]
    public int WordLength { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(CatalogSeed);
        var sharedPrefix = RandomWord(random, WordLength);
        var catalog = new HashSet<string>();
        var products = new List<string>(ProductCount);

        while (products.Count < ProductCount)
        {
            var product = sharedPrefix + RandomWord(random, SuffixLength);

            if (catalog.Add(product))
            {
                products.Add(product);
            }
        }

        _products = [.. products];
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
