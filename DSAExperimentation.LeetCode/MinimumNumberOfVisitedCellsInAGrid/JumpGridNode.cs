namespace DSAExperimentation.LeetCode.MinimumNumberOfVisitedCellsInAGrid;

// A reference type, not a value type - every topology/walk/reduce contract in this
// library constrains TNode : class, the same reason DataStructures.Graph.Grids'
// GridNode is a class. Record equality still does the right thing for
// visited-tracking: two JumpGridNodes at the same coordinate on the same grid
// compare equal.
internal sealed record JumpGridNode(int Row, int Col, JumpGrid Grid);
