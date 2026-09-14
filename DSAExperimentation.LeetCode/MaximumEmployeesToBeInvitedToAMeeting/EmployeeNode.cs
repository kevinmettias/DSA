namespace DSAExperimentation.LeetCode.MaximumEmployeesToBeInvitedToAMeeting;

// One employee, whose single successor is the colleague they favor: favorite[]
// gives every node out-degree exactly one, which is what makes this a functional
// graph. Successors is a list rather than a dedicated single-child contract for
// the same reason ColorGraphNode's is - ListChildren already reads a List<T>, and
// the out-degree-1 constraint is a caller discipline EmployeeGraph.Build enforces,
// not something the type itself states.
//
// Answers LC 2127 alone - a plain adjacency-list graph node with no fixed vertex
// set or modulus - so it lives beside the solution rather than in Domain/ or
// DataStructures/ (ARCHITECTURE.md #17.3/#17.6), the same placement
// ColorGraphNode has. It previously existed twice: once as a Tests fixture and
// once again as a private copy inside
// MaximumEmployeesToBeInvitedToAMeetingBenchmarks; this is the single surviving
// declaration.
internal sealed class EmployeeNode(int id)
{
    public int Id { get; } = id;

    public List<EmployeeNode> Successors { get; } = [];

    public override string ToString() => Id.ToString();
}
