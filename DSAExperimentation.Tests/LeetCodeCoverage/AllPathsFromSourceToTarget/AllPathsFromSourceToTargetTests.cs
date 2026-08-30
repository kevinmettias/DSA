using DSAExperimentation.Algorithms.Backtracking;

namespace DSAExperimentation.Tests.LeetCodeCoverage.AllPathsFromSourceToTarget;

// LeetCode 797. All Paths From Source to Target: every path is a choose/explore/
// unchoose walk over the DAG's own adjacency list, using this repo's Backtrack -
// the same choose/candidates/unchoose shape CombinationSum's coverage already
// composes, just closed over graph[node] instead of a sorted candidate array. No
// visited-set is needed: the problem guarantees the input is acyclic, so a path
// can never revisit a node on its own.
public sealed partial class AllPathsFromSourceToTargetTests
{
    [Fact]
    public void AllPathsSourceTarget_ClassicExample_ReturnsEveryPath()
    {
        int[][] graph = [[1, 2], [3], [3], []];

        var paths = AllPathsSourceTarget(graph).Select(p => p.ToArray()).ToArray();

        Assert.Equal(2, paths.Length);
        Assert.Contains(paths, p => p.SequenceEqual([0, 1, 3]));
        Assert.Contains(paths, p => p.SequenceEqual([0, 2, 3]));
    }

    [Fact]
    public void AllPathsSourceTarget_SingleNodeGraph_ReturnsTrivialPath()
    {
        int[][] graph = [[]];

        var paths = AllPathsSourceTarget(graph).Select(p => p.ToArray()).ToArray();

        Assert.Equal([[0]], paths);
    }

    private static List<List<int>> AllPathsSourceTarget(int[][] graph)
    {
        var target = graph.Length - 1;
        var results = new List<List<int>>();
        var state = new State();
        state.Path.Add(0);

        Backtrack.Search<State, int>(
            state,
            s => s.Path[^1] == target,
            s => s.Path[^1] == target ? [] : graph[s.Path[^1]],
            (s, next) => s.Path.Add(next),
            (s, _) => s.Path.RemoveAt(s.Path.Count - 1),
            s => results.Add([.. s.Path]));

        return results;
    }

    private sealed class State
    {
        public List<int> Path { get; } = [];
    }
}
