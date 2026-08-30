using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.DisjointSet;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Lexicographically Smallest Equivalent String (LC 1061): adjacency-list BFS
// (build a graph over the 26 letters from every s1[i]/s2[i] pair, then BFS from
// each unvisited letter to find its component's smallest member) vs. this
// repo's own DisjointSet(26) - O(1) Union per pair and O(a(26)) Find per
// baseStr character, with no per-component allocation.
[MemoryDiagnoser]
public class LexicographicallySmallestEquivalentStringBenchmarks
{
    [Params(200, 5_000)]
    public int Length;

    private string _s1 = null!;
    private string _s2 = null!;
    private string _baseStr = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1061);
        var s1 = new char[Length];
        var s2 = new char[Length];
        var baseChars = new char[Length];

        for (var i = 0; i < Length; i++)
        {
            s1[i] = (char)('a' + random.Next(26));
            s2[i] = (char)('a' + random.Next(26));
            baseChars[i] = (char)('a' + random.Next(26));
        }

        _s1 = new string(s1);
        _s2 = new string(s2);
        _baseStr = new string(baseChars);
    }

    [Benchmark(Baseline = true)]
    public string AdjacencyListBfs()
    {
        var adjacency = new List<int>[26];
        for (var i = 0; i < 26; i++)
        {
            adjacency[i] = [];
        }

        for (var i = 0; i < _s1.Length; i++)
        {
            var a = _s1[i] - 'a';
            var b = _s2[i] - 'a';
            adjacency[a].Add(b);
            adjacency[b].Add(a);
        }

        var smallestInGroup = new char[26];
        var visited = new bool[26];

        for (var letter = 0; letter < 26; letter++)
        {
            if (visited[letter])
            {
                continue;
            }

            var queue = new Queue<int>();
            queue.Enqueue(letter);
            visited[letter] = true;

            var smallest = (char)('a' + letter);
            var component = new List<int> { letter };

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

                    var candidate = (char)('a' + next);
                    if (candidate < smallest)
                    {
                        smallest = candidate;
                    }
                }
            }

            foreach (var member in component)
            {
                smallestInGroup[member] = smallest;
            }
        }

        var result = new char[_baseStr.Length];
        for (var i = 0; i < _baseStr.Length; i++)
        {
            result[i] = smallestInGroup[_baseStr[i] - 'a'];
        }

        return new string(result);
    }

    [Benchmark]
    public string DisjointSetUnionFind()
    {
        var equivalences = new DisjointSet(26);

        for (var i = 0; i < _s1.Length; i++)
        {
            equivalences.Union(_s1[i] - 'a', _s2[i] - 'a');
        }

        var smallestInGroup = new char[26];
        for (var letter = 0; letter < 26; letter++)
        {
            var root = equivalences.Find(letter);
            var candidate = (char)('a' + letter);

            if (smallestInGroup[root] == default || candidate < smallestInGroup[root])
            {
                smallestInGroup[root] = candidate;
            }
        }

        var result = new char[_baseStr.Length];
        for (var i = 0; i < _baseStr.Length; i++)
        {
            result[i] = smallestInGroup[equivalences.Find(_baseStr[i] - 'a')];
        }

        return new string(result);
    }
}
