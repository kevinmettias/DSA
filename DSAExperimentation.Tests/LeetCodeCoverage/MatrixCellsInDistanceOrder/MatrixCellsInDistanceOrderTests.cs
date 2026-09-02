using DSAExperimentation.Algorithms.Reducing;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Grids;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MatrixCellsInDistanceOrder;

// LeetCode 1030. Matrix Cells in Distance Order: on a fully-open grid, BFS distance
// from the center IS Manhattan distance, so this reuses GridShortestPath.cs's own
// primitive combo (Reduce.Graph over GridTopology/GridChildren with
// DistanceMapReduceAlgebra - the same BFS-distance-map engine GridTests.cs already
// exercises) to compute every cell's distance in one O(rows*cols) pass, then this
// repo's own MergeSort.Sort<Element,TSequence> over an ArrayIndexedSequence - the
// same custom-comparer shape QueueReconstructionByHeightTests.cs already exercises -
// to order the cells by that distance. LeetCode accepts any tie ordering among
// equidistant cells, so the assertions check the distance-sorted invariant directly
// rather than one exact permutation, except on the tie-free single-row case.
public sealed class MatrixCellsInDistanceOrderTests
{
    [Fact]
    public void AllCellsDistOrder_TieFreeRow_ReturnsExactDistanceOrder()
    {
        var result = AllCellsDistOrder(rows: 1, cols: 2, rCenter: 0, cCenter: 0);

        Assert.Equal([[0, 0], [0, 1]], result);
    }

    [Fact]
    public void AllCellsDistOrder_LeetCodeExample_ContainsEveryCellInNonDecreasingDistance()
    {
        var result = AllCellsDistOrder(rows: 2, cols: 2, rCenter: 0, cCenter: 1);

        Assert.Equal(4, result.Length);
        int[][] allCells = [[0, 0], [0, 1], [1, 0], [1, 1]];
        Assert.Equal(allCells, result.OrderBy(c => c[0]).ThenBy(c => c[1]));

        var distances = result.Select(c => Math.Abs(c[0] - 0) + Math.Abs(c[1] - 1)).ToArray();
        Assert.Equal(distances.OrderBy(d => d), distances);
    }

    private static int[][] AllCellsDistOrder(int rows, int cols, int rCenter, int cCenter)
    {
        var passable = BuildPassableGrid(rows, cols);
        var grid = new Grid(passable);
        var start = new GridNode(rCenter, cCenter, grid);

        var distances = Reduce.Graph<
            GridNode, GridTopology, GridChildren,
            NaturalChildOrder<GridNode, GridChildren>, GridChildren,
            BreadthFirstReduceOrder<GridNode>,
            DistanceMapReduceAlgebra<GridNode>, Dictionary<GridNode, int>>(start);

        return ToSortedCells(distances);
    }

    private static bool[,] BuildPassableGrid(int rows, int cols)
    {
        var passable = new bool[rows, cols];

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                passable[r, c] = true;
            }
        }

        return passable;
    }

    private static int[][] ToSortedCells(Dictionary<GridNode, int> distances)
    {
        var cells = distances.Select(kv => (kv.Key.Row, kv.Key.Col, Distance: kv.Value)).ToArray();

        MergeSort.Sort<(int Row, int Col, int Distance), ArrayIndexedSequence<(int Row, int Col, int Distance)>>(
            new ArrayIndexedSequence<(int Row, int Col, int Distance)>(cells),
            Comparer<(int Row, int Col, int Distance)>.Create((a, b) => a.Distance.CompareTo(b.Distance)));

        return cells.Select(c => new[] { c.Row, c.Col }).ToArray();
    }
}
