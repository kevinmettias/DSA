using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.StringMatching;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Longest Happy Prefix (LC 1392): the textbook O(n^2) shrink-and-compare check
// (try every prefix length from n-1 down to 1, comparing that many characters
// each time) vs. this repo's own KMP prefix/failure function
// (PrefixFunctionSearch.ComputeFailureFunction), which gets the answer directly
// from its last entry in a single O(n) pass. An (n-1)-run of 'a' followed by one
// 'b' is the naive approach's worst case: every candidate length's prefix/suffix
// comparison agrees on every character except the last, forcing a near-full-length
// scan before the mismatch is found, for almost every one of the n-1 candidate
// lengths - true O(n^2) work, not the O(n) an all-'a' string would give it (its
// very first, longest candidate would match immediately).
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
    public int ShrinkAndCompare()
    {
        var s = _value;

        for (var length = s.Length - 1; length >= 1; length--)
        {
            var prefix = s.AsSpan(0, length);
            var suffix = s.AsSpan(s.Length - length, length);

            if (prefix.SequenceEqual(suffix))
            {
                return length;
            }
        }

        return 0;
    }

    [Benchmark]
    public int PrefixFunctionLookup()
    {
        var failure = PrefixFunctionSearch.ComputeFailureFunction(_value);
        return failure.Length == 0 ? 0 : failure[^1];
    }
}
