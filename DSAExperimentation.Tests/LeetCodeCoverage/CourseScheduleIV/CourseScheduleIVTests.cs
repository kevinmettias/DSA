using DSAExperimentation.Algorithms.ShortestPaths;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Tests.Algorithms.ShortestPaths.Fixtures;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CourseScheduleIV;

// LeetCode 1462. Course Schedule IV: wire every prerequisite pair as a directed,
// unit-weight edge (prerequisite -> dependent) onto this repo's own
// WeightedNode/WeightedTopology fixtures (already established as reusable
// across LeetCodeCoverage by
// FindTheCityWithTheSmallestNumberOfNeighborsAtAThresholdDistanceTests.cs), then
// answer every [u, v] query from a single
// AllPairsShortestPaths.TryComputeDistances call (Floyd-Warshall): u is a
// prerequisite of v exactly when (u, v) has ANY finite distance in the
// resulting map - the actual number doesn't matter here, only reachability
// does. One all-pairs matrix answers every query in O(1) instead of a fresh
// traversal per query.
public sealed partial class CourseScheduleIVTests
{
    [Fact]
    public void CheckIfPrerequisite_DirectPrerequisiteOnly_ReturnsExpectedAnswers()
    {
        var numCourses = 2;
        int[][] prerequisites = [[1, 0]];
        int[][] queries = [[0, 1], [1, 0]];

        Assert.Equal([false, true], CheckIfPrerequisite(numCourses, prerequisites, queries));
    }

    [Fact]
    public void CheckIfPrerequisite_NoPrerequisitesAtAll_ReturnsAllFalse()
    {
        var numCourses = 2;
        int[][] prerequisites = [];
        int[][] queries = [[1, 0], [0, 1]];

        Assert.Equal([false, false], CheckIfPrerequisite(numCourses, prerequisites, queries));
    }

    [Fact]
    public void CheckIfPrerequisite_TransitivePrerequisite_ReturnsTrue()
    {
        var numCourses = 3;
        int[][] prerequisites = [[1, 2], [1, 0], [2, 0]];
        int[][] queries = [[1, 0], [1, 2]];

        Assert.Equal([true, true], CheckIfPrerequisite(numCourses, prerequisites, queries));
    }

    [Fact]
    public void CheckIfPrerequisite_UnreachableCourseInALongerChain_ReturnsFalse()
    {
        var numCourses = 5;
        int[][] prerequisites = [[0, 1], [1, 2], [2, 3], [3, 4]];
        int[][] queries = [[0, 4], [4, 0], [1, 3]];

        Assert.Equal([true, false, true], CheckIfPrerequisite(numCourses, prerequisites, queries));
    }

    private static List<bool> CheckIfPrerequisite(int numCourses, int[][] prerequisites, int[][] queries)
    {
        var courses = new Dictionary<int, WeightedNode>();
        for (var i = 0; i < numCourses; i++)
        {
            courses[i] = new WeightedNode(i.ToString());
        }

        foreach (var prerequisite in prerequisites)
        {
            var (from, to) = (prerequisite[0], prerequisite[1]);
            courses[from].Edges.Add((1, courses[to]));
        }

        AllPairsShortestPaths.TryComputeDistances<WeightedNode, WeightedTopology, ListEdges<WeightedNode, int>, int>(
            courses.Values, out var distances);

        var answers = new List<bool>();
        foreach (var query in queries)
        {
            answers.Add(distances.ContainsKey((courses[query[0]], courses[query[1]])));
        }

        return answers;
    }
}
