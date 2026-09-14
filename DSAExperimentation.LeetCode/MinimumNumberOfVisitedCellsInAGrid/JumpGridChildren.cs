using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.LeetCode.MinimumNumberOfVisitedCellsInAGrid;

// Computed on demand from the cell's own stored jump distance, the same way
// GridChildren computes its neighbors from geometry instead of storing a
// materialized list: from (row, col) with jump distance v, every (row, col + 1)
// through (row, col + v) is reachable rightward and every (row + 1, col) through
// (row + v, col) is reachable downward, both clamped to the grid's bounds - never
// the full row or column, so this only ever touches the O(v) cells actually
// reachable in one move.
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
