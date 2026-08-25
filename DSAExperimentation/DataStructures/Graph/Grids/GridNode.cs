namespace DSAExperimentation.DataStructures.Graph.Grids;

// A reference type, not a value type - every topology/walk/reduce contract in this
// library constrains TNode : class (identity-based, matching TrackedVisitGuard's
// HashSet<TNode>), so a GridNode has to be a class the same way TestNode/TrieNode/
// WeightedNode are, even though it's small enough to want value semantics. Record
// equality still does the right thing for visited-tracking: two GridNodes at the
// same coordinate in the same Grid compare equal.
internal sealed record GridNode(int Row, int Col, Grid Grid);
