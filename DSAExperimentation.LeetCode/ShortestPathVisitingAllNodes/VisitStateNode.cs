namespace DSAExperimentation.LeetCode.ShortestPathVisitingAllNodes;

// One node per (current node, bitmask of nodes visited so far) state of LC 847's
// walk; Neighbors holds every state reachable by taking one edge of the input
// graph from here, wired once while VisitStateGraph is built - the same
// "materialize every state, then wire neighbors" shape FlightStateGraph and
// DigitStepGraph use for their own problems' state spaces.
//
// This lives beside the solution rather than in Domain/ because the state it
// models - "where I am, plus which nodes I have already touched" - is LC 847's
// own question and answers nothing else (ARCHITECTURE.md 17.3).
internal sealed record VisitStateNode(int Node, int Mask)
{
    public List<VisitStateNode> Neighbors { get; } = [];
}
