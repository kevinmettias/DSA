using DSAExperimentation.DataStructures.FenwickTree;
using DSAExperimentation.Tests.LeetCodeCoverage.ConstructQuadTree.Fixtures;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ConstructQuadTree;

// LeetCode 427. Construct Quad Tree: the same "one of this repo's own
// FenwickTree<int,SumOperation<int>> per row" 2D-region-sum move RangeSumQuery2DImmutableTests
// already makes for LC 304 - a region is uniform exactly when its cell sum is 0 (all zero) or
// equal to its area (all one), an O(rows*log(cols)) check per node rather than a raw O(area)
// cell-by-cell scan. See ConstructQuadTreeBenchmarks.cs for why that per-call win still loses
// in aggregate here: unlike LC 304's independently large QueryCount, a quad tree's own
// recursion never issues enough confirmation calls to amortize the Fenwick forest's own
// O(cells*log(cols)) build cost. This file exists to prove the composition is correct, not
// that it is the faster of the two.
public sealed partial class ConstructQuadTreeTests
{
    [Fact]
    public void Construct_AllOnesGrid_ProducesSingleLeaf()
    {
        int[][] grid = [[1, 1], [1, 1]];

        var root = Build(grid);

        Assert.True(root.IsLeaf);
        Assert.True(root.Val);
    }

    [Fact]
    public void Construct_CheckerboardGrid_ProducesFourLeafQuadrants()
    {
        int[][] grid =
        [
            [0, 1],
            [1, 0],
        ];

        var root = Build(grid);

        Assert.False(root.IsLeaf);
        Assert.True(root.TopLeft!.IsLeaf);
        Assert.False(root.TopLeft.Val);
        Assert.True(root.TopRight!.IsLeaf);
        Assert.True(root.TopRight.Val);
        Assert.True(root.BottomLeft!.IsLeaf);
        Assert.True(root.BottomLeft.Val);
        Assert.True(root.BottomRight!.IsLeaf);
        Assert.False(root.BottomRight.Val);
    }

    [Fact]
    public void Construct_MixedGrid_ReconstructsOriginalGridFromTree()
    {
        int[][] grid =
        [
            [1, 1, 1, 1, 0, 0, 0, 0],
            [1, 1, 1, 1, 0, 0, 0, 0],
            [1, 1, 1, 1, 0, 0, 0, 0],
            [1, 1, 1, 1, 0, 0, 0, 0],
            [1, 1, 1, 1, 1, 1, 1, 1],
            [1, 1, 1, 1, 1, 1, 1, 1],
            [1, 1, 1, 1, 1, 1, 1, 1],
            [1, 1, 1, 1, 1, 1, 1, 1],
        ];

        var root = Build(grid);
        var rebuilt = grid.Select(row => new int[row.Length]).ToArray();
        Fill(root, rebuilt, 0, 0, grid.Length);

        Assert.Equal(grid, rebuilt);
    }

    private static void Fill(QuadTreeNode node, int[][] grid, int row, int col, int size)
    {
        if (node.IsLeaf)
        {
            for (var r = row; r < row + size; r++)
            {
                for (var c = col; c < col + size; c++)
                {
                    grid[r][c] = node.Val ? 1 : 0;
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
