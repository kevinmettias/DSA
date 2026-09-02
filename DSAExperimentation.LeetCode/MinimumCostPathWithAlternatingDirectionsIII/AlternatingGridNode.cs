namespace DSAExperimentation.LeetCode.MinimumCostPathWithAlternatingDirectionsIII;

// One cell of the m x n grid crossed with which action-parity comes next - the
// actual state ShortestPath.Dijkstra has to search over, since the parity rule
// changes which of the four moves are free of penalty but never which moves are
// legal (every direction is always allowed; wait is a fifth option handled by
// AlternatingGridTopology). Carries its own penalty grid by reference so
// GetEdges - a static-abstract member, so it can carry no instance state of its
// own - can compute a node's neighbors with no external setup step. Every node
// built during one search shares the same array instance, so record equality's
// reference comparison on Penalty never stops two same-coordinate, same-parity
// states from comparing equal.
internal sealed record AlternatingGridNode(int Row, int Col, bool NextActionIsOdd, int[][] Penalty)
{
    public long EntranceCost => (long)(Row + 1) * (Col + 1);
}
