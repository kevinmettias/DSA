using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.MinimumTimeToVisitACellInAGrid;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumTimeToVisitACellInAGridSolution's, the same
// methods MinimumTimeToVisitACellInAGridTests proves correct (TwoSumBenchmarks
// precedent). WaitCostGridWorkloads builds the grid - (0,0) always 0 and every other
// cell demanding a random, sometimes-large wait - and grid[0][1] is then cleared,
// since row 0 and column 0 both count up from 0 and the problem guarantees that cell
// is reachable. Every relaxation therefore goes through ArrivalTime's wait-and-parity
// logic instead of taking the constant-weight-1 shortcut.
[MemoryDiagnoser]
public class MinimumTimeToVisitACellInAGridBenchmarks
{
    private const int MaxWaitExclusive = 50;
    private const int Seed = 2577;

    private int[][] _grid = [];

    [Params(20, 60)]
    public int Size { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _grid = WaitCostGridWorkloads.WithZeroOrigin(Size, MaxWaitExclusive, Seed);
        _grid[0][1] = 0;
    }

    [Benchmark(Baseline = true)]
    public int BclPriorityQueue() => MinimumTimeToVisitACellInAGridSolution.MinimumTimeByBclPriorityQueue(_grid);

    [Benchmark]
    public int Heap() => MinimumTimeToVisitACellInAGridSolution.MinimumTimeByHeap(_grid);
}
