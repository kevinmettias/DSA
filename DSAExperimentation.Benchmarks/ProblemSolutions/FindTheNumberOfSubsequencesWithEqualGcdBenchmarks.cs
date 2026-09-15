using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.FindTheNumberOfSubsequencesWithEqualGcd;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindTheNumberOfSubsequencesWithEqualGcdSolution's,
// the same methods FindTheNumberOfSubsequencesWithEqualGcdTests proves correct
// (TwoSumBenchmarks precedent). Brute force is a genuine 3^n choice tree (each
// element: join seq1, join seq2, or join neither), so Length stays small enough
// for that arm to finish in reasonable time
// (CountTheNumberOfSquareFreeSubsetsBenchmarks' own precedent for "size the
// baseline can survive") - the memoized arm's whole point is that it doesn't care
// how large n gets, only how many distinct (index, gcd1, gcd2) states actually
// occur.
[MemoryDiagnoser]
public class FindTheNumberOfSubsequencesWithEqualGcdBenchmarks
{
    private const int MinValueInclusive = 1;
    private const int MaxValueExclusive = 51;
    private const int Seed = 3336;

    private int[] _nums = [];

    [Params(8, 12)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(MinValueInclusive, MaxValueExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BruteForce() => FindTheNumberOfSubsequencesWithEqualGcdSolution.CountPairsByBruteForce(_nums);

    [Benchmark]
    public int GcdMemoization() => FindTheNumberOfSubsequencesWithEqualGcdSolution.CountPairsByGcdMemoization(_nums);
}
