namespace DSAExperimentation.Benchmarks.Fixtures;

// Mirrors DSAExperimentation.Tests' MinimumNumberOfVisitedCellsInAGrid JumpGridNode
// fixture. A reference type so it satisfies every TNode : class constraint in this
// library; record equality makes two JumpGridNodes at the same coordinate on the
// same Grid compare equal for visited-tracking.
internal sealed record JumpGridNode(int Row, int Col, JumpGrid Grid);
