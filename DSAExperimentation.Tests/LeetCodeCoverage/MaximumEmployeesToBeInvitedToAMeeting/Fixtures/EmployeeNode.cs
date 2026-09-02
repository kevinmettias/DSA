namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumEmployeesToBeInvitedToAMeeting.Fixtures;

// Successors always holds exactly one entry - favorite[i] - matching
// ColorGraphNode's "one class, IGraphTopology reads its own successor list"
// shape (LargestColorValueInADirectedGraph precedent) rather than a
// dedicated single-child contract; the out-degree-1 constraint is a caller
// discipline BuildGraph enforces, not something the type itself states.
internal sealed class EmployeeNode(int id)
{
    public int Id { get; } = id;

    public List<EmployeeNode> Successors { get; } = [];

    public override string ToString() => Id.ToString();
}
