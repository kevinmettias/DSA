namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumReverseOperations.Fixtures;

// A reference type, not a value type - every topology/walk/reduce contract in this
// library constrains TNode : class, so a PositionNode has to be a class the same way
// GridNode is. Record equality still does the right thing for visited-tracking: two
// PositionNodes at the same array position on the same Board compare equal.
internal sealed record PositionNode(int Position, ReversalBoard Board);
