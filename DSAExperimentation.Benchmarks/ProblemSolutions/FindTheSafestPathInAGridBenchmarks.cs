using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.FindTheSafestPathInAGrid;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindTheSafestPathInAGridSolution's, the same
// methods FindTheSafestPathInAGridTests proves correct. MaximumSafenessFactorByBclBfs
// pays for repeated BFS re-walks, one per binary-search step over the safeness
// threshold; MaximumSafenessFactorByHeap answers the same question in one pass.
[MemoryDiagnoser]
public class FindTheSafestPathInAGridBenchmarks
{
    private const int ThiefProbabilityDenominator = 20;
    private const int Seed = 1;

    [Params(20, 80)]
    public int Size;

    private int[][] _grid = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _grid = Enumerable.Range(0, Size)
            .Select(_ => Enumerable.Range(0, Size)
                .Select(_ => random.Next(0, ThiefProbabilityDenominator) == 0 ? 1 : 0)
                .ToArray())
            .ToArray();

        // Start/end don't need to be thief-free per the problem's own
        // constraints, but forcing it keeps both endpoints' safeness away from
        // the trivial 0 case so every iteration exercises the full algorithm.
        _grid[0][0] = 0;
        _grid[Size - 1][Size - 1] = 0;
    }

    [Benchmark(Baseline = true)]
    public int BclBfs() => FindTheSafestPathInAGridSolution.MaximumSafenessFactorByBclBfs(_grid);

    [Benchmark]
    public int Heap() => FindTheSafestPathInAGridSolution.MaximumSafenessFactorByHeap(_grid);
}
