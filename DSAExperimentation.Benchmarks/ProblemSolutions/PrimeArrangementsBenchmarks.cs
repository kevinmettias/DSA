using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.PrimeArrangements;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are PrimeArrangementsSolution's, the same methods
// PrimeArrangementsTests proves correct. n is the whole input, so there is nothing
// to hoist into a [GlobalSetup] - the [Params] sizes are the workload.
[MemoryDiagnoser]
public class PrimeArrangementsBenchmarks
{
    [Params(2_000, 20_000)]
    public int N;

    [Benchmark(Baseline = true)]
    public int TrialDivision() => PrimeArrangementsSolution.NumPrimeArrangementsByTrialDivision(N);

    [Benchmark]
    public int SieveOfEratosthenes() =>
        PrimeArrangementsSolution.NumPrimeArrangementsBySieveOfEratosthenes(N);
}
