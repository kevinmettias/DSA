using DSAExperimentation.DataStructures.HashMap;
using PairStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.LeetCode.ValidArrangementOfPairs;

// LeetCode 2097. Valid Arrangement of Pairs: reorder the given directed pairs so
// that every pair's first element equals the previous pair's second - a directed
// multigraph Eulerian path, the same Hierholzer shape ReconstructItinerary (LC 332)
// already uses, except this problem accepts ANY valid arrangement rather than the
// lexicographically smallest one. That is why a plain LIFO bucket per node suffices
// here where LC 332 needs an ordered container: only "every pair consumed exactly
// once" matters, not which unconsumed pair is taken next.
//
// The walk has to start at the node with one more outgoing pair than incoming
// (outdegree - indegree == 1). The problem guarantees a valid arrangement exists, so
// that node is unique whenever it exists at all; when every node balances instead,
// the pairs form an Eulerian circuit and any node with an outgoing pair works, so
// the start falls back to pairs[0]'s source.
//
// Both strategies run that identical algorithm and differ only in the containers
// holding each node's unconsumed destinations: a BCL Dictionary<int, List<int>>
// removing the LAST entry (List.RemoveAt(Count - 1) is itself O(1), so the baseline
// is not handicapped by an unrelated shift cost - the comparison stays about
// container overhead, not a rigged removal position), against this repo's own
// HashMap<int, Stack<int>>, whose Stack composes DynamicArray the same way. Same
// asymptotic shape either side; the difference is each container's hashing and
// indirection cost.
internal static class ValidArrangementOfPairsSolution
{
    // A pair is [from, to].
    private const int From = 0;
    private const int To = 1;

    // outdegree - indegree at the one node an Eulerian path may start from.
    private const int SourceImbalance = 1;

    // The textbook answer: BCL dictionaries and lists throughout, the walk written
    // out by hand. This is the arm the composed strategy below has to justify
    // itself against, so its internals stay deliberately free of this repo.
    public static int[][] ValidArrangementByDictionaryWithList(int[][] pairs)
    {
        var adjacency = new Dictionary<int, List<int>>();
        var outMinusIn = new Dictionary<int, int>();

        foreach (var pair in pairs)
        {
            RecordPairInDictionary(pair, adjacency, outMinusIn);
        }

        var route = new List<int>();
        var start = FindImbalancedSourceInDictionary(pairs, outMinusIn);
        VisitWithList(start, adjacency, route);
        route.Reverse();

        return BuildArrangement(route);
    }

    private static void RecordPairInDictionary(
        int[] pair, Dictionary<int, List<int>> adjacency, Dictionary<int, int> outMinusIn)
    {
        if (!adjacency.TryGetValue(pair[From], out var destinations))
        {
            destinations = [];
            adjacency[pair[From]] = destinations;
        }

        destinations.Add(pair[To]);
        outMinusIn[pair[From]] = outMinusIn.GetValueOrDefault(pair[From]) + 1;
        outMinusIn[pair[To]] = outMinusIn.GetValueOrDefault(pair[To]) - 1;
    }

    // Scanned in the order the pairs were given rather than over the balance map's
    // keys: the imbalanced node always has an outgoing pair, so it is always some
    // pair's source, and scanning that way keeps the chosen start independent of any
    // container's enumeration order - so both strategies pick the same one even on a
    // random benchmark workload that has no valid arrangement to make it unique.
    private static int FindImbalancedSourceInDictionary(int[][] pairs, Dictionary<int, int> outMinusIn)
    {
        foreach (var pair in pairs)
        {
            if (outMinusIn.GetValueOrDefault(pair[From]) == SourceImbalance)
            {
                return pair[From];
            }
        }

        return pairs[0][From];
    }

    // The same Hierholzer walk over this repo's own containers: HashMap for the
    // adjacency and the degree balances, Stack for each node's unconsumed
    // destinations, whose TryPop is exactly the "take one edge and never look at it
    // again" step the algorithm wants.
    public static int[][] ValidArrangementByRepoHashMapWithStack(int[][] pairs)
    {
        var adjacency = new HashMap<int, PairStack>();
        var outMinusIn = new HashMap<int, int>();

        foreach (var pair in pairs)
        {
            RecordPairInHashMap(pair, adjacency, outMinusIn);
        }

        var route = new List<int>();
        var start = FindImbalancedSourceInHashMap(pairs, outMinusIn);
        VisitWithStack(start, adjacency, route);
        route.Reverse();

        return BuildArrangement(route);
    }

    private static void RecordPairInHashMap(
        int[] pair, HashMap<int, PairStack> adjacency, HashMap<int, int> outMinusIn)
    {
        if (!adjacency.TryGetValue(pair[From], out var destinations))
        {
            destinations = new PairStack();
            adjacency.Set(pair[From], destinations);
        }

        destinations.Push(pair[To]);

        outMinusIn.TryGetValue(pair[From], out var outBalance);
        outMinusIn.Set(pair[From], outBalance + 1);
        outMinusIn.TryGetValue(pair[To], out var inBalance);
        outMinusIn.Set(pair[To], inBalance - 1);
    }

    private static int FindImbalancedSourceInHashMap(int[][] pairs, HashMap<int, int> outMinusIn)
    {
        foreach (var pair in pairs)
        {
            outMinusIn.TryGetValue(pair[From], out var balance);

            if (balance == SourceImbalance)
            {
                return pair[From];
            }
        }

        return pairs[0][From];
    }

    private static void VisitWithList(int node, Dictionary<int, List<int>> adjacency, List<int> route)
    {
        if (adjacency.TryGetValue(node, out var destinations))
        {
            while (destinations.Count > 0)
            {
                var next = destinations[^1];
                destinations.RemoveAt(destinations.Count - 1);
                VisitWithList(next, adjacency, route);
            }
        }

        route.Add(node);
    }

    private static void VisitWithStack(int node, HashMap<int, PairStack> adjacency, List<int> route)
    {
        if (adjacency.TryGetValue(node, out var destinations))
        {
            while (destinations.TryPop(out var next))
            {
                VisitWithStack(next, adjacency, route);
            }
        }

        route.Add(node);
    }

    // Hierholzer produces the node sequence; LeetCode wants the pairs between
    // consecutive nodes, so the arrangement is one shorter than the route.
    private static int[][] BuildArrangement(List<int> route)
    {
        var arrangement = new int[route.Count - 1][];

        for (var i = 0; i < arrangement.Length; i++)
        {
            arrangement[i] = [route[i], route[i + 1]];
        }

        return arrangement;
    }
}
