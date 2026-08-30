using DSAExperimentation.DataStructures.FenwickTree;
using DSAExperimentation.Tests.LeetCodeCoverage.LogicalOrOfTwoBinaryGridsRepresentedAsQuadTrees.Fixtures;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LogicalOrOfTwoBinaryGridsRepresentedAsQuadTrees;

// LeetCode 558. Logical OR of Two Binary Grids Represented as Quad-Trees: the same
// "one of this repo's own FenwickTree<int,SumOperation<int>> per row" 2D-region-sum
// move ConstructQuadTreeTests (LC 427) already makes - materialize both input trees
// back to their NxN grids, OR the grids cell-by-cell, then reconstruct the result
// tree from the merged grid via the same Fenwick-based uniformity check (a region is
// uniform exactly when its cell sum is 0 or the full area). This file exists to
// prove the composition is correct, not that it is the fastest way to solve LC 558 -
// see LogicalOrOfTwoBinaryGridsRepresentedAsQuadTreesBenchmarks.cs for how it
// compares against the canonical direct tree-to-tree recursive merge, which never
// materializes a grid at all.
public sealed partial class LogicalOrOfTwoBinaryGridsRepresentedAsQuadTreesTests
{
    [Fact]
    public void Or_AllTrueLeafWithAllFalseLeaf_CollapsesToSingleTrueLeaf()
    {
        var tree1 = new QuadTreeNode(val: true, isLeaf: true);
        var tree2 = new QuadTreeNode(val: false, isLeaf: true);

        var result = Or(tree1, tree2, size: 2);

        Assert.True(result.IsLeaf);
        Assert.True(result.Val);
    }

    [Fact]
    public void Or_CheckerboardWithInverseCheckerboard_CollapsesToSingleTrueLeaf()
    {
        int[][] checkerboard = [[1, 0], [0, 1]];
        int[][] inverseCheckerboard = [[0, 1], [1, 0]];

        var tree1 = Build(checkerboard);
        var tree2 = Build(inverseCheckerboard);

        var result = Or(tree1, tree2, size: 2);

        Assert.True(result.IsLeaf);
        Assert.True(result.Val);
    }

    [Fact]
    public void Or_TwoMixedGrids_ReconstructsElementwiseOrOfBothGrids()
    {
        int[][] grid1 =
        [
            [1, 1, 0, 0],
            [1, 1, 0, 0],
            [0, 0, 0, 0],
            [0, 0, 0, 0],
        ];
        int[][] grid2 =
        [
            [0, 0, 0, 0],
            [0, 0, 0, 0],
            [0, 0, 1, 0],
            [0, 0, 0, 1],
        ];

        var tree1 = Build(grid1);
        var tree2 = Build(grid2);

        var result = Or(tree1, tree2, size: 4);

        var rebuilt = grid1.Select(row => new int[row.Length]).ToArray();
        Fill(result, rebuilt, 0, 0, 4);

        int[][] expected =
        [
            [1, 1, 0, 0],
            [1, 1, 0, 0],
            [0, 0, 1, 0],
            [0, 0, 0, 1],
        ];
        Assert.Equal(expected, rebuilt);
    }

    private static QuadTreeNode Or(QuadTreeNode tree1, QuadTreeNode tree2, int size)
    {
        var grid1 = NewGrid(size);
        var grid2 = NewGrid(size);
        Fill(tree1, grid1, 0, 0, size);
        Fill(tree2, grid2, 0, 0, size);

        var merged = NewGrid(size);
        for (var row = 0; row < size; row++)
        {
            for (var col = 0; col < size; col++)
            {
                merged[row][col] = grid1[row][col] | grid2[row][col];
            }
        }

        return Build(merged);
    }

    private static int[][] NewGrid(int size)
    {
        var grid = new int[size][];
        for (var row = 0; row < size; row++)
        {
            grid[row] = new int[size];
        }

        return grid;
    }

    private static void Fill(QuadTreeNode node, int[][] grid, int row, int col, int size)
    {
        if (node.IsLeaf)
        {
            var value = node.Val ? 1 : 0;
            for (var r = row; r < row + size; r++)
            {
                for (var c = col; c < col + size; c++)
                {
                    grid[r][c] = value;
                }
            }

            return;
        }

        var half = size / 2;
        Fill(node.TopLeft!, grid, row, col, half);
        Fill(node.TopRight!, grid, row, col + half, half);
        Fill(node.BottomLeft!, grid, row + half, col, half);
        Fill(node.BottomRight!, grid, row + half, col + half, half);
    }

    private static QuadTreeNode Build(int[][] grid)
    {
        var rows = grid.Select(row => new FenwickTree<int, SumOperation<int>>(row)).ToArray();
        return BuildRegion(rows, 0, 0, grid.Length);
    }

    private static QuadTreeNode BuildRegion(FenwickTree<int, SumOperation<int>>[] rows, int row, int col, int size)
    {
        var sum = 0;
        for (var r = row; r < row + size; r++)
        {
            sum += rows[r].Query(col, col + size - 1);
        }

        var area = size * size;
        if (sum == 0 || sum == area)
        {
            return new QuadTreeNode(val: sum == area, isLeaf: true);
        }

        var half = size / 2;
        return new QuadTreeNode(val: true, isLeaf: false)
        {
            TopLeft = BuildRegion(rows, row, col, half),
            TopRight = BuildRegion(rows, row, col + half, half),
            BottomLeft = BuildRegion(rows, row + half, col, half),
            BottomRight = BuildRegion(rows, row + half, col + half, half),
        };
    }
}
