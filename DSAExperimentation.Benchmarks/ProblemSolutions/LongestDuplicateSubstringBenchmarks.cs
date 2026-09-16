using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.LongestDuplicateSubstring;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are LongestDuplicateSubstringSolution's. A small alphabet
// makes duplicates plentiful and long, so the all-pairs baseline pays for real
// character scans rather than failing on the first character every time.
[MemoryDiagnoser]
public class LongestDuplicateSubstringBenchmarks
{
    private const int AlphabetSize = 4;
    private const int RandomSeed = 1044; private string _text = "";

    // LC problem number

    [Params(200, 2_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _text = new string(Enumerable.Range(0, Length).Select(_ => (char)('a' + random.Next(AlphabetSize))).ToArray());
    }

    [Benchmark(Baseline = true)]
    public string AllSuffixPairsBruteForce() =>
        LongestDuplicateSubstringSolution.LongestDuplicateSubstringByAllSuffixPairs(_text);

    [Benchmark]
    public string SuffixArrayLongestCommonPrefix() =>
        LongestDuplicateSubstringSolution.LongestDuplicateSubstringBySuffixArray(_text);
}
