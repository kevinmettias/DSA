using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.NumberOfWaysToPaintN3Grid;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are NumberOfWaysToPaintN3GridSolution's, the same
// methods NumberOfWaysToPaintN3GridTests proves correct - bottom-up tabulation
// over the (same, different) row-pattern-count pair vs. the Memoizer-driven
// top-down recursion over the same pair, both O(N).
[MemoryDiagnoser]
public class NumberOfWaysToPaintN3GridBenchmarks
{
    [Params(1_000, 5_000)]
    public int RowCount { get; set; }

    [Benchmark(Baseline = true)]
    public long Tabulation() => NumberOfWaysToPaintN3GridSolution.CountWaysByTabulation(RowCount);

    [Benchmark]
    public long Memoized() => NumberOfWaysToPaintN3GridSolution.CountWaysByMemoizedRecurrence(RowCount);
}
