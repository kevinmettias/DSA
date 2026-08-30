using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.DisjointSet;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Number of Operations to Make Network Connected (LC 1319): a DFS flood fill
// over an adjacency list built from the connections (baseline - the textbook
// approach, NumberOfProvincesBenchmarks' own precedent for this exact
// DFS-vs-Union-Find contrast) vs. this repo's own DisjointSet unioning every
// connection and counting distinct roots with this repo's own Set<int>. A
// spanning tree is generated first so every run has >= n-1 cables (the
// interesting, non-trivial case), then extra random edges give Union-Find real
// merge-avoiding work to do.
[MemoryDiagnoser]
public class NumberOfOperationsToMakeNetworkConnectedBenchmarks
{
    [Params(50, 300)]
    public int ComputerCount;

    private int[][] _connections = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1319);
        var connections = new List<int[]>();

        for (var i = 1; i < ComputerCount; i++)
        {
            connections.Add([random.Next(i), i]);
        }

        for (var e = 0; e < ComputerCount; e++)
        {
            var a = random.Next(ComputerCount);
            var b = random.Next(ComputerCount);

            if (a != b)
            {
                connections.Add([a, b]);
            }
        }

        _connections = [.. connections];
    }

    [Benchmark(Baseline = true)]
    public int DepthFirstFloodFill()
    {
        var adjacency = BuildAdjacency();
        var visited = new bool[ComputerCount];
        var components = 0;

        for (var i = 0; i < ComputerCount; i++)
        {
            if (visited[i])
            {
                continue;
            }

            Visit(i, adjacency, visited);
            components++;
        }

        return components - 1;
    }

    private int[][] BuildAdjacency()
    {
        var adjacency = new List<int>[ComputerCount];
        for (var i = 0; i < ComputerCount; i++)
        {
            adjacency[i] = [];
        }

        foreach (var connection in _connections)
        {
            adjacency[connection[0]].Add(connection[1]);
            adjacency[connection[1]].Add(connection[0]);
        }

        var result = new int[ComputerCount][];
        for (var i = 0; i < ComputerCount; i++)
        {
            result[i] = [.. adjacency[i]];
        }

        return result;
    }

    private static void Visit(int node, int[][] adjacency, bool[] visited)
    {
        visited[node] = true;

        foreach (var next in adjacency[node])
        {
            if (!visited[next])
            {
                Visit(next, adjacency, visited);
            }
        }
    }

    [Benchmark]
    public int DisjointSetUnionFind()
    {
        var components = new DisjointSet(ComputerCount);

        foreach (var connection in _connections)
        {
            components.Union(connection[0], connection[1]);
        }

        var roots = new Set<int>();
        for (var i = 0; i < ComputerCount; i++)
        {
            roots.TryAdd(components.Find(i));
        }

        return roots.Count - 1;
    }
}
