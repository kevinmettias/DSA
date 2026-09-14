using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.GreatestCommonDivisorTraversal;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are GreatestCommonDivisorTraversalSolution's, the same methods
// GreatestCommonDivisorTraversalTests proves correct. Values are drawn once in
// [GlobalSetup] as products of a small shared prime pool, so real overlaps - and
// therefore real union work - actually occur on both arms. What is measured is the
// O(n^2) pairwise gcd sweep against the O(n*sqrt(maxValue)) per-factor union.
[MemoryDiagnoser]
public class GreatestCommonDivisorTraversalBenchmarks
{
    private static readonly int[] SharedPrimes = [2, 3, 5, 7, 11, 13];

    // LC problem number, reused as the deterministic benchmark seed.
    private const int RandomSeed = 2709;

    [Params(50, 400)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _values = Enumerable.Range(0, Length)
            .Select(_ => SharedPrimes[random.Next(SharedPrimes.Length)] * SharedPrimes[random.Next(SharedPrimes.Length)])
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public bool PairwiseGcdScan() =>
        GreatestCommonDivisorTraversalSolution.CanTraverseAllPairsByPairwiseGcd(_values);

    [Benchmark]
    public bool DisjointSetByPrimeFactor() =>
        GreatestCommonDivisorTraversalSolution.CanTraverseAllPairsByPrimeFactorUnion(_values);
}
