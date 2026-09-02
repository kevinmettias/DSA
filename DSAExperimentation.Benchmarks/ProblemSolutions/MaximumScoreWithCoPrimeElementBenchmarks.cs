using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MaximumScoreWithCoPrimeElement;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaximumScoreWithCoPrimeElementSolution's, the
// same methods MaximumScoreWithCoPrimeElementTests proves correct. Brute force
// re-derives every candidate's conflict count with a fresh O(n) gcd scan, so
// Length/MaxVal stay small enough for that arm to finish in reasonable time;
// the divisor-sieve arm's whole point is that it answers each candidate from
// two precomputed sieves instead.
[MemoryDiagnoser]
public class MaximumScoreWithCoPrimeElementBenchmarks
{
    private const int Seed = 3953; // LC problem number
    private const int MaxVal = 500;

    [Params(100, 800)]
    public int Length;

    private int[] _nums = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _nums = [.. Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxVal + 1))];
    }

    [Benchmark(Baseline = true)]
    public int BruteForce() => MaximumScoreWithCoPrimeElementSolution.MaximumScoreByBruteForce(_nums, MaxVal);

    [Benchmark]
    public int DivisorSieve() => MaximumScoreWithCoPrimeElementSolution.MaximumScoreByDivisorSieve(_nums, MaxVal);
}
