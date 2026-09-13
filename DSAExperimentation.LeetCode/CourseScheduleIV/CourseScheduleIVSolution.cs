using DSAExperimentation.Algorithms.ShortestPaths;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.LeetCode.CourseScheduleIV;

// LeetCode 1462. Course Schedule IV: given numCourses courses and a list of
// direct prerequisite pairs, answer for every query [u, v] whether u is a
// (possibly indirect) prerequisite of v.
//
// The question is pure reachability on the prerequisite DAG, so both strategies
// differ only in *when* they do the work: one search per query, against one
// all-pairs matrix built up front after which every query is a lookup. Distance
// is never compared to anything - u reaches v exactly when the pair has any
// finite distance at all.
internal static class CourseScheduleIVSolution
{
    // The textbook answer, and the arm the all-pairs strategy has to justify
    // itself against: a fresh O(V+E) breadth-first walk per query with no
    // precomputation, written with a BCL Queue and a bool[] visited map
    // (ARCHITECTURE.md 17.5 - only the graph it is handed is a repo type).
    public static List<bool> CheckIfPrerequisiteByBreadthFirstSearchPerQuery(
        int numCourses, int[][] prerequisites, int[][] queries) =>
        CheckIfPrerequisiteByBreadthFirstSearchPerQuery(
            CourseGraph.Build(numCourses, prerequisites), queries);

    public static List<bool> CheckIfPrerequisiteByBreadthFirstSearchPerQuery(
        CourseGraph graph, int[][] queries)
    {
        var answers = new List<bool>(queries.Length);

        foreach (var query in queries)
        {
            answers.Add(IsReachable(graph, query[0], query[1]));
        }

        return answers;
    }

    private static bool IsReachable(CourseGraph graph, int from, int to)
    {
        if (from == to)
        {
            return true;
        }

        var (visited, pending) = CreateFrontier(graph, from);

        return ReachesTarget(graph, visited, pending, to);
    }

    private static (bool[] Visited, Queue<int> Pending) CreateFrontier(CourseGraph graph, int from)
    {
        var visited = new bool[graph.Courses.Length];
        var pending = new Queue<int>();
        visited[from] = true;
        pending.Enqueue(from);

        return (visited, pending);
    }

    private static bool ReachesTarget(CourseGraph graph, bool[] visited, Queue<int> pending, int to)
    {
        while (pending.Count > 0)
        {
            var current = pending.Dequeue();

            foreach (var (_, dependent) in graph.Courses[current].Edges)
            {
                if (dependent.Id == to)
                {
                    return true;
                }

                if (!visited[dependent.Id])
                {
                    visited[dependent.Id] = true;
                    pending.Enqueue(dependent.Id);
                }
            }
        }

        return false;
    }

    // AllPairsShortestPaths.TryComputeDistances (Floyd-Warshall) answers every
    // (u, v) pair in one call, after which each query is an O(1) dictionary
    // lookup - the same "one all-pairs matrix, many pair lookups" composition
    // FindTheCityWithTheSmallestNumberOfNeighborsAtAThresholdDistanceSolution
    // already uses for this primitive. Unreachable pairs are simply absent from
    // the map rather than carrying a sentinel, so ContainsKey *is* the answer.
    // The bool result reports a negative cycle, which unit-weight prerequisite
    // edges make impossible here.
    public static List<bool> CheckIfPrerequisiteByFloydWarshall(
        int numCourses, int[][] prerequisites, int[][] queries) =>
        CheckIfPrerequisiteByFloydWarshall(CourseGraph.Build(numCourses, prerequisites), queries);

    public static List<bool> CheckIfPrerequisiteByFloydWarshall(CourseGraph graph, int[][] queries)
    {
        _ = AllPairsShortestPaths.TryComputeDistances<
            CourseNode, CourseTopology, ListEdges<CourseNode, int>, int>(
            graph.Courses, out var distances);

        var answers = new List<bool>(queries.Length);

        foreach (var query in queries)
        {
            answers.Add(distances.ContainsKey((graph.Courses[query[0]], graph.Courses[query[1]])));
        }

        return answers;
    }
}
