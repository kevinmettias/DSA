using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.PrisonCellsAfterNDays;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are PrisonCellsAfterNDaysSolution's, the same methods
// PrisonCellsAfterNDaysTests proves correct. The naive day-by-day simulation
// (baseline) walks all N days directly and so scales linearly, even though the
// 8-cell state space has only 256 possible encodings and must cycle almost
// immediately; the composed arm maps each encoded state to the day it was first
// seen in this repo's own HashMap<int,int> and jumps straight to N modulo the
// cycle length - near-constant time regardless of how large N gets.
[MemoryDiagnoser]
public class PrisonCellsAfterNDaysBenchmarks
{
    private int[] _cells = [];

    [Params(10_000, 1_000_000)]
    public int Days { get; set; }

    [GlobalSetup]
    public void Setup() => _cells = [1, 0, 0, 1, 0, 0, 1, 0];

    [Benchmark(Baseline = true)]
    public int[] DailySimulation() =>
        PrisonCellsAfterNDaysSolution.CellsAfterNDaysByDailySimulation(_cells, Days);

    [Benchmark]
    public int[] CycleDetectionViaHashMap() =>
        PrisonCellsAfterNDaysSolution.CellsAfterNDaysByCycleDetection(_cells, Days);
}
