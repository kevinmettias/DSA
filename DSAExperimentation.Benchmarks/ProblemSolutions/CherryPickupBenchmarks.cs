using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.CherryPickup;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CherryPickupSolution's, the same methods
// CherryPickupTests proves correct. The grid is all-cherries with no obstacles so
// nothing short-circuits the naive baseline's full branching early; Size is kept
// modest for exactly that reason.
[MemoryDiagnoser]
public class CherryPickupBenchmarks
{
    [Params(4, 6)]
    public int Size;

    private int[,] _grid = null!;

    [GlobalSetup]
    public void Setup()
    {
        _grid = new int[Size, Size];

        for (var row = 0; row < Size; row++)
        {
            for (var col = 0; col < Size; col++)
            {
                _grid[row, col] = 1;
            }
        }
    }

    [Benchmark(Baseline = true)]
    public int UnmemoizedRecursion() => CherryPickupSolution.MaxCherriesByUnmemoizedRecursion(_grid);

    [Benchmark]
    public int MemoizedRecursion() => CherryPickupSolution.MaxCherriesByMemoizedRecursion(_grid);
}
