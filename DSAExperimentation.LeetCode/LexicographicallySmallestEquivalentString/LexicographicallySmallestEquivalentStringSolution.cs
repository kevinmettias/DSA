using DSAExperimentation.DataStructures.DisjointSet;

namespace DSAExperimentation.LeetCode.LexicographicallySmallestEquivalentString;

// LeetCode 1061. Lexicographically Smallest Equivalent String: s1[i] and s2[i] are
// equivalent for every i, equivalence is transitive and symmetric, and baseStr must
// be rewritten so every character becomes the smallest letter in its equivalence
// class.
//
// Both strategies answer the same question - "which letters share a class, and what
// is the smallest member of each" - over the 26 lowercase letters; they differ only
// in how the classes are discovered.
internal static class LexicographicallySmallestEquivalentStringSolution
{
    private const int AlphabetSize = 26;

    // The textbook answer: materialize an adjacency list over the 26 letters and BFS
    // out of each unvisited letter to collect its component, tracking the smallest
    // member seen. Deliberately BCL-only - it is the arm the DisjointSet composition
    // below has to justify itself against.
    public static string SmallestEquivalentStringByAdjacencyListBfs(string s1, string s2, string baseStr)
    {
        var adjacency = BuildAdjacencyList(s1, s2);
        var smallestInGroup = ComputeSmallestPerComponent(adjacency);

        return RemapThroughComponents(baseStr, smallestInGroup);
    }

    private static List<int>[] BuildAdjacencyList(string s1, string s2)
    {
        var adjacency = new List<int>[AlphabetSize];

        for (var letter = 0; letter < AlphabetSize; letter++)
        {
            adjacency[letter] = [];
        }

        for (var i = 0; i < s1.Length; i++)
        {
            AddEquivalencePair(s1[i] - 'a', s2[i] - 'a', adjacency);
        }

        return adjacency;
    }

    private static void AddEquivalencePair(int first, int second, List<int>[] adjacency)
    {
        adjacency[first].Add(second);
        adjacency[second].Add(first);
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

    // This repo's own DisjointSet over the 26 letter ids - the same union-over-26-
    // letters shape SatisfiabilityOfEqualityEquations uses. Union is O(1) amortized
    // per pair and Find is O(a(26)) per baseStr character, with no per-component list
    // allocated at all: one pass records the smallest letter reached per root and one
    // pass remaps baseStr through it.
    public static string SmallestEquivalentStringByDisjointSet(string s1, string s2, string baseStr)
    {
        var equivalences = BuildEquivalences(s1, s2);
        var smallestInGroup = ComputeSmallestPerRoot(equivalences);

        return RemapThroughRoots(baseStr, equivalences, smallestInGroup);
    }

    private static DisjointSet BuildEquivalences(string s1, string s2)
    {
        var equivalences = new DisjointSet(AlphabetSize);

        for (var i = 0; i < s1.Length; i++)
        {
            equivalences.Union(s1[i] - 'a', s2[i] - 'a');
        }

        return equivalences;
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

    private static string RemapThroughRoots(string baseStr, DisjointSet equivalences, char[] smallestInGroup)
    {
        var result = new char[baseStr.Length];

        for (var i = 0; i < baseStr.Length; i++)
        {
            result[i] = smallestInGroup[equivalences.Find(baseStr[i] - 'a')];
        }

        return new string(result);
    }

    private static string RemapThroughComponents(string baseStr, char[] smallestInGroup)
    {
        var result = new char[baseStr.Length];

        for (var i = 0; i < baseStr.Length; i++)
        {
            result[i] = smallestInGroup[baseStr[i] - 'a'];
        }

        return new string(result);
    }
}
