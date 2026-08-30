using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.FenwickTree;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Logical OR of Two Binary Grids Represented as Quad-Trees (LC 558): three genuinely
// different ways to compute the OR of two quad-trees. (1) materialize both input
// trees to full grids, OR them cell-by-cell, then rebuild the result via a
// brute-force uniform-region scan; (2) the same materialize-then-rebuild shape but
// rebuilding with this repo's own FenwickTree<int,SumOperation<int>> per row (the
// ConstructQuadTree/LC427 move, reused here on the merged grid); and (3) the
// canonical direct tree-to-tree recursive merge that never materializes a grid at
// all and collapses uniform quadrants as soon as either side is a leaf. (1) and (2)
// both pay a fixed O(size^2) regardless of how coarse either input tree actually is;
// (3) is output-sensitive - its cost tracks the combined node count of the two INPUT
// trees, not the grid area - so it is expected to win by a growing margin as Size
// grows while the two trees stay coarse. This mirrors ConstructQuadTreeBenchmarks'
// own finding that FenwickTree does not automatically beat brute force here either:
// (1) and (2) are included together specifically to show that swapping in the
// repo-primitive-based region check alone does not fix (1)'s real problem, which is
// materializing the grid at all - only (3), which skips materialization entirely,
// does.
[MemoryDiagnoser]
public class LogicalOrOfTwoBinaryGridsRepresentedAsQuadTreesBenchmarks
{
    [Params(16, 128)]
    public int Size;

    private QuadTreeNode _tree1 = null!;
    private QuadTreeNode _tree2 = null!;

    [GlobalSetup]
    public void Setup()
    {
        var grid1 = NewGrid(Size);
        var grid2 = NewGrid(Size);

        for (var row = 0; row < Size; row++)
        {
            for (var col = 0; col < Size; col++)
            {
                // grid1 splits top/bottom, grid2 splits left/right - every top-level
                // quadrant of the OR result is genuinely mixed, so neither input tree
                // nor the merged grid collapses trivially at the root.
                grid1[row][col] = row < Size / 2 ? 0 : 1;
                grid2[row][col] = col < Size / 2 ? 0 : 1;
            }
        }

        _tree1 = Build(grid1);
        _tree2 = Build(grid2);
    }

    [Benchmark(Baseline = true)]
    public int BruteForceGridMaterialize()
    {
        var merged = MergeGrids();
        return CountLeaves(BuildBruteForce(merged, 0, 0, Size));
    }

    [Benchmark]
    public int FenwickGridMaterialize()
    {
        var merged = MergeGrids();
        var rows = merged.Select(row => new FenwickTree<int, SumOperation<int>>(row)).ToArray();
        return CountLeaves(BuildFenwick(rows, 0, 0, Size));
    }

    [Benchmark]
    public int DirectRecursiveMerge() => CountLeaves(Or(_tree1, _tree2));

    private int[][] MergeGrids()
    {
        var grid1 = NewGrid(Size);
        var grid2 = NewGrid(Size);
        Fill(_tree1, grid1, 0, 0, Size);
        Fill(_tree2, grid2, 0, 0, Size);

        var merged = NewGrid(Size);
        for (var row = 0; row < Size; row++)
        {
            for (var col = 0; col < Size; col++)
            {
                merged[row][col] = grid1[row][col] | grid2[row][col];
            }
        }

        return merged;
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
        return BuildFenwick(rows, 0, 0, grid.Length);
    }

    private static QuadTreeNode BuildFenwick(FenwickTree<int, SumOperation<int>>[] rows, int row, int col, int size)
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
            TopLeft = BuildFenwick(rows, row, col, half),
            TopRight = BuildFenwick(rows, row, col + half, half),
            BottomLeft = BuildFenwick(rows, row + half, col, half),
            BottomRight = BuildFenwick(rows, row + half, col + half, half),
        };
    }

    private static QuadTreeNode BuildBruteForce(int[][] grid, int row, int col, int size)
    {
        var first = grid[row][col];
        var uniform = true;

        for (var r = row; r < row + size && uniform; r++)
        {
            for (var c = col; c < col + size; c++)
            {
                if (grid[r][c] != first)
                {
                    uniform = false;
                    break;
                }
            }
        }

        if (uniform)
        {
            return new QuadTreeNode(val: first == 1, isLeaf: true);
        }

        var half = size / 2;
        return new QuadTreeNode(val: true, isLeaf: false)
        {
            TopLeft = BuildBruteForce(grid, row, col, half),
            TopRight = BuildBruteForce(grid, row, col + half, half),
            BottomLeft = BuildBruteForce(grid, row + half, col, half),
            BottomRight = BuildBruteForce(grid, row + half, col + half, half),
        };
    }

    private static QuadTreeNode Or(QuadTreeNode tree1, QuadTreeNode tree2)
    {
        if (tree1.IsLeaf)
        {
            return tree1.Val ? tree1 : tree2;
        }

        if (tree2.IsLeaf)
        {
            return tree2.Val ? tree2 : tree1;
        }

        var topLeft = Or(tree1.TopLeft!, tree2.TopLeft!);
        var topRight = Or(tree1.TopRight!, tree2.TopRight!);
        var bottomLeft = Or(tree1.BottomLeft!, tree2.BottomLeft!);
        var bottomRight = Or(tree1.BottomRight!, tree2.BottomRight!);

        if (topLeft.IsLeaf && topRight.IsLeaf && bottomLeft.IsLeaf && bottomRight.IsLeaf
            && topLeft.Val == topRight.Val && topRight.Val == bottomLeft.Val && bottomLeft.Val == bottomRight.Val)
        {
            return new QuadTreeNode(val: topLeft.Val, isLeaf: true);
        }

        return new QuadTreeNode(val: true, isLeaf: false)
        {
            TopLeft = topLeft,
            TopRight = topRight,
            BottomLeft = bottomLeft,
            BottomRight = bottomRight,
        };
    }

    private static int CountLeaves(QuadTreeNode node)
        => node.IsLeaf
            ? 1
            : CountLeaves(node.TopLeft!) + CountLeaves(node.TopRight!)
                + CountLeaves(node.BottomLeft!) + CountLeaves(node.BottomRight!);
}

// Mirrors LeetCode's own Node - see
// DSAExperimentation.Tests.LeetCodeCoverage.LogicalOrOfTwoBinaryGridsRepresentedAsQuadTrees.Fixtures.QuadTreeNode,
// duplicated here rather than shared since the Benchmarks project can't reference
// the Tests project's fixture types.
internal sealed class QuadTreeNode(bool val, bool isLeaf)
{
    public bool Val { get; } = val;

    public bool IsLeaf { get; } = isLeaf;

    public QuadTreeNode? TopLeft { get; init; }

    public QuadTreeNode? TopRight { get; init; }

    public QuadTreeNode? BottomLeft { get; init; }

    public QuadTreeNode? BottomRight { get; init; }
}
