using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.LongestHappyPrefix;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are LongestHappyPrefixSolution's, the same methods
// LongestHappyPrefixTests proves correct - the textbook O(n^2) shrink-and-compare
// check vs. this repo's own KMP prefix/failure function, which gets the answer
// directly from its last entry in a single O(n) pass.
//
// An (n-1)-run of 'a' followed by one 'b' is the naive approach's worst case: every
// candidate length's prefix/suffix comparison agrees on every character except the
// last, forcing a near-full-length scan before the mismatch is found, for almost
// every one of the n-1 candidate lengths - true O(n^2) work, not the O(n) an
// all-'a' string would give it (its very first, longest candidate would match
// immediately). It also makes the answer the empty prefix, so neither arm is
// charged for a substring allocation the other avoids.
[MemoryDiagnoser]
public class LongestHappyPrefixBenchmarks
{
    private const string MismatchSuffix = "b"; // forces every candidate length's comparison to fail only on the last character

    [Params(200, 5_000)]
    public int Length;

    private string _value = null!;

    [GlobalSetup]
    public void Setup() => _value = new string('a', Length - 1) + MismatchSuffix;

    [Benchmark(Baseline = true)]
    public string ShrinkAndCompare() =>
        LongestHappyPrefixSolution.LongestPrefixByShrinkAndCompare(_value);

    [Benchmark]
    public string PrefixFunctionLookup() =>
        LongestHappyPrefixSolution.LongestPrefixByPrefixFunction(_value);
}
