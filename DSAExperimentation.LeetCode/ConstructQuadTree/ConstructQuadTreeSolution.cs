using DSAExperimentation.DataStructures;
using DSAExperimentation.DataStructures.FenwickTree;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.ConstructQuadTree;

// LeetCode 427. Construct Quad Tree: recursively split an n x n binary grid into
// four equal quadrants, stopping and emitting a leaf the moment a region is
// uniform (all 0s or all 1s). QuadTreeNode is DataStructures.Graph.Engines.Dags.
// Trees' fixed-4-ary tree node - the same shape LC 558 (Logical Or of Two Binary
// Grids Represented as Quad Trees) needs, so it lives beside its arity siblings
// (BinaryTreeNode, RootedTreeNode) rather than pinned to this one problem.
//
// BuildByBruteForceCellScan confirms uniformity with a raw cell-by-cell scan -
// BCL arrays only, the textbook baseline. BuildByRowFenwickTree instead builds one
// FenwickTree<int,SumOperation<int>> per grid row - the same move
// RangeSumQuery2DImmutable makes for LC 304 - and confirms uniformity from an
// O(rows*log(cols)) region-sum check (0 => all zero, area => all one) instead of a
// raw O(area) scan. Both build a real QuadTreeNode tree, LeetCode's actual answer
// shape - the pre-migration benchmark had both arms count leaves instead, which is
// weaker than what the problem asks for. See ConstructQuadTreeBenchmarks.cs for why
// the Fenwick approach's cheaper per-call confirmation still loses in aggregate:
// its own forest-build cost is charged into the same measured call.
internal static class ConstructQuadTreeSolution
{

    public static QuadTreeNode BuildByBruteForceCellScan(int[][] grid) =>
        BuildRegionByCellScan(grid, 0, 0, grid.Length);

    // Builds the row-Fenwick forest itself, deliberately not split into a
    // hoisted-input overload: the forest's own O(cells*log(cols)) build cost is
    // part of what ConstructQuadTreeBenchmarks measures for this strategy.
    public static QuadTreeNode BuildByRowFenwickTree(int[][] grid)
    {
        var rows = grid.Select(row => new FenwickTree<int, SumOperation<int>>(row)).ToArray();

        return BuildRegionByRowFenwickTree(rows, 0, 0, grid.Length);
    }

    private static QuadTreeNode BuildRegionByCellScan(int[][] grid, int row, int col, int size)
    {
        if (IsUniformRegionByCellScan(grid, row, col, size))
        {
            return new QuadTreeNode(Val: grid[row][col] == 1, IsLeaf: true);
        }

        var half = size / AlgorithmConstants.HalvingFactor;
        return new QuadTreeNode(Val: true, IsLeaf: false)
        {
            TopLeft = BuildRegionByCellScan(grid, row, col, half),
            TopRight = BuildRegionByCellScan(grid, row, col + half, half),
            BottomLeft = BuildRegionByCellScan(grid, row + half, col, half),
            BottomRight = BuildRegionByCellScan(grid, row + half, col + half, half),
        };
    }

    private static bool IsUniformRegionByCellScan(int[][] grid, int row, int col, int size)
    {
        var first = grid[row][col];

        for (var r = row; r < row + size; r++)
        {
            for (var c = col; c < col + size; c++)
            {
                if (grid[r][c] != first)
                {
                    return false;
                }
            }
        }

        return true;
    }

    private static QuadTreeNode BuildRegionByRowFenwickTree(
        FenwickTree<int, SumOperation<int>>[] rows, int row, int col, int size)
    {
        var sum = 0;
        for (var r = row; r < row + size; r++)
        {
            sum += rows[r].Query(col, col + size - 1);
        }

        var area = size * size;
        if (sum == 0 || sum == area)
        {
            return new QuadTreeNode(Val: sum == area, IsLeaf: true);
        }

        var half = size / AlgorithmConstants.HalvingFactor;
        return new QuadTreeNode(Val: true, IsLeaf: false)
        {
            TopLeft = BuildRegionByRowFenwickTree(rows, row, col, half),
            TopRight = BuildRegionByRowFenwickTree(rows, row, col + half, half),
            BottomLeft = BuildRegionByRowFenwickTree(rows, row + half, col, half),
            BottomRight = BuildRegionByRowFenwickTree(rows, row + half, col + half, half),
        };
    }
}
