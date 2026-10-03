using DSAExperimentation.LeetCode.AllPathsFromSourceToTarget;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are AllPathsFromSourceToTargetSolution's, the same
// methods AllPathsFromSourceToTargetSolutionTests proves correct. The graph is "layered
// complete" (every node i connects to every later node), so path count grows
// exponentially with node count, giving both strategies real work at both [Params]
// sizes. Building that adjacency list is charged to [GlobalSetup], not to the
// measured walk.
//
// Both arms previously only counted paths; they now build and return LeetCode's
// actual answer, so the measurement includes materializing 2^(n-2) paths in both
// arms alike (ARCHITECTURE.md 17.8's precedent).
public class AllPathsFromSourceToTargetBenchmarks
{
    private int[][] _graph = [];

    [Params(10, 15)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _graph = new int[NodeCount][];

        for (var i = 0; i < NodeCount; i++)
        {
            _graph[i] = Enumerable.Range(i + 1, NodeCount - i - 1).ToArray();
        }
    }

    [Benchmark(Baseline = true)]
    public List<List<int>> SpecializedRecursive() =>
        AllPathsFromSourceToTargetSolution.AllPathsByRecursiveWalk(_graph);

    [Benchmark]
    public List<List<int>> Backtracking() =>
        AllPathsFromSourceToTargetSolution.AllPathsByBacktracking(_graph);
}
