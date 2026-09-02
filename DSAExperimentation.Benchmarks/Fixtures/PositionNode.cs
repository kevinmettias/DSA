namespace DSAExperimentation.Benchmarks.Fixtures;

// Mirrors DSAExperimentation.Tests' MinimumReverseOperations PositionNode fixture. A
// reference type so it satisfies every TNode : class constraint in this library;
// record equality makes two PositionNodes at the same position on the same Board
// compare equal for visited-tracking.
internal sealed record PositionNode(int Position, ReversalBoard Board);
