using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.CountPrimes;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CountPrimesSolution's, the same methods
// CountPrimesTests proves correct.
[MemoryDiagnoser]
public class CountPrimesBenchmarks
{
    [Params(2_000, 20_000)]
    public int N { get; set; }

    [Benchmark(Baseline = true)]
    public int TrialDivision() => CountPrimesSolution.CountPrimesByTrialDivision(N);

    [Benchmark]
    public int SieveOfEratosthenes() => CountPrimesSolution.CountPrimesBySieveOfEratosthenes(N);
}
