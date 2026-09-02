using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.Benchmarks.Fixtures;

// Mirrors DSAExperimentation.Tests' MinimumNumberOfVisitedCellsInAGrid
// JumpGridChildren fixture: computed on demand from the cell's own stored jump
// distance, touching only the O(v) cells actually reachable in one move.
internal readonly struct JumpGridChildren(JumpGridNode node) : IChildren<JumpGridNode>
{
    private int JumpDistance => node.Grid.JumpDistance(node.Row, node.Col);

    private int RightReach => Math.Min(JumpDistance, node.Grid.Cols - 1 - node.Col);

    private int DownReach => Math.Min(JumpDistance, node.Grid.Rows - 1 - node.Row);

    public int Count => RightReach + DownReach;

    public JumpGridNode Get(int index)
    {
        var rightReach = RightReach;

        return index < rightReach
            ? new JumpGridNode(node.Row, node.Col + 1 + index, node.Grid)
            : new JumpGridNode(node.Row + 1 + (index - rightReach), node.Col, node.Grid);
    }
}
