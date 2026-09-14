using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.LeetCode.MaximumEmployeesToBeInvitedToAMeeting;

// Bare IGraphTopology, not IDagTopology: favorite[] is guaranteed to contain
// cycles (every out-degree-1 walk ends in one), and finding them is the whole
// problem - so nothing here may promise acyclicity to an engine.
internal readonly struct EmployeeTopology : IGraphTopology<EmployeeNode, ListChildren<EmployeeNode>>
{
    public static ListChildren<EmployeeNode> GetChildren(EmployeeNode node) => new(node.Successors);
}
