using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.LastSubstringInLexicographicalOrder;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are LastSubstringInLexicographicalOrderSolution's, the same
// methods LastSubstringInLexicographicalOrderTests proves correct - pairwise O(n^2)
// suffix comparison against this repo's own SuffixArray, which sorts every suffix in
// O(n log^2 n) and then reads the answer off as its final entry. A small 4-letter
// alphabet makes many suffixes share long common prefixes, forcing both approaches
// through real character-by-character comparison work instead of resolving on the
// first character. [GlobalSetup] generates the text, so text generation is charged to
// setup rather than to the measured methods.
[MemoryDiagnoser]
public class LastSubstringInLexicographicalOrderBenchmarks
{
    private const int AlphabetSize = 4;
    private const int RandomSeed = 1; private string _text = "";

    // unchanged from the pre-migration workload

    [Params(200, 2_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _text = string.Create(Length, random, static (span, rng) =>
        {
            for (var i = 0; i < span.Length; i++)
            {
                span[i] = (char)('a' + rng.Next(0, AlphabetSize));
            }
        });
    }

    [Benchmark(Baseline = true)]
    public string PairwiseComparison() =>
        LastSubstringInLexicographicalOrderSolution.LastSubstringByPairwiseComparison(_text);

    [Benchmark]
    public string SuffixArrayLookup() =>
        LastSubstringInLexicographicalOrderSolution.LastSubstringBySuffixArray(_text);
}
