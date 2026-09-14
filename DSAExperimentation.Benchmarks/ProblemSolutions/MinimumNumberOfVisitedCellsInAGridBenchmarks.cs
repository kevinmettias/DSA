using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MinimumNumberOfVisitedCellsInAGrid;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumNumberOfVisitedCellsInAGridSolution's, the same
// methods MinimumNumberOfVisitedCellsInAGridTests proves correct. BruteForceScan
// scans the entire rest of a popped cell's row and its entire column (O(rows + cols)
// per pop), while ReduceGraph composes this repo's own Reduce.Graph over
// JumpGridTopology, whose JumpGridChildren computes only the O(v) cells actually
// reachable in one jump directly from the cell's own stored value - so the gap
// between the two arms widens as the jump values shrink relative to the grid's side
// length.
//
// The ReduceGraph arm is handed a prepared JumpGrid so the wrapper is charged to
// [GlobalSetup] rather than to the search (#17.4); the scan arm takes LeetCode's own
// jagged array because that is already its input.
[MemoryDiagnoser]
public class MinimumNumberOfVisitedCellsInAGridBenchmarks
{
    // Small relative to Side, so ReduceGraph's O(v)-per-pop children are a real
    // fraction of BruteForceScan's O(rows + cols)-per-pop full row/column scan.
    private const int MaxJumpDistance = 4;

    // LC problem number, reused as the deterministic jump-value seed.
    private const int GridSeed = 2617;

    [Params(20, 80)]
    public int Side;

    private int[][] _grid = null!;
    private JumpGrid _jumpGrid = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(GridSeed);
        _grid = new int[Side][];

        for (var row = 0; row < Side; row++)
        {
            _grid[row] = new int[Side];

            for (var col = 0; col < Side; col++)
            {
                _grid[row][col] = random.Next(MaxJumpDistance + 1);
            }
        }

        _grid[Side - 1][Side - 1] = 0;
        _jumpGrid = new JumpGrid(_grid);
    }

    [Benchmark(Baseline = true)]
    public int BruteForceScan() =>
        MinimumNumberOfVisitedCellsInAGridSolution.MinVisitedCellsByBruteForceScan(_grid);

    [Benchmark]
    public int ReduceGraph() =>
        MinimumNumberOfVisitedCellsInAGridSolution.MinVisitedCellsByReduceGraph(_jumpGrid);
}
