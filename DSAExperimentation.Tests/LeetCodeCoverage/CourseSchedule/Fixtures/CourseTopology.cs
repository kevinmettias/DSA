using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CourseSchedule.Fixtures;

internal readonly struct CourseTopology : IGraphTopology<CourseNode, ListChildren<CourseNode>>
{
    public static ListChildren<CourseNode> GetChildren(CourseNode node) => new(node.EnabledCourses);
}
