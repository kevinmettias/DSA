using DSAExperimentation.Algorithms.Reducing;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Grids;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.MatrixCellsInDistanceOrder;

// LeetCode 1030. Matrix Cells in Distance Order: list every cell of a rows x cols
// matrix sorted by Manhattan distance from (rCenter, cCenter). LeetCode accepts any
// ordering among equidistant cells, so the two strategies here are allowed to
// disagree on ties and only have to agree on the distance sequence.
internal static class MatrixCellsInDistanceOrderSolution
{
    // The textbook answer: Manhattan distance is a closed form, so compute it per
    // cell and hand the array to BCL Array.Sort. Deliberately written without this
    // repo's primitives - it is the arm the composed solution below has to justify
    // itself against.
    public static int[][] AllCellsDistOrderByManhattanFormula(int rows, int cols, int rCenter, int cCenter)
    {
        var cells = new (int Row, int Col, int Distance)[rows * cols];
        var next = 0;

        for (var row = 0; row < rows; row++)
        {
            for (var col = 0; col < cols; col++)
            {
                cells[next++] = (row, col, Math.Abs(row - rCenter) + Math.Abs(col - cCenter));
            }
        }

        Array.Sort(cells, ByDistance);

        return ToRowColPairs(cells);
    }

    // This repo's own primitives: on a fully-open grid, BFS distance from the center
    // IS Manhattan distance, so Reduce.Graph over GridTopology/GridChildren with
    // DistanceMapReduceAlgebra - GridShortestPath.cs's own combination - produces
    // every cell's distance in one O(rows*cols) pass, and MergeSort.Sort over an
    // ArrayIndexedSequence orders them by it.
    public static int[][] AllCellsDistOrderByGridBfs(int rows, int cols, int rCenter, int cCenter)
    {
        var grid = BuildOpenGrid(rows, cols);

        return AllCellsDistOrderByGridBfs(grid, rCenter, cCenter);
    }

    public static int[][] AllCellsDistOrderByGridBfs(Grid grid, int rCenter, int cCenter)
    {
        var start = new GridNode(rCenter, cCenter, grid);

        var distances = Reduce.Graph<
            GridNode, GridTopology, GridChildren,
            NaturalChildOrder<GridNode, GridChildren>, GridChildren,
            BreadthFirstReduceOrder<GridNode>,
            DistanceMapReduceAlgebra<GridNode>, Dictionary<GridNode, int>>(start);

        var cells = distances.Select(entry => (entry.Key.Row, entry.Key.Col, Distance: entry.Value)).ToArray();

        MergeSort.Sort<(int Row, int Col, int Distance), ArrayIndexedSequence<(int Row, int Col, int Distance)>>(
            new ArrayIndexedSequence<(int Row, int Col, int Distance)>(cells),
            Comparer<(int Row, int Col, int Distance)>.Create(ByDistance));

        return ToRowColPairs(cells);
    }

    // Every cell of a LC 1030 matrix is reachable, so the grid the BFS walks is
    // uniformly passable - the geometry alone supplies the adjacency.
    public static Grid BuildOpenGrid(int rows, int cols)
    {
        var passable = new bool[rows, cols];

        for (var row = 0; row < rows; row++)
        {
            for (var col = 0; col < cols; col++)
            {
                passable[row, col] = true;
            }
        }

        return new Grid(passable);
    }

    private static int ByDistance((int Row, int Col, int Distance) left, (int Row, int Col, int Distance) right) =>
        left.Distance.CompareTo(right.Distance);

    private static int[][] ToRowColPairs((int Row, int Col, int Distance)[] cells) =>
        cells.Select(cell => new[] { cell.Row, cell.Col }).ToArray();
}
