namespace DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

// A fixed 4-ary tree node: a leaf holds one Val for its entire region; an internal
// node ignores Val and holds four equally-sized quadrant children. Distinct from
// BinaryTreeNode (arity 2) and RootedTreeNode (n-ary from a parent array) - this is
// the shape LeetCode hands out for quadtree-over-a-binary-grid problems (used by LC
// 427 Construct Quad Tree and LC 558 Logical Or of Two Binary Grids Represented as
// Quad Trees), so it belongs beside its arity siblings rather than pinned to either
// problem.
internal sealed record QuadTreeNode(bool Val, bool IsLeaf)
{
    public QuadTreeNode? TopLeft { get; init; }

    public QuadTreeNode? TopRight { get; init; }

    public QuadTreeNode? BottomLeft { get; init; }

    public QuadTreeNode? BottomRight { get; init; }
}
