using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Backtracking;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// All Paths From Source to Target (LC 797): a hand-written recursive choose/
// explore/unchoose walk over the DAG's adjacency list vs. this repo's generic
// Backtrack. The graph is "layered complete" (every node i connects to every
// later node), so path count grows exponentially with node count, giving both
// strategies real work at both [Params] sizes.
[MemoryDiagnoser]
public class AllPathsFromSourceToTargetBenchmarks
{
    [Params(10, 15)]
    public int NodeCount;

    private int[][] _graph = null!;

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
    public int SpecializedRecursive()
    {
        var target = NodeCount - 1;
        var count = 0;
        var path = new List<int> { 0 };

        void Search(int node)
        {
            if (node == target)
            {
                count++;
                return;
            }

            foreach (var next in _graph[node])
            {
                path.Add(next);
                Search(next);
                path.RemoveAt(path.Count - 1);
            }
        }

        Search(0);
        return count;
    }

    [Benchmark]
    public int Backtracking()
    {
        var target = NodeCount - 1;
        var count = 0;
        var state = new State();
        state.Path.Add(0);

        Backtrack.Search<State, int>(
            state,
            s => s.Path[^1] == target,
            s => s.Path[^1] == target ? [] : _graph[s.Path[^1]],
            (s, next) => s.Path.Add(next),
            (s, _) => s.Path.RemoveAt(s.Path.Count - 1),
            _ => count++);

        return count;
    }

    private sealed class State
    {
        public List<int> Path { get; } = [];
    }
}
