using System.Numerics;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.LongestPalindromicPathInGraph;

// LeetCode 3615. Longest Palindromic Path in Graph: the longest simple path
// (n <= 14) whose visited-node labels read the same forwards and backwards.
internal static class LongestPalindromicPathInGraphSolution
{
    // The textbook answer: hand-rolled DFS over a visited bitmask, enumerating
    // every simple path and checking the palindrome condition on the path
    // accumulated so far - the arm the memoized strategy below has to justify
    // itself against. Deliberately no repo primitives in its internals.
    public static int LongestPalindromeByBruteForceDfs(int nodeCount, int[][] edges, string label)
    {
        var graph = LabeledGraph.Build(nodeCount, edges, label);

        return LongestPalindromeByBruteForceDfs(graph);
    }

    public static int LongestPalindromeByBruteForceDfs(LabeledGraph graph)
    {
        var best = 0;
        var path = new List<int>();

        for (var start = 0; start < graph.NodeCount; start++)
        {
            path.Add(start);
            Explore(graph, 1 << start, path, ref best);
            path.RemoveAt(path.Count - 1);
        }

        return best;
    }

    private static void Explore(LabeledGraph graph, int visited, List<int> path, ref int best)
    {
        // The node the walk stands on is always the path's last entry - every caller
        // adds it before descending - so it is read from the path rather than passed
        // beside it, where a swap with `visited` would have compiled.
        var current = path[path.Count - 1];

        if (IsPalindrome(graph.Label, path))
        {
            best = Math.Max(best, path.Count);
        }

        var candidates = graph.NeighborMask[current] & ~visited;

        for (var bits = candidates; bits != 0; bits &= bits - 1)
        {
            var next = BitOperations.TrailingZeroCount(bits);
            path.Add(next);
            Explore(graph, visited | (1 << next), path, ref best);
            path.RemoveAt(path.Count - 1);
        }
    }

    private static bool IsPalindrome(string label, List<int> path)
    {
        var left = 0;
        var right = path.Count - 1;

        while (left < right)
        {
            if (label[path[left]] != label[path[right]])
            {
                return false;
            }

            left++;
            right--;
        }

        return true;
    }

    // Every palindrome is symmetric around its center, so build it from the
    // center outward instead of walking end to end: dp(mask, u, v) is the
    // longest palindrome reachable by growing further from the pair (u, v),
    // which only ever needs a same-labeled, still-unused neighbor of u and one
    // of v. Algorithms.DynamicProgramming.Memoizer gives this recurrence a
    // single shared cache across every center - a lone node (odd-length center)
    // or a same-labeled edge (even-length center) - so no (mask, u, v) triple is
    // explored twice regardless of how many centers reach it.
    public static int LongestPalindromeByBitmaskMemo(int nodeCount, int[][] edges, string label)
    {
        var graph = LabeledGraph.Build(nodeCount, edges, label);

        return LongestPalindromeByBitmaskMemo(graph);
    }

    public static int LongestPalindromeByBitmaskMemo(LabeledGraph graph)
    {
        var root = (Mask: 0, U: -1, V: -1);

        return Memoizer.Memoize<(int Mask, int U, int V), int>(root, new PalindromeGrowth(graph));
    }

    // The two center families a palindrome can grow from: a lone node (odd
    // length, center repeated once) or an edge whose endpoints already share a
    // label (even length, center repeated twice).
    private static int ExpandFromCenters(LabeledGraph graph, IRecurrence<(int Mask, int U, int V), int> rec)
    {
        var best = LoneNodeCenters(graph, rec);
        var evenLength = EdgeCenters(graph, rec);

        return Math.Max(best, evenLength);
    }

    // Odd-length centers: a single node, with u and v both standing on it.
    private static int LoneNodeCenters(LabeledGraph graph, IRecurrence<(int Mask, int U, int V), int> rec)
    {
        var best = 0;

        for (var node = 0; node < graph.NodeCount; node++)
        {
            var loneCenter = rec.Replay((1 << node, node, node), rec);
            best = Math.Max(best, loneCenter);
        }

        return best;
    }

    // Even-length centers: a same-labeled edge. The candidate mask keeps only
    // endpoints ordered after u, so each edge is offered exactly once.
    private static int EdgeCenters(LabeledGraph graph, IRecurrence<(int Mask, int U, int V), int> rec)
    {
        var best = 0;

        for (var u = 0; u < graph.NodeCount; u++)
        {
            for (var bits = graph.NeighborMask[u] & ~((1 << (u + 1)) - 1); bits != 0; bits &= bits - 1)
            {
                var v = BitOperations.TrailingZeroCount(bits);

                if (graph.Label[u] == graph.Label[v])
                {
                    var edgeCenter = rec.Replay(((1 << u) | (1 << v), u, v), rec);
                    best = Math.Max(best, edgeCenter);
                }
            }
        }

        return best;
    }

    // Both ends are already placed, so a longer palindrome pairs an unused
    // same-labeled neighbor of u with one of v, tried over every choice of u's.
    private static int ExpandFromEnds(
        LabeledGraph graph, (int Mask, int U, int V) state, IRecurrence<(int Mask, int U, int V), int> rec)
    {
        var best = BitOperations.PopCount((uint)state.Mask);
        var uCandidates = graph.NeighborMask[state.U] & ~state.Mask;

        for (var uBits = uCandidates; uBits != 0; uBits &= uBits - 1)
        {
            var nextU = BitOperations.TrailingZeroCount(uBits);
            var grownFromU = BestGrowthFrom(graph, state, nextU, rec);

            best = Math.Max(best, grownFromU);
        }

        return best;
    }

    // The best palindrome extendable from one fixed nextU: the pair is closed by
    // any distinct, unused neighbor of v that carries nextU's label.
    private static int BestGrowthFrom(
        LabeledGraph graph, (int Mask, int U, int V) state, int nextU, IRecurrence<(int Mask, int U, int V), int> rec)
    {
        var best = 0;
        var vCandidates = graph.NeighborMask[state.V] & ~state.Mask;

        for (var vBits = vCandidates; vBits != 0; vBits &= vBits - 1)
        {
            var nextV = BitOperations.TrailingZeroCount(vBits);

            if (nextU == nextV || graph.Label[nextU] != graph.Label[nextV])
            {
                continue;
            }

            var grown = rec.Replay((state.Mask | (1 << nextU) | (1 << nextV), nextU, nextV), rec);
            best = Math.Max(best, grown);
        }

        return best;
    }

    // The recurrence, named: an unstarted mask picks a center, and a started one grows
    // one matched-label node onto each end.
    private sealed class PalindromeGrowth(LabeledGraph graph) : IRecurrence<(int Mask, int U, int V), int>
    {
        /// <inheritdoc/>
        public int Replay((int Mask, int U, int V) state, IRecurrence<(int Mask, int U, int V), int> rest)
        {
            if (state.Mask == 0)
            {
                return ExpandFromCenters(graph, rest);
            }

            return ExpandFromEnds(graph, state, rest);
        }
    }
}
