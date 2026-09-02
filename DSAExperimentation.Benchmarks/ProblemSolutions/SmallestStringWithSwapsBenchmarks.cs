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
    // LC problem number, used as the RNG seed.
    private const int RandomSeed = 1202;
    private const int AlphabetSize = 26;

    [Params(200, 5_000)]
    public int Length;

    private string _s = null!;
    private int[][] _pairs = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var chars = new char[Length];
        for (var i = 0; i < Length; i++)
        {
            chars[i] = (char)('a' + random.Next(AlphabetSize));
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
            AssignComponentFromStart(start, adjacency, visited, result);
        }

        return new string(result);
    }

    [Benchmark]
    public string DisjointSetUnionFind()
    {
        var components = BuildUnionFind(Length, _pairs);
        var groups = GroupByRoot(components, Length);

        var result = _s.ToCharArray();
        foreach (var indices in groups.Values)
        {
            AssignSortedChars(_s, result, indices);
        }

        return new string(result);
    }

    // BFS's out from `start` (unless already visited), then writes the component's
    // sorted characters back into `result` at the component's own sorted positions.
    private void AssignComponentFromStart(int start, List<int>[] adjacency, bool[] visited, char[] result)
    {
        if (visited[start])
        {
            return;
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
                VisitNeighbor(next, visited, component, queue);
            }
        }

        component.Sort();
        AssignSortedChars(_s, result, component);
    }

    // Marks `next` visited and enqueues it for its own BFS expansion, unless it was
    // already visited.
    private static void VisitNeighbor(int next, bool[] visited, List<int> component, Queue<int> queue)
    {
        if (visited[next])
        {
            return;
        }

        visited[next] = true;
        component.Add(next);
        queue.Enqueue(next);
    }

    private static DisjointSet BuildUnionFind(int length, int[][] pairs)
    {
        var components = new DisjointSet(length);

        foreach (var pair in pairs)
        {
            components.Union(pair[0], pair[1]);
        }

        return components;
    }

    private static Dictionary<int, List<int>> GroupByRoot(DisjointSet components, int length)
    {
        var groups = new Dictionary<int, List<int>>();

        for (var i = 0; i < length; i++)
        {
            var root = components.Find(i);
            if (!groups.TryGetValue(root, out var indices))
            {
                indices = [];
                groups[root] = indices;
            }

            indices.Add(i);
        }

        return groups;
    }

    // Writes the lexicographically-sorted characters at `positions` (assumed ascending)
    // back into `result` at those same positions.
    private static void AssignSortedChars(string s, char[] result, List<int> positions)
    {
        var sortedChars = positions.Select(i => s[i]).OrderBy(c => c).ToArray();

        for (var j = 0; j < positions.Count; j++)
        {
            result[positions[j]] = sortedChars[j];
        }
    }
}
