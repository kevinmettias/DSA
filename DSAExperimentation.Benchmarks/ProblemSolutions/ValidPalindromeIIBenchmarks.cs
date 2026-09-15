using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.ValidPalindromeII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Valid Palindrome II (LC 680): harness only - both arms are
// ValidPalindromeIISolution's, the same methods ValidPalindromeIITests proves
// correct. _s places two differing characters symmetrically off-center so the
// initial scan runs a genuine O(n) distance before finding the mismatch,
// instead of collapsing to O(1) at either end.
[MemoryDiagnoser]
public class ValidPalindromeIIBenchmarks
{
    private const int OffsetDivisor = 3;

    private string _s = "";

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup() => _s = BuildInput(Length);

    private static string BuildInput(int length)
    {
        var chars = new char[length];
        Array.Fill(chars, 'a');

        var mid1 = length / OffsetDivisor;
        var mid2 = length - 1 - mid1;
        chars[mid1] = 'b';
        chars[mid2] = 'c';

        return new string(chars);
    }

    [Benchmark(Baseline = true)]
    public bool TryEachSingleDeletion() => ValidPalindromeIISolution.IsValidPalindromeByBruteForceDeletion(_s);

    [Benchmark]
    public bool MismatchSkipTwoPointer() => ValidPalindromeIISolution.IsValidPalindromeByMismatchSkip(_s);
}
