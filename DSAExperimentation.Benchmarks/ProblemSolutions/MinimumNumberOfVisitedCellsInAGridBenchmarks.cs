using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Reducing;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Minimum Number of Visited Cells in a Grid (LC 2617): BruteForceScan scans the
// entire rest of a popped cell's row and its entire column (O(rows + cols) per pop),
// while ReduceGraph composes this repo's own Reduce.Graph over JumpGridTopology,
// whose JumpGridChildren computes only the O(v) cells actually reachable in one jump
// directly from the cell's own stored value - so the gap between the two arms widens
// as the jump values shrink relative to the grid's side length.
[MemoryDiagnoser]
public class MinimumNumberOfVisitedCellsInAGridBenchmarks
{
    // Small relative to Side, so ReduceGraph's O(v)-per-pop children are a real
    // fraction of BruteForceScan's O(rows + cols)-per-pop full row/column scan.
    private const int MaxJumpDistance = 4;

    [Params(20, 80)]
    public int Side;

    private int[][] _grid = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(2617);
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
    }

    [Benchmark(Baseline = true)]
    public int BruteForceScan()
    {
        var distances = new int[Side, Side];

        for (var row = 0; row < Side; row++)
        {
            for (var col = 0; col < Side; col++)
            {
                distances[row, col] = -1;
            }
        }

        distances[0, 0] = 0;

        var queue = new Queue<(int Row, int Col)>();
        queue.Enqueue((0, 0));

        while (queue.Count > 0)
        {
            var (row, col) = queue.Dequeue();
            var jump = _grid[row][col];

            for (var nextCol = 0; nextCol < Side; nextCol++)
            {
                if (nextCol > col && nextCol <= col + jump && distances[row, nextCol] == -1)
                {
                    distances[row, nextCol] = distances[row, col] + 1;
                    queue.Enqueue((row, nextCol));
                }
            }

            for (var nextRow = 0; nextRow < Side; nextRow++)
            {
                if (nextRow > row && nextRow <= row + jump && distances[nextRow, col] == -1)
                {
                    distances[nextRow, col] = distances[row, col] + 1;
                    queue.Enqueue((nextRow, col));
                }
            }
        }

        var target = distances[Side - 1, Side - 1];
        return target == -1 ? -1 : target + 1;
    }

    [Benchmark]
    public int ReduceGraph()
    {
        var jumpGrid = new JumpGrid(_grid);
        var source = new JumpGridNode(0, 0, jumpGrid);
        var target = new JumpGridNode(Side - 1, Side - 1, jumpGrid);

        var distanceByNode = Reduce.Graph<
            JumpGridNode, JumpGridTopology, JumpGridChildren,
            NaturalChildOrder<JumpGridNode, JumpGridChildren>, JumpGridChildren,
            BreadthFirstReduceOrder<JumpGridNode>,
            DistanceMapReduceAlgebra<JumpGridNode>, Dictionary<JumpGridNode, int>>(source);

        return distanceByNode.TryGetValue(target, out var distance) ? distance + 1 : -1;
    }
}
