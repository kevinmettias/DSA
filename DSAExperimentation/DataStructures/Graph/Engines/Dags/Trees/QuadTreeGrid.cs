using DSAExperimentation.DataStructures;

namespace DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

// The NxN binary grid a QuadTreeNode denotes, materialized back out of the tree: a leaf paints its
// own Val across the region it covers, a non-leaf halves the region and recurses into its four
// quadrants. Hardwired to the concrete QuadTreeNode, so per ARCHITECTURE.md §5 step 7 / §13.5 it
// co-locates with that Representation under DataStructures/ - the same reasoning InOrderTraversal
// gives for living beside BinaryTreeNode rather than under Algorithms/.
//
// Extracted rather than written per caller: LC 427's and LC 558's harnesses each walked result
// trees back into grids to assert reconstruction, and LC 558's two grid-materializing strategies
// each decoded both of their inputs - three copies of the same recursion, which a repo-wide
// structural-duplication check flagged. All three now share this one.
//
// size is the caller's, not the tree's: a single leaf represents a grid of any size, so the tree
// alone cannot say how large the grid it describes is (the same reason
// OrByBruteForceGridMaterialize takes it as an extra required input).
internal static class QuadTreeGrid
{
    public static int[][] Materialize(QuadTreeNode root, int size)
    {
        var grid = Allocate(size);
        Fill(root, grid, new Region(0, 0, size));

        return grid;
    }

    public static int[][] Allocate(int size)
    {
        var grid = new int[size][];

        for (var row = 0; row < size; row++)
        {
            grid[row] = new int[size];
        }

        return grid;
    }

    // Presumes what QuadTreeNode's own invariant promises: a non-leaf node carries all four
    // quadrants. Nothing in the type system enforces it - the shape is all-or-nothing, and every
    // builder of one (ConstructQuadTreeSolution, and the merge in
    // LogicalOrOfTwoBinaryGridsRepresentedAsQuadTreesSolution) sets IsLeaf false only in the
    // initializer that fills all four - so this walk asks for the quadrants directly, the same
    // presumption those two make.
    private static void Fill(QuadTreeNode node, int[][] grid, Region region)
    {
        if (node.IsLeaf)
        {
            var value = node.Val ? 1 : 0;

            for (var row = region.Row; row < region.Row + region.Size; row++)
            {
                for (var col = region.Col; col < region.Col + region.Size; col++)
                {
                    grid[row][col] = value;
                }
            }

            return;
        }

        var half = region.Size / AlgorithmConstants.HalvingFactor;
        Fill(node.TopLeft!, grid, new Region(region.Row, region.Col, half));
        Fill(node.TopRight!, grid, new Region(region.Row, region.Col + half, half));
        Fill(node.BottomLeft!, grid, new Region(region.Row + half, region.Col, half));
        Fill(node.BottomRight!, grid, new Region(region.Row + half, region.Col + half, half));
    }

    // The (origin, size) pair the recursion narrows - two values that always travel together here,
    // never independently (the same "group related parameters into a type" recipe
    // SegmentRange/Searching.SearchRange follow). Private: no caller outside this walk names a
    // region, so it stays out of the type's surface (§5 step 1).
    private readonly record struct Region(int Row, int Col, int Size);
}
