namespace DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

// A fixed 4-ary tree node: a leaf holds one Val for its entire region; an internal
// node ignores Val and holds four equally-sized quadrant children. Distinct from
// BinaryTreeNode (arity 2) and RootedTreeNode (n-ary from a parent array) - this is
// the shape LeetCode hands out for quadtree-over-a-binary-grid problems (used by LC
// 427 Construct Quad Tree and LC 558 Logical Or of Two Binary Grids Represented as
// Quad Trees), so it belongs beside its arity siblings rather than pinned to either
// problem.
/// <summary>
/// A quadtree node: a leaf holds one value for its whole square region, and an internal node holds four quadrants.
/// </summary>
/// <param name="Val">The value of every cell in a leaf's region, true for 1; an internal node ignores it.</param>
/// <param name="IsLeaf">Whether the node holds one value for its region rather than four quadrants.</param>
public sealed record QuadTreeNode(bool Val, bool IsLeaf)
{
    /// <summary>The top-left quadrant, set on every internal node and null on a leaf.</summary>
    public QuadTreeNode? TopLeft { get; init; }

    /// <summary>The top-right quadrant, set on every internal node and null on a leaf.</summary>
    public QuadTreeNode? TopRight { get; init; }

    /// <summary>The bottom-left quadrant, set on every internal node and null on a leaf.</summary>
    public QuadTreeNode? BottomLeft { get; init; }

    /// <summary>The bottom-right quadrant, set on every internal node and null on a leaf.</summary>
    public QuadTreeNode? BottomRight { get; init; }
}
