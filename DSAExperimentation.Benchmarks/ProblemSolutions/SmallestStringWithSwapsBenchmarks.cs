using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.DisjointSet;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Smallest String With Swaps (LC 1202): adjacency-list BFS (build a graph over the
// string's own indices from every swap pair, then BFS from each unvisited index to
// find its component) vs. this repo's own DisjointSet(n) - O(1) Union per pair and
// O(a(n)) Find per index, with no per-component allocation for the walk itself. Same
// LexicographicallySmallestEquivalentStringBenchmarks shape, over index positions
// instead of the 26 letters.
[MemoryDiagnoser]
public class SmallestStringWithSwapsBenchmarks
{
    [Params(200, 5_000)]
    public int Length;

    private string _s = null!;
    private int[][] _pairs = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1202);
        var chars = new char[Length];
        for (var i = 0; i < Length; i++)
        {
            chars[i] = (char)('a' + random.Next(26));
        }

        _s = new string(chars);

        _pairs = new int[Length][];
        for (var i = 0; i < Length; i++)
        {
            _pairs[i] = [random.Next(Length), random.Next(Length)];
        }
    }

    [Benchmark(Baseline = true)]
    public string AdjacencyListBfs()
    {
        var adjacency = new List<int>[Length];
        for (var i = 0; i < Length; i++)
        {
            adjacency[i] = [];
        }

        foreach (var pair in _pairs)
        {
            adjacency[pair[0]].Add(pair[1]);
            adjacency[pair[1]].Add(pair[0]);
        }

        var visited = new bool[Length];
        var result = _s.ToCharArray();

        for (var start = 0; start < Length; start++)
        {
            if (visited[start])
            {
                continue;
            }

            var queue = new Queue<int>();
            queue.Enqueue(start);
            visited[start] = true;

            var component = new List<int> { start };
            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                foreach (var next in adjacency[current])
                {
                    if (visited[next])
                    {
                        continue;
                    }

                    visited[next] = true;
                    component.Add(next);
                    queue.Enqueue(next);
                }
            }

            component.Sort();
            var chars = component.Select(i => _s[i]).OrderBy(c => c).ToArray();
            for (var j = 0; j < component.Count; j++)
            {
                result[component[j]] = chars[j];
            }
        }

        return new string(result);
    }

    [Benchmark]
    public string DisjointSetUnionFind()
    {
        var components = new DisjointSet(Length);

        foreach (var pair in _pairs)
        {
            components.Union(pair[0], pair[1]);
        }

        var groups = new Dictionary<int, List<int>>();
        for (var i = 0; i < Length; i++)
        {
            var root = components.Find(i);
            if (!groups.TryGetValue(root, out var indices))
            {
                indices = [];
                groups[root] = indices;
            }

            indices.Add(i);
        }

        var result = _s.ToCharArray();
        foreach (var indices in groups.Values)
        {
            var chars = indices.Select(i => _s[i]).OrderBy(c => c).ToArray();
            for (var j = 0; j < indices.Count; j++)
            {
                result[indices[j]] = chars[j];
            }
        }

        return new string(result);
    }
}
