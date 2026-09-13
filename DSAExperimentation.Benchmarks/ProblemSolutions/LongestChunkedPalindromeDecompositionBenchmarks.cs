using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.LongestChunkedPalindromeDecomposition;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are LongestChunkedPalindromeDecompositionSolution's, the
// same strategies LongestChunkedPalindromeDecompositionTests proves correct. Setup
// builds a text whose characters are all distinct (Unicode code points starting at
// 1000, so no accidental match ever fires), the same "force the real worst case"
// intent TwoSumBenchmarks' own setup comment names - both strategies are forced to
// grow their pending window all the way to the middle, which is exactly where the
// string-concatenation arm's repeated O(len) build+compare costs the most.
[MemoryDiagnoser]
public class LongestChunkedPalindromeDecompositionBenchmarks
{
    private const int CodePointBase = 1000;

    [Params(200, 2_000)]
    public int Length;

    private string _text = null!;

    [GlobalSetup]
    public void Setup()
    {
        var chars = new char[Length];

        for (var i = 0; i < Length; i++)
        {
            chars[i] = (char)(CodePointBase + i);
        }

        _text = new string(chars);
    }

    [Benchmark(Baseline = true)]
    public int StringConcatenation() =>
        LongestChunkedPalindromeDecompositionSolution.LongestDecompositionByStringConcatenation(_text);

    [Benchmark]
    public int RollingHashChunking() =>
        LongestChunkedPalindromeDecompositionSolution.LongestDecompositionByRollingHash(_text);
}
