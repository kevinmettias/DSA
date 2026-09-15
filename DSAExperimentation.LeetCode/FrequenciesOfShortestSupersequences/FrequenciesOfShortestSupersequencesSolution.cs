using DSAExperimentation.Algorithms.TopologicalSort;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.LeetCode.FrequenciesOfShortestSupersequences;

// LC 3435: every word is a 2-character precedence edge between two of at most 16
// distinct letters. A shortest common supersequence uses every letter once, except
// that any letter caught in a precedence cycle has to appear twice to break it - so
// the answer is: find every minimum-size subset of letters whose doubling makes the
// remaining precedence graph acyclic (a minimum feedback vertex set, enumerated in
// full, not just one instance of it), and report one frequency vector per subset.
// Both strategies share that same subset search; they differ only in how "does
// doubling this subset leave an acyclic graph?" is answered.
internal static class FrequenciesOfShortestSupersequencesSolution
{
    // The textbook arm: a three-color DFS over plain adjacency lists, skipping any
    // node (and any edge touching it) that is in the candidate doubled subset -
    // deliberately no repo primitive here, so it is what the composed arm below has
    // to justify itself against.
    public static int[][] SupersequenceFrequenciesByDfsSkipSet(IEnumerable<string> words) =>
        SupersequenceFrequenciesByDfsSkipSet(LetterGraph.Build(words));

    public static int[][] SupersequenceFrequenciesByDfsSkipSet(LetterGraph graph)
    {
        var adjacency = BuildAdjacencyIndices(graph);

        return SmallestDoubledSubsets(graph, new DfsSkipSetValidity(adjacency));
    }

    private static List<int>[] BuildAdjacencyIndices(LetterGraph graph)
    {
        var index = new Dictionary<char, int>();

        for (var i = 0; i < graph.Letters.Count; i++)
        {
            index[graph.Letters[i]] = i;
        }

        var adjacency = new List<int>[graph.Letters.Count];

        for (var i = 0; i < adjacency.Length; i++)
        {
            adjacency[i] = [];
        }

        foreach (var (from, to) in graph.Edges)
        {
            adjacency[index[from]].Add(index[to]);
        }

        return adjacency;
    }

    // 0 = unvisited, 1 = visiting (on the current DFS path), 2 = done. A node inside
    // doubledMask contributes no outgoing edges to the search, exactly as if it (and
    // every edge touching it) had been deleted from the graph.
    private static bool HasCycleSkipping(List<int>[] adjacency, int doubledMask)
    {
        var state = new int[adjacency.Length];

        for (var node = 0; node < adjacency.Length; node++)
        {
            if (Visit(adjacency, node, state, doubledMask))
            {
                return true;
            }
        }

        return false;
    }

    // One step of the three-color DFS: a node already ON the current path closes a
    // cycle, a node already finished cannot, and an unvisited one descends into its
    // non-doubled neighbors before being marked done.
    private static bool Visit(List<int>[] adjacency, int node, int[] state, int doubledMask)
    {
        if (state[node] != 0)
        {
            return state[node] == 1;
        }

        state[node] = 1;

        if ((doubledMask & (1 << node)) == 0)
        {
            foreach (var next in adjacency[node])
            {
                if ((doubledMask & (1 << next)) == 0 && Visit(adjacency, next, state, doubledMask))
                {
                    return true;
                }
            }
        }

        state[node] = 2;
        return false;
    }

    // This repo's own Kahn's algorithm already answers "is this acyclic?" for free -
    // TopologicalSort.TrySort returns false exactly when leftover in-degree signals
    // a cycle (see its own doc comment). So "doubling this subset" reduces to
    // building the induced subgraph over the non-doubled letters and asking
    // TrySort, the same "puzzle reduces to one call into a repo primitive" move
    // OpenTheLockSolution's MinTurnsByReduceGraph makes.
    public static int[][] SupersequenceFrequenciesByTopologicalSort(IEnumerable<string> words) =>
        SupersequenceFrequenciesByTopologicalSort(LetterGraph.Build(words));

