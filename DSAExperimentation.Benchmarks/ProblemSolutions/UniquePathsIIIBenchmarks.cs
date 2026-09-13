using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.UniquePathsIII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are UniquePathsIIISolution's, the same methods
// UniquePathsIIITests proves correct. The search tree's size is far more sensitive
// to obstacle layout than to raw grid dimensions - a Hamiltonian-path count can blow
// up combinatorially even on a small board - so, the same reasoning
// NQueensBenchmarks/SudokuSolverBenchmarks already give for fixing their own
// board/size rather than [Params]-ing it, [GlobalSetup] fixes one small grid and the
// comparison isolates the constant-factor cost of Backtrack.Search's generic
// delegate dispatch from an equivalent purpose-built recursion.
[MemoryDiagnoser]
public class UniquePathsIIIBenchmarks
{
    private const int EndCellMarker = 2;

    private int[][] _grid = null!;

    [GlobalSetup]
    public void Setup() => _grid = [[1, 0, 0, 0], [0, 0, 0, 0], [0, 0, 0, EndCellMarker]];

    [Benchmark(Baseline = true)]
    public int SpecializedRecursive() => UniquePathsIIISolution.CountUniquePathsBySpecializedRecursion(_grid);

    [Benchmark]
    public int BacktrackEngine() => UniquePathsIIISolution.CountUniquePathsByBacktrackEngine(_grid);
}
