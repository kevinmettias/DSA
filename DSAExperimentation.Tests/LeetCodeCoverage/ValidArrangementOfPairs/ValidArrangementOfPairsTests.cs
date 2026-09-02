using DSAExperimentation.DataStructures.HashMap;
using PairStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ValidArrangementOfPairs;

// LeetCode 2097. Valid Arrangement of Pairs: a directed-multigraph Eulerian path,
// the exact same Hierholzer's-algorithm shape ReconstructItineraryTests (LC 332)
// already proves over this repo's own HashMap<TKey,TValue> - here paired with the
// repo's own Stack<Element> instead of a min-Heap, since this problem only needs
// SOME valid arrangement, not the lexicographically smallest one. The start node is
// whichever node has one more outgoing pair than incoming (outdegree - indegree ==
// 1, guaranteed unique when it exists, per the problem's own guarantee that a valid
// arrangement exists); when every node balances out instead, the pairs form an
// Eulerian circuit and any node with an outgoing pair works, so this falls back to
// pairs[0][0].
public sealed partial class ValidArrangementOfPairsTests
{
    [Fact]
    public void ValidArrangement_UniqueImbalancedSource_StartsFromIt()
    {
        int[][] pairs = [[5, 1], [4, 5], [11, 9], [9, 4]];

        var arrangement = ValidArrangement(pairs);

        AssertIsValidArrangement(pairs, arrangement);
        Assert.Equal(11, arrangement[0][0]);
    }

    [Fact]
    public void ValidArrangement_MixedInAndOutDegrees_StartsFromTheImbalancedSource()
    {
        int[][] pairs = [[1, 2], [1, 3], [2, 1]];

        var arrangement = ValidArrangement(pairs);

        AssertIsValidArrangement(pairs, arrangement);
        Assert.Equal(1, arrangement[0][0]);
    }

    [Fact]
    public void ValidArrangement_EulerianCircuit_FallsBackToFirstPairsSource()
    {
        int[][] pairs = [[1, 3], [3, 2], [2, 1]];

        var arrangement = ValidArrangement(pairs);

        AssertIsValidArrangement(pairs, arrangement);
        Assert.Equal(1, arrangement[0][0]);
    }

    private static void AssertIsValidArrangement(int[][] pairs, int[][] arrangement)
    {
        Assert.Equal(pairs.Length, arrangement.Length);

        for (var i = 1; i < arrangement.Length; i++)
        {
            Assert.Equal(arrangement[i - 1][1], arrangement[i][0]);
        }

        var expected = pairs.Select(pair => (pair[0], pair[1])).OrderBy(pair => pair.Item1).ThenBy(pair => pair.Item2);
        var actual = arrangement.Select(pair => (pair[0], pair[1])).OrderBy(pair => pair.Item1).ThenBy(pair => pair.Item2);
        Assert.Equal(expected, actual);
    }

    private static int[][] ValidArrangement(int[][] pairs)
    {
        var adjacency = new HashMap<int, PairStack>();
        var outMinusIn = new HashMap<int, int>();

        foreach (var pair in pairs)
        {
            RecordPair(pair, adjacency, outMinusIn);
        }

        var start = FindImbalancedSource(pairs, outMinusIn);

        var route = new List<int>();
        Visit(start, adjacency, route);
        route.Reverse();

        return BuildArrangement(route);
    }

    private static void RecordPair(int[] pair, HashMap<int, PairStack> adjacency, HashMap<int, int> outMinusIn)
    {
        if (!adjacency.TryGetValue(pair[0], out var destinations))
        {
            destinations = new PairStack();
            adjacency.Set(pair[0], destinations);
        }

        destinations.Push(pair[1]);

        outMinusIn.TryGetValue(pair[0], out var outBalance);
        outMinusIn.Set(pair[0], outBalance + 1);
        outMinusIn.TryGetValue(pair[1], out var inBalance);
        outMinusIn.Set(pair[1], inBalance - 1);
    }

    private static int FindImbalancedSource(int[][] pairs, HashMap<int, int> outMinusIn)
    {
        var start = pairs[0][0];

        foreach (var node in outMinusIn.Keys)
        {
            outMinusIn.TryGetValue(node, out var balance);

            if (balance == 1)
            {
                start = node;
                break;
            }
        }

        return start;
    }

    private static int[][] BuildArrangement(List<int> route)
    {
        var arrangement = new int[route.Count - 1][];

        for (var i = 0; i < arrangement.Length; i++)
        {
            arrangement[i] = [route[i], route[i + 1]];
        }

        return arrangement;
    }

    private static void Visit(int node, HashMap<int, PairStack> adjacency, List<int> route)
    {
        if (adjacency.TryGetValue(node, out var destinations))
        {
            while (destinations.TryPop(out var next))
            {
                Visit(next, adjacency, route);
            }
        }

        route.Add(node);
    }
}
