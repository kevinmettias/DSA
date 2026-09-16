using BenchmarkDotNet.Attributes;
using DSAExperimentation.Domain.Modular;
using DSAExperimentation.LeetCode.CountValidSequences;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CountValidSequencesSolution's, the same methods
// CountValidSequencesTests proves correct. PrecomputedFactorials is handed its
// prepared FactorialTable - built once per N in [GlobalSetup] - so factorial
// construction is charged to setup rather than to the query being measured,
// while DirectBinomial recomputes its own numerator/denominator from scratch
// every call, exactly what it would cost with no [GlobalSetup] at all.
[MemoryDiagnoser]
public class CountValidSequencesBenchmarks
{
    private int _k;

    private FactorialTable _table = null!;
    [Params(1_000, 500_000)]
    public int N { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _k = N / 2;
        _table = FactorialTable.Build(N);
    }

    [Benchmark(Baseline = true)]
    public int DirectBinomial() => CountValidSequencesSolution.CountByDirectBinomial(N, _k);

    [Benchmark]
    public int PrecomputedFactorials() => CountValidSequencesSolution.CountByPrecomputedFactorials(_table, N, _k);
}
