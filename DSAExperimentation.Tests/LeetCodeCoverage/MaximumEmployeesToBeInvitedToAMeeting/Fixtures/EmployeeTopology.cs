using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumEmployeesToBeInvitedToAMeeting.Fixtures;

internal readonly struct EmployeeTopology : IGraphTopology<EmployeeNode, ListChildren<EmployeeNode>>
{
    public static ListChildren<EmployeeNode> GetChildren(EmployeeNode node) => new(node.Successors);
}
