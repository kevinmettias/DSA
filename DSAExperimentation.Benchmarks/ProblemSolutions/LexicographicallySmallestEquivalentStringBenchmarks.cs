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
    private const int RandomSeed = 1061; // LC problem number
    private const int AlphabetSize = 26;

    [Params(200, 5_000)]
    public int Length;

    private string _s1 = null!;
    private string _s2 = null!;
    private string _baseStr = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var s1 = new char[Length];
        var s2 = new char[Length];
        var baseChars = new char[Length];

        for (var i = 0; i < Length; i++)
        {
            s1[i] = (char)('a' + random.Next(AlphabetSize));
            s2[i] = (char)('a' + random.Next(AlphabetSize));
            baseChars[i] = (char)('a' + random.Next(AlphabetSize));
        }

        _s1 = new string(s1);
        _s2 = new string(s2);
        _baseStr = new string(baseChars);
    }

    [Benchmark(Baseline = true)]
    public string AdjacencyListBfs()
    {
        var adjacency = BuildAdjacencyList();
        var smallestInGroup = ComputeSmallestPerComponent(adjacency);
        return BuildResultFromComponents(smallestInGroup);
    }

    private List<int>[] BuildAdjacencyList()
    {
        var adjacency = new List<int>[AlphabetSize];
        for (var i = 0; i < AlphabetSize; i++)
        {
            adjacency[i] = [];
        }

        for (var i = 0; i < _s1.Length; i++)
        {
            AddEquivalencePair(i, adjacency);
        }

        return adjacency;
    }

    private void AddEquivalencePair(int i, List<int>[] adjacency)
    {
        var a = _s1[i] - 'a';
        var b = _s2[i] - 'a';
        adjacency[a].Add(b);
        adjacency[b].Add(a);
    }

    private static char[] ComputeSmallestPerComponent(List<int>[] adjacency)
    {
        var smallestInGroup = new char[AlphabetSize];
        var visited = new bool[AlphabetSize];

        for (var letter = 0; letter < AlphabetSize; letter++)
        {
            AssignComponentSmallest(letter, adjacency, visited, smallestInGroup);
        }

        return smallestInGroup;
    }

    private string BuildResultFromComponents(char[] smallestInGroup)
    {
        var result = new char[_baseStr.Length];
        for (var i = 0; i < _baseStr.Length; i++)
        {
            result[i] = smallestInGroup[_baseStr[i] - 'a'];
        }

        return new string(result);
    }

    private static void AssignComponentSmallest(int letter, List<int>[] adjacency, bool[] visited, char[] smallestInGroup)
    {
        if (visited[letter])
        {
            return;
        }

        var (smallest, component) = RunComponentBfs(letter, adjacency, visited);
        AssignSmallestToComponent(component, smallest, smallestInGroup);
    }

    private static (char Smallest, List<int> Component) RunComponentBfs(int letter, List<int>[] adjacency, bool[] visited)
    {
        var queue = new Queue<int>();
        queue.Enqueue(letter);
        visited[letter] = true;

        var smallest = (char)('a' + letter);
        var component = new List<int> { letter };
        var scan = new ComponentScan(visited, component, queue);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            foreach (var next in adjacency[current])
            {
                smallest = VisitCandidate(next, scan, smallest);
            }
        }

        return (smallest, component);
    }

    private static void AssignSmallestToComponent(List<int> component, char smallest, char[] smallestInGroup)
    {
        foreach (var member in component)
        {
            smallestInGroup[member] = smallest;
        }
    }

    private static char VisitCandidate(int next, ComponentScan scan, char smallest)
    {
        if (scan.Visited[next])
        {
            return smallest;
        }

        scan.Visited[next] = true;
        scan.Component.Add(next);
        scan.Queue.Enqueue(next);

        var candidate = (char)('a' + next);
        return candidate < smallest ? candidate : smallest;
    }

    private readonly record struct ComponentScan(bool[] Visited, List<int> Component, Queue<int> Queue);

    [Benchmark]
    public string DisjointSetUnionFind()
    {
        var equivalences = new DisjointSet(AlphabetSize);
        UnionEquivalentLetters(equivalences);
        var smallestInGroup = ComputeSmallestPerRoot(equivalences);
        return BuildResultFromRoots(equivalences, smallestInGroup);
    }

    private void UnionEquivalentLetters(DisjointSet equivalences)
    {
        for (var i = 0; i < _s1.Length; i++)
        {
            equivalences.Union(_s1[i] - 'a', _s2[i] - 'a');
        }
    }

    private static char[] ComputeSmallestPerRoot(DisjointSet equivalences)
    {
        var smallestInGroup = new char[AlphabetSize];
        for (var letter = 0; letter < AlphabetSize; letter++)
        {
            var root = equivalences.Find(letter);
            var candidate = (char)('a' + letter);

            if (smallestInGroup[root] == default || candidate < smallestInGroup[root])
            {
                smallestInGroup[root] = candidate;
            }
        }

        return smallestInGroup;
    }

    private string BuildResultFromRoots(DisjointSet equivalences, char[] smallestInGroup)
    {
        var result = new char[_baseStr.Length];
        for (var i = 0; i < _baseStr.Length; i++)
        {
            result[i] = smallestInGroup[equivalences.Find(_baseStr[i] - 'a')];
        }

        return new string(result);
    }
}
