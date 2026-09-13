using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Graph.Grids;
using DSAExperimentation.LeetCode.MatrixCellsInDistanceOrder;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MatrixCellsInDistanceOrderSolution's, the same methods
// MatrixCellsInDistanceOrderTests proves correct - the textbook closed-form Manhattan
// distance plus BCL Array.Sort against this repo's own Grid BFS distance map plus
// MergeSort. The grid is built in [GlobalSetup] and handed to the BFS arm's hoisted
// overload, so grid construction is charged to setup rather than to the search being
// measured. Both are O(rows*cols*log(rows*cols)); this demonstrates the repo's own
// grid-traversal and sorting primitives compose correctly, not an asymptotic win.
[MemoryDiagnoser]
public class MatrixCellsInDistanceOrderBenchmarks
{
    private const int MidpointDivisor = 2;

    [Params(20, 80)]
    public int Size;

    private Grid _grid = null!;
    private int _center;

    [GlobalSetup]
    public void Setup()
    {
        _grid = MatrixCellsInDistanceOrderSolution.BuildOpenGrid(Size, Size);
        _center = Size / MidpointDivisor;
    }

    [Benchmark(Baseline = true)]
    public int[][] ManhattanFormulaThenArraySort() =>
        MatrixCellsInDistanceOrderSolution.AllCellsDistOrderByManhattanFormula(Size, Size, _center, _center);

    [Benchmark]
    public int[][] GridBfsThenMergeSort() =>
        MatrixCellsInDistanceOrderSolution.AllCellsDistOrderByGridBfs(_grid, _center, _center);
}
