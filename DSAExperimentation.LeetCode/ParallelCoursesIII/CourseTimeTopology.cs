using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.LeetCode.ParallelCoursesIII;

internal readonly struct CourseTimeTopology : IGraphTopology<CourseTimeNode, ListChildren<CourseTimeNode>>
{
    public static ListChildren<CourseTimeNode> GetChildren(CourseTimeNode node) => new(node.Successors);
}
