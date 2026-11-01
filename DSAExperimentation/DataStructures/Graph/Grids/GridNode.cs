namespace DSAExperimentation.DataStructures.Graph.Grids;

// A reference type, not a value type - every topology/walk/reduce contract in this
// library constrains TNode : class (identity-based, matching TrackedVisitGuard's
// HashSet<TNode>), so a GridNode has to be a class the same way TestNode/TrieNode/
// WeightedNode are, even though it's small enough to want value semantics. Record
// equality still does the right thing for visited-tracking: two GridNodes at the
// same coordinate in the same Grid compare equal.
/// <summary>
/// A <see cref="Grid"/> cell as a graph node, equal to any other node for the same cell of the same grid.
/// </summary>
/// <param name="Row">The cell's row.</param>
/// <param name="Col">The cell's column.</param>
/// <param name="Grid">The board the cell is on, which decides its neighbours.</param>
public sealed record GridNode(int Row, int Col, Grid Grid);
