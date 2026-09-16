using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.ConstructQuadTree;

namespace DSAExperimentation.LeetCode.LogicalOrOfTwoBinaryGridsRepresentedAsQuadTrees;

// LeetCode 558. Logical Or of Two Binary Grids Represented as Quad-Trees: three
// genuinely different ways to OR two quad-trees representing same-size NxN binary
// grids. OrByDirectRecursiveMerge is the canonical answer - LeetCode's own two-Node
// signature - and walks both trees together, collapsing a subtree the moment either
// side is a uniform leaf (or both sides finish uniform and agree), never
// materializing a grid at all. OrByBruteForceGridMaterialize and
// OrByFenwickGridMaterialize instead flatten both input trees back to NxN grids, OR
// them cell-by-cell, then hand the merged grid to ConstructQuadTreeSolution (LC 427)
// to rebuild - the brute-force cell scan and the row-Fenwick-tree region check
// respectively. Materializing needs the grid's size explicitly, since a QuadTreeNode
// leaf alone does not carry it (a single leaf can represent any size grid) - an
// extra required input DirectRecursiveMerge alone can do without. See
// LogicalOrOfTwoBinaryGridsRepresentedAsQuadTreesBenchmarks.cs for how the three
// compare: the two materializing arms both pay a fixed O(size^2) regardless of how
// coarse either input tree actually is, while DirectRecursiveMerge is output-
// sensitive - its cost tracks the combined node count of the two INPUT trees, not
// the grid area.
internal static class LogicalOrOfTwoBinaryGridsRepresentedAsQuadTreesSolution
{
    public static QuadTreeNode OrByDirectRecursiveMerge(QuadTreeNode tree1, QuadTreeNode tree2)
    {
        if (tree1.IsLeaf)
        {
            return tree1.Val ? tree1 : tree2;
        }

        if (tree2.IsLeaf)
        {
            return tree2.Val ? tree2 : tree1;
        }

        var topLeft = OrByDirectRecursiveMerge(tree1.TopLeft!, tree2.TopLeft!);
        var topRight = OrByDirectRecursiveMerge(tree1.TopRight!, tree2.TopRight!);
        var bottomLeft = OrByDirectRecursiveMerge(tree1.BottomLeft!, tree2.BottomLeft!);
        var bottomRight = OrByDirectRecursiveMerge(tree1.BottomRight!, tree2.BottomRight!);

        if (AreAllUniformLeaves(topLeft, topRight, bottomLeft, bottomRight))
        {
            return new QuadTreeNode(Val: topLeft.Val, IsLeaf: true);
        }

        return new QuadTreeNode(Val: true, IsLeaf: false)
        {
            TopLeft = topLeft,
            TopRight = topRight,
            BottomLeft = bottomLeft,
            BottomRight = bottomRight,
        };
    }

    // The merged parent is uniform exactly when all four of its quadrants are leaves
    // and all four carry the same value - the one case that collapses back to a leaf.
    private static bool AreAllUniformLeaves(
        QuadTreeNode topLeft, QuadTreeNode topRight, QuadTreeNode bottomLeft, QuadTreeNode bottomRight) =>
        topLeft.IsLeaf && topRight.IsLeaf && bottomLeft.IsLeaf && bottomRight.IsLeaf
        && topLeft.Val == topRight.Val && topRight.Val == bottomLeft.Val && bottomLeft.Val == bottomRight.Val;

    // Baseline: materializes both trees to grids and rebuilds via ConstructQuadTree
    // Solution's own raw-cell-scan baseline - BCL arrays only internally, the same
    // "what you would write without this repo" character, reused rather than
    // re-inlined here.
    public static QuadTreeNode OrByBruteForceGridMaterialize(QuadTreeNode tree1, QuadTreeNode tree2, int size)
    {
        var merged = MergeGrids(tree1, tree2, size);
        return ConstructQuadTreeSolution.BuildByBruteForceCellScan(merged);
    }

    public static QuadTreeNode OrByFenwickGridMaterialize(QuadTreeNode tree1, QuadTreeNode tree2, int size)
    {
        var merged = MergeGrids(tree1, tree2, size);
        return ConstructQuadTreeSolution.BuildByRowFenwickTree(merged);
    }

    private static int[][] MergeGrids(QuadTreeNode tree1, QuadTreeNode tree2, int size)
    {
        var grid1 = QuadTreeGrid.Materialize(tree1, size);
        var grid2 = QuadTreeGrid.Materialize(tree2, size);

        var merged = QuadTreeGrid.Allocate(size);
        for (var row = 0; row < size; row++)
        {
            for (var col = 0; col < size; col++)
            {
                merged[row][col] = grid1[row][col] | grid2[row][col];
            }
        }

        return merged;
    }
}
