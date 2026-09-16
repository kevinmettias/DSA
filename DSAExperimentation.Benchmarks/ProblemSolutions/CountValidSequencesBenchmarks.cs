using BenchmarkDotNet.Attributes;
using DSAExperimentation.Domain.Modular;
using DSAExperimentation.LeetCode.CountValidSequences;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CountValidSequencesSolution's, the same methods
// CountValidSequencesTests proves correct. PrecomputedFactorials is handed its
// prepared FactorialTable - built once per TargetSum in [GlobalSetup] - so factorial
// construction is charged to setup rather than to the query being measured,
// while DirectBinomial recomputes its own numerator/denominator from scratch
// every call, exactly what it would cost with no [GlobalSetup] at all.
[MemoryDiagnoser]
public class CountValidSequencesBenchmarks
{
    private int _length;

    private FactorialTable _table = null!;
    [Params(1_000, 500_000)]
    public int TargetSum { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _length = TargetSum / 2;
        _table = FactorialTable.Build(TargetSum);
    }

    [Benchmark(Baseline = true)]
    public int DirectBinomial() => CountValidSequencesSolution.CountByDirectBinomial(TargetSum, _length);

    [Benchmark]
    public int PrecomputedFactorials() => CountValidSequencesSolution.CountByPrecomputedFactorials(_table, TargetSum, _length);
}
