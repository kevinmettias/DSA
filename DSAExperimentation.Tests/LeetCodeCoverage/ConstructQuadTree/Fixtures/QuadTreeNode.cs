namespace DSAExperimentation.Tests.LeetCodeCoverage.ConstructQuadTree.Fixtures;

// Mirrors LeetCode's own Node: a leaf holds one Val for its entire region; an internal
// node ignores Val and holds four equally-sized quadrant children.
internal sealed class QuadTreeNode(bool val, bool isLeaf)
{
    public bool Val { get; } = val;

    public bool IsLeaf { get; } = isLeaf;

    public QuadTreeNode? TopLeft { get; init; }

    public QuadTreeNode? TopRight { get; init; }

    public QuadTreeNode? BottomLeft { get; init; }

    public QuadTreeNode? BottomRight { get; init; }
}
