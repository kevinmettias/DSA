using DSAExperimentation.Algorithms.Backtracking;

namespace DSAExperimentation.LeetCode.AllPathsFromSourceToTarget;

// LeetCode 797. All Paths From Source to Target: every path from node 0 to node
// n-1 of a DAG given as an adjacency list.
//
// Both strategies are the same choose/explore/unchoose walk over graph[node]. No
// visited-set is needed in either: the problem guarantees the input is acyclic, so
// a path can never revisit a node on its own. They differ only in who owns the
// recursion - a hand-written local function, or this repo's Backtrack engine.
internal static class AllPathsFromSourceToTargetSolution
{
    // The textbook answer: plain recursion over a BCL List<int> path buffer,
    // appending a copy whenever the walk reaches the target. It is the arm the
    // Backtrack composition below has to justify itself against, so its internals
    // stay BCL throughout.
    public static List<List<int>> AllPathsByRecursiveWalk(int[][] graph)
    {
        var walk = new PathWalk(graph, graph.Length - 1, [0], []);
        ExtendFrom(0, walk);

        return walk.Paths;
    }

    private static void ExtendFrom(int node, PathWalk walk)
    {
        if (node == walk.Target)
        {
            walk.Paths.Add([.. walk.Path]);
            return;
        }

        foreach (var next in walk.Graph[node])
        {
            walk.Path.Add(next);
            ExtendFrom(next, walk);
            walk.Path.RemoveAt(walk.Path.Count - 1);
        }
    }

    private readonly record struct PathWalk(
        int[][] Graph,
        int Target,
        List<int> Path,
        List<List<int>> Paths);

    // The same walk expressed as this repo's Backtrack.Search: the adjacency list
    // supplies the candidates, Add/RemoveAt are the choose/unchoose inverse pair,
    // and reaching the target is the solution predicate. Candidates returns nothing
    // at the target so the engine stops extending a finished path.
    public static List<List<int>> AllPathsByBacktracking(int[][] graph)
    {
        var target = graph.Length - 1;
        var paths = new List<List<int>>();
        var state = new PathState();
        state.Path.Add(0);

        Backtrack.Search<PathState, int>(
            state,
            s => s.Path[^1] == target,
            s => s.Path[^1] == target ? [] : graph[s.Path[^1]],
            (s, next) => s.Path.Add(next),
            (s, _) => s.Path.RemoveAt(s.Path.Count - 1),
            s => paths.Add([.. s.Path]));

        return paths;
    }

    // Backtrack constrains TState to a class deliberately: Choose/Unchoose mutate
    // the buffer in place and must alias rather than copy.
    private sealed class PathState
    {
        public List<int> Path { get; } = [];
    }
}
