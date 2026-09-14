namespace DSAExperimentation.LeetCode.MinimumReverseOperations;

// One array position of LC 2612's implicit graph, carrying a reference to the board
// whose arithmetic decides what it is adjacent to.
//
// A reference type, not a value type - every topology/walk/reduce contract in this
// library constrains TNode : class, so a PositionNode has to be a class the same way
// DataStructures.Graph.Grids' GridNode is. Record equality still does the right thing
// for visited-tracking: two PositionNodes at the same array position on the same
// board compare equal.
//
// Answers this problem alone, so it lives beside the solution rather than in Domain/
// or DataStructures/ (ARCHITECTURE.md #17.3/#17.6). It previously existed as a Tests
// fixture and again in Benchmarks/Fixtures; this is the declaration both harnesses
// now share.
internal sealed record PositionNode(int Position, ReversalBoard Board);
