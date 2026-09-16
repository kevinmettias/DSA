using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.PrimeArrangements;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are PrimeArrangementsSolution's, the same methods
// PrimeArrangementsTests proves correct. UpperBound is the whole input, so there is
// nothing to hoist into a [GlobalSetup] - the [Params] sizes are the workload.
[MemoryDiagnoser]
public class PrimeArrangementsBenchmarks
{
    [Params(2_000, 20_000)]
    public int UpperBound { get; set; }

    [Benchmark(Baseline = true)]
    public int TrialDivision() =>
        PrimeArrangementsSolution.CountPrimeArrangementsByTrialDivision(UpperBound);

    [Benchmark]
    public int SieveOfEratosthenes() =>
        PrimeArrangementsSolution.CountPrimeArrangementsBySieveOfEratosthenes(UpperBound);
}
