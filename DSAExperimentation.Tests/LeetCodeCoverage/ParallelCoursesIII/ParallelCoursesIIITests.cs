using DSAExperimentation.Algorithms.TopologicalSort;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Tests.LeetCodeCoverage.ParallelCoursesIII.Fixtures;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ParallelCoursesIII;

// LeetCode 2050. Parallel Courses III: this repo's own Kahn's-algorithm
// TopologicalSort.TrySort (TopologicalSort.cs) supplies a dependency-respecting
// order, then a single forward relaxation pass over that order computes each
// course's earliest completion time - the same "push a running value onto every
// child, relaxed once instead of iterated to a fixed point" shape
// LargestColorValueInADirectedGraphTests already uses, just relaxing a max
// finish-time instead of a per-color count. relations point prerequisite ->
// dependent (CourseTimeNode.Successors), the same edge direction
// CourseScheduleII/LargestColorValueInADirectedGraph already use for Kahn's own
// in-degree bookkeeping. Courses form a guaranteed-acyclic DAG per LC's own
// constraint, so the cycle case TrySort's bool return would signal never fires
// here.
public sealed partial class ParallelCoursesIIITests
{
    [Fact]
    public void MinimumTime_DiamondPrerequisitesWithVaryingDurations_ReturnsCriticalPathLength()
    {
        var nodes = BuildGraph(time: [3, 2, 5], relations: [[1, 3], [2, 3]]);

        Assert.Equal(8, MinimumTime(nodes));
    }

    [Fact]
    public void MinimumTime_NoPrerequisites_ReturnsLongestSingleCourseDuration()
    {
        var nodes = BuildGraph(time: [1, 2, 3, 4], relations: []);

        Assert.Equal(4, MinimumTime(nodes));
    }

    [Fact]
    public void MinimumTime_LinearChainOfPrerequisites_ReturnsSummedCriticalPath()
    {
        var nodes = BuildGraph(time: [3, 2, 5], relations: [[1, 2], [2, 3]]);

        Assert.Equal(10, MinimumTime(nodes));
    }

    private static List<CourseTimeNode> BuildGraph(int[] time, int[][] relations)
    {
        var nodes = time.Select((t, i) => new CourseTimeNode(i, t)).ToList();

        foreach (var relation in relations)
        {
            nodes[relation[0] - 1].Successors.Add(nodes[relation[1] - 1]);
        }

        return nodes;
    }

    private static int MinimumTime(List<CourseTimeNode> nodes)
    {
        TopologicalSort.TrySort<
            CourseTimeNode, CourseTimeTopology, ListChildren<CourseTimeNode>,
            NaturalChildOrder<CourseTimeNode, ListChildren<CourseTimeNode>>, ListChildren<CourseTimeNode>>(
            nodes, out var ordering);

        var readyAt = nodes.ToDictionary(node => node, _ => 0);
        var best = 0;

        foreach (var node in ordering)
        {
            var finish = readyAt[node] + node.Time;
            best = Math.Max(best, finish);

            var children = CourseTimeTopology.GetChildren(node);
            for (var i = 0; i < children.Count; i++)
            {
                var child = children.Get(i);
                readyAt[child] = Math.Max(readyAt[child], finish);
            }
        }

        return best;
    }
}
