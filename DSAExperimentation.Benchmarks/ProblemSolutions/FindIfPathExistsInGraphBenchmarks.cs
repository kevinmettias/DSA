using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.DisjointSet;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Find if Path Exists in Graph (LC 1971): an iterative DFS reachability search
// over an adjacency list built from the edges (baseline - the textbook approach,
// NumberOfOperationsToMakeNetworkConnectedBenchmarks/NumberOfProvincesBenchmarks'
// own precedent for this exact DFS-vs-Union-Find contrast; iterative rather than
// recursive purely to stay overflow-safe at this benchmark's larger NodeCount) vs.
// this repo's own DisjointSet unioning every edge and answering with one
// IsConnected(source, destination) lookup. Source and destination sit in two
// disjoint spanning trees on every run - the worst case for DFS, which must
// exhaust the whole source component before concluding no path exists - so both
// strategies do real, comparable work.
[MemoryDiagnoser]
public class FindIfPathExistsInGraphBenchmarks
{
    private const int RandomSeed = 1971;

    private const int HalfDivisor = 2;

    [Params(300, 3_000)]
    public int NodeCount;

    private int[][] _edges = null!;
    private int _source;
    private int _destination;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var half = NodeCount / HalfDivisor;
        var edges = new List<int[]>();

        // Two separate spanning trees - [0, half) and [half, NodeCount) - so
        // source (0) and destination (NodeCount - 1) never share a component.
        for (var i = 1; i < half; i++)
        {
            edges.Add([random.Next(i), i]);
        }

        for (var i = half + 1; i < NodeCount; i++)
        {
            edges.Add([half + random.Next(i - half), i]);
        }

        _edges = [.. edges];
        _source = 0;
        _destination = NodeCount - 1;
    }

    [Benchmark(Baseline = true)]
    public bool IterativeDepthFirstReachability()
    {
        var adjacency = BuildAdjacency();
        var (visited, stack) = InitializeTraversal();

        TraverseDepthFirst(adjacency, visited, stack);

        return visited[_destination];
    }

    private (bool[] Visited, Stack<int> Stack) InitializeTraversal()
    {
        var visited = new bool[NodeCount];
        var stack = new Stack<int>();
        stack.Push(_source);
        visited[_source] = true;
        return (visited, stack);
    }

    private static void TraverseDepthFirst(int[][] adjacency, bool[] visited, Stack<int> stack)
    {
        while (stack.Count > 0)
        {
            var node = stack.Pop();

            foreach (var next in adjacency[node])
            {
                if (visited[next])
                {
                    continue;
                }

                visited[next] = true;
                stack.Push(next);
            }
        }
    }

    private int[][] BuildAdjacency()
    {
        var adjacency = new List<int>[NodeCount];
        for (var i = 0; i < NodeCount; i++)
        {
            adjacency[i] = [];
        }

        foreach (var edge in _edges)
        {
            adjacency[edge[0]].Add(edge[1]);
            adjacency[edge[1]].Add(edge[0]);
        }

        var result = new int[NodeCount][];
        for (var i = 0; i < NodeCount; i++)
        {
            result[i] = [.. adjacency[i]];
        }

        return result;
    }

    [Benchmark]
    public bool DisjointSetUnionFind()
    {
        var components = new DisjointSet(NodeCount);

        foreach (var edge in _edges)
        {
            components.Union(edge[0], edge[1]);
        }

        return components.IsConnected(_source, _destination);
    }
}
