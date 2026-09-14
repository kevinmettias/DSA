using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MaximumProductOfTheLengthOfTwoPalindromicSubstrings;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are
// MaximumProductOfTheLengthOfTwoPalindromicSubstringsSolution's, the same methods
// MaximumProductOfTheLengthOfTwoPalindromicSubstringsTests proves correct - the
// O(n^3) baseline that re-derives the longest odd palindrome on each side of every
// split from scratch against the single O(n) Manacher.ComputeOddRadii pass plus a
// forward and backward sweep over its per-center radii. _text is all one repeated
// character, so every expansion in the baseline runs to the actual boundary instead
// of exiting after one comparison, forcing both strategies through genuinely large
// palindromes instead of an early exit making the naive version look artificially
// competitive.
[MemoryDiagnoser]
public class MaximumProductOfTheLengthOfTwoPalindromicSubstringsBenchmarks
{
    [Params(30, 90)]
    public int Length;

    private string _text = null!;

    [GlobalSetup]
    public void Setup() => _text = new string('a', Length);

    [Benchmark(Baseline = true)]
    public long NaiveSplitScan() =>
        MaximumProductOfTheLengthOfTwoPalindromicSubstringsSolution.MaxProductByCenterExpansion(_text);

    [Benchmark]
    public long ManacherSplitScan() =>
        MaximumProductOfTheLengthOfTwoPalindromicSubstringsSolution.MaxProductByManacherRadii(_text);
}
