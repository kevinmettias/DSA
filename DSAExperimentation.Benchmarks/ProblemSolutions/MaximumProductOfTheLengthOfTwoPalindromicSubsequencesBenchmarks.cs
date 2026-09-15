using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MaximumProductOfTheLengthOfTwoPalindromicSubsequences;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are
// MaximumProductOfTheLengthOfTwoPalindromicSubsequencesSolution's, the same methods
// MaximumProductOfTheLengthOfTwoPalindromicSubsequencesTests proves correct. LeetCode caps
// s.Length at 12, so the intended solution is genuinely exponential and what is measured is
// hand-rolled bitmask enumeration against this repo's own Backtrack.Search choose/explore/
// unchoose walk over the identical 2^n subsequence space. The string is drawn once in
// [GlobalSetup] from a four-letter alphabet so palindromic subsequences are common enough
// that the shared disjoint-pair scan does real work.
[MemoryDiagnoser]
public class MaximumProductOfTheLengthOfTwoPalindromicSubsequencesBenchmarks
{
    private const int AlphabetSize = 4; // characters are drawn from 'a'-'d'

    // Deterministic workload seed, kept at the value this benchmark has always used.
    private const int RandomSeed = 1;

    private string _value = "";

    [Params(8, 12)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _value = new string(Enumerable.Range(0, Length).Select(_ => (char)('a' + random.Next(AlphabetSize))).ToArray());
    }

    [Benchmark(Baseline = true)]
    public int BruteForce() =>
        MaximumProductOfTheLengthOfTwoPalindromicSubsequencesSolution.MaxProductByBitmaskScan(_value);

    [Benchmark]
    public int Backtracking() =>
        MaximumProductOfTheLengthOfTwoPalindromicSubsequencesSolution.MaxProductByBacktrack(_value);
}
