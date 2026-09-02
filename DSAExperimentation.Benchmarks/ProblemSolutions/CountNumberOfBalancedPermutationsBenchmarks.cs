using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.CountNumberOfBalancedPermutations;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CountNumberOfBalancedPermutationsSolution's, the
// same methods CountNumberOfBalancedPermutationsTests proves correct
// (CountAnagramsBenchmarks precedent). Digits are drawn from a small alphabet so
// repeats are common, exercising the DP's inverse-factorial division path
// instead of degenerating to every digit distinct. Length stays small - brute
// force would not finish otherwise, even though the real problem allows up to 80
// digits - the same tradeoff CountAnagramsBenchmarks makes for word length.
[MemoryDiagnoser]
public class CountNumberOfBalancedPermutationsBenchmarks
{
    private const int Seed = 3343; // LC problem number
    private const string Digits = "01234";

    [Params(6, 9)]
    public int Length;

    private string _num = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _num = new string(Enumerable.Range(0, Length).Select(_ => Digits[random.Next(Digits.Length)]).ToArray());
    }

    [Benchmark(Baseline = true)]
    public long BruteForce() => CountNumberOfBalancedPermutationsSolution.CountBalancedPermutationsByBruteForce(_num);

    [Benchmark]
    public long DigitCountDp() => CountNumberOfBalancedPermutationsSolution.CountBalancedPermutationsByDigitCountDp(_num);
}
