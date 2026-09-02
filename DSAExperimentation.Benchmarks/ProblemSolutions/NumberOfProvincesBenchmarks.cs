using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.DisjointSet;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Number of Provinces (LC 547): a DFS flood-fill over the adjacency matrix (baseline,
// the textbook approach - explicit recursion plus a visited array) vs. this repo's
// own DisjointSet unioning every isConnected[i][j] pair and counting distinct roots
// with this repo's own Set<int> - the same Union-Find primitive RedundantConnectionTests
// already proves for cycle detection, applied here to component counting instead.
// Both still touch every cell of the n x n matrix, so this is not a different
// asymptotic class - Union-Find's edge is near-constant-time merging via path
// compression/union-by-rank instead of DFS's own recursion and visited-array
// bookkeeping.
[MemoryDiagnoser]
public class NumberOfProvincesBenchmarks
{
    private const int ConnectionOddsDenominator = 10;

    [Params(50, 300)]
    public int CityCount;

    private int[][] _isConnected = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _isConnected = new int[CityCount][];

        for (var i = 0; i < CityCount; i++)
        {
            _isConnected[i] = new int[CityCount];
            _isConnected[i][i] = 1;
        }

        for (var i = 0; i < CityCount; i++)
        {
            for (var j = i + 1; j < CityCount; j++)
            {
                var connected = random.Next(0, ConnectionOddsDenominator) == 0 ? 1 : 0;
                _isConnected[i][j] = connected;
                _isConnected[j][i] = connected;
            }
        }
    }

    [Benchmark(Baseline = true)]
    public int DepthFirstFloodFill()
    {
        var visited = new bool[CityCount];
        var provinces = 0;

        for (var i = 0; i < CityCount; i++)
        {
            if (visited[i])
            {
                continue;
            }

            Visit(i, visited);
            provinces++;
        }

        return provinces;
    }

    private void Visit(int city, bool[] visited)
    {
        visited[city] = true;

        for (var next = 0; next < CityCount; next++)
        {
            if (_isConnected[city][next] == 1 && !visited[next])
            {
                Visit(next, visited);
            }
        }
    }

    [Benchmark]
    public int DisjointSetUnionFind()
    {
        var components = new DisjointSet(CityCount);

        for (var i = 0; i < CityCount; i++)
        {
            for (var j = i + 1; j < CityCount; j++)
            {
                if (_isConnected[i][j] == 1)
                {
                    components.Union(i, j);
                }
            }
        }

        var roots = new Set<int>();
        for (var i = 0; i < CityCount; i++)
        {
            roots.TryAdd(components.Find(i));
        }

        return roots.Count;
    }
}
