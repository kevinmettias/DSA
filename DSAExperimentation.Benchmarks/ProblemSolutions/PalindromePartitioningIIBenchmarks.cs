using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.PalindromePartitioningII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are PalindromePartitioningIISolution's, the same methods
// PalindromePartitioningIISolutionTests proves correct. The original benchmark's two
// [Benchmark] arms (Baseline, PrimitiveComposed) were both compile-smoke
// placeholders (`=> 1`) - one real strategy, not two - so this measures the
// memoized recurrence against the bottom-up cut table instead. The string's letters
// are drawn from a seeded Random over "ab": two letters rather than LC 132's 26
// leave palindromes of every short length throughout, so neither strategy's
// palindrome checks fail at once. Length stops at LC 132's 2,000-letter cap.
public class PalindromePartitioningIIBenchmarks
{
    private const int RandomSeed = 132; // LC problem number
    private const string Alphabet = "ab";

    private string _text = "";

    [Params(200, 2_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup() =>
        _text = new string([.. SeededDraws.Values(Length, 0, Alphabet.Length, new Random(RandomSeed)).Select(index => Alphabet[index])]);

    [Benchmark(Baseline = true)]
    public int MemoizedSuffixRecurrence() =>
        PalindromePartitioningIISolution.MinCutByMemoizedSuffixRecurrence(_text);

    [Benchmark]
    public int IterativeDynamicProgramming() =>
        PalindromePartitioningIISolution.MinCutByIterativeDynamicProgramming(_text);
}