    public static int[][] SupersequenceFrequenciesByTopologicalSort(LetterGraph graph) =>
        SmallestDoubledSubsets(graph, new TopologicalSortValidity(graph));

    private static bool IsAcyclicExcluding(LetterGraph graph, int doubledMask)
    {
        var nodesByLetter = BuildInducedLetterGraph(graph, doubledMask);

        return TopologicalSort.TrySort<
            LetterNode, LetterTopology, ListChildren<LetterNode>,
            NaturalChildOrder<LetterNode, ListChildren<LetterNode>>, ListChildren<LetterNode>>(
            nodesByLetter.Values, out _);
    }

    // The subgraph left once every doubled letter is deleted: its nodes are the
    // undoubled letters, and an edge survives only when both of its endpoints did.
    private static Dictionary<char, LetterNode> BuildInducedLetterGraph(LetterGraph graph, int doubledMask)
    {
        var nodesByLetter = new Dictionary<char, LetterNode>();

        for (var i = 0; i < graph.Letters.Count; i++)
        {
            if ((doubledMask & (1 << i)) == 0)
            {
                var letter = graph.Letters[i];
                nodesByLetter[letter] = new LetterNode(letter);
            }
        }

        foreach (var (from, to) in graph.Edges)
        {
            if (nodesByLetter.TryGetValue(from, out var fromNode) &&
                nodesByLetter.TryGetValue(to, out var toNode))
            {
                fromNode.Neighbors.Add(toNode);
            }
        }

        return nodesByLetter;
    }

    // The one question the two strategies answer differently: does doubling this
    // subset of letters leave the precedence graph acyclic? The subset is named by
    // the mask that selects it, and the contract a bare `Func<int, bool>` had
    // nowhere to state - bit i is letter i of graph.Letters, a set bit means that
    // letter has to appear twice - has somewhere to be written down.
    private interface IDoubledSubsetValidity
    {
        bool IsValid(int doubledMask);
    }

    // Shared search driver: grows the doubled-subset size from 0 until at least one
    // subset of that size is valid, then returns every valid subset of that
    // (minimum) size as one frequency vector each - pure bookkeeping shared by both
    // strategies, the same role Domain.Locks.LockGraph.WheelTurnNeighbors plays for
    // OpenTheLock's two arms.
    private static int[][] SmallestDoubledSubsets(LetterGraph graph, IDoubledSubsetValidity isValidDoubledSubset)
    {
        var n = graph.Letters.Count;

        for (var size = 0; size <= n; size++)
        {
            var validMasks = MasksOfPopcount(n, size).Where(isValidDoubledSubset.IsValid).ToArray();

            if (validMasks.Length > 0)
            {
                return validMasks.Select(mask => BuildFrequency(graph.Letters, mask)).ToArray();
            }
        }

        return [];
    }

    // The textbook arm's answer: three-color DFS over the plain adjacency lists.
    private sealed class DfsSkipSetValidity(List<int>[] adjacency) : IDoubledSubsetValidity
    {
        public bool IsValid(int doubledMask) => !HasCycleSkipping(adjacency, doubledMask);
    }

    // The composed arm's answer: one call into this repo's own topological sort.
    private sealed class TopologicalSortValidity(LetterGraph graph) : IDoubledSubsetValidity
    {
        public bool IsValid(int doubledMask) => IsAcyclicExcluding(graph, doubledMask);
    }

    private static IEnumerable<int> MasksOfPopcount(int bitCount, int popcount)
    {
        for (var mask = 0; mask < (1 << bitCount); mask++)
        {
            if (PopCount(mask) == popcount)
            {
                yield return mask;
            }
        }
    }

    private static int PopCount(int mask)
    {
        var count = 0;

        while (mask != 0)
        {
            count += mask & 1;
            mask >>= 1;
        }

        return count;
    }

    private static int[] BuildFrequency(IReadOnlyList<char> letters, int doubledMask)
    {
        var frequency = new int[26];

        for (var i = 0; i < letters.Count; i++)
        {
            var isDoubled = (doubledMask & (1 << i)) != 0;
            frequency[letters[i] - 'a'] = isDoubled ? 2 : 1;
        }

        return frequency;
    }
}
