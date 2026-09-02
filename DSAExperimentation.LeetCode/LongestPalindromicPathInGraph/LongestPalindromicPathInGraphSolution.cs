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
    public static int LongestPalindromeByBruteForceDfs(int n, int[][] edges, string label) =>
        LongestPalindromeByBruteForceDfs(LabeledGraph.Build(n, edges, label));

    public static int LongestPalindromeByBruteForceDfs(LabeledGraph graph)
    {
        var best = 0;
        var path = new List<int>();

        for (var start = 0; start < graph.NodeCount; start++)
        {
            path.Add(start);
            Explore(graph, start, 1 << start, path, ref best);
            path.RemoveAt(path.Count - 1);
        }

        return best;
    }

    private static void Explore(LabeledGraph graph, int current, int visited, List<int> path, ref int best)
    {
        if (IsPalindrome(graph.Label, path))
        {
            best = Math.Max(best, path.Count);
        }

        var candidates = graph.NeighborMask[current] & ~visited;

        for (var bits = candidates; bits != 0; bits &= bits - 1)
        {
            var next = BitOperations.TrailingZeroCount(bits);
            path.Add(next);
            Explore(graph, next, visited | (1 << next), path, ref best);
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
    public static int LongestPalindromeByBitmaskMemo(int n, int[][] edges, string label) =>
        LongestPalindromeByBitmaskMemo(LabeledGraph.Build(n, edges, label));

    public static int LongestPalindromeByBitmaskMemo(LabeledGraph graph)
    {
        var root = (Mask: 0, U: -1, V: -1);

        return Memoizer.Memoize<(int Mask, int U, int V), int>(root, (state, rec) => Expand(graph, state, rec));
    }

    private static int Expand(
        LabeledGraph graph, (int Mask, int U, int V) state, Func<(int Mask, int U, int V), int> rec)
        => state.Mask == 0 ? ExpandFromCenters(graph, rec) : ExpandFromEnds(graph, state, rec);

    // The two center families a palindrome can grow from: a lone node (odd
    // length, center repeated once) or an edge whose endpoints already share a
    // label (even length, center repeated twice).
    private static int ExpandFromCenters(LabeledGraph graph, Func<(int Mask, int U, int V), int> rec)
    {
        var best = 0;

        for (var node = 0; node < graph.NodeCount; node++)
        {
            best = Math.Max(best, rec((1 << node, node, node)));
        }

        for (var u = 0; u < graph.NodeCount; u++)
        {
            for (var bits = graph.NeighborMask[u] & ~((1 << (u + 1)) - 1); bits != 0; bits &= bits - 1)
            {
                var v = BitOperations.TrailingZeroCount(bits);

                if (graph.Label[u] == graph.Label[v])
                {
                    best = Math.Max(best, rec(((1 << u) | (1 << v), u, v)));
                }
            }
        }

        return best;
    }

    private static int ExpandFromEnds(
        LabeledGraph graph, (int Mask, int U, int V) state, Func<(int Mask, int U, int V), int> rec)
    {
        var best = BitOperations.PopCount((uint)state.Mask);
        var uCandidates = graph.NeighborMask[state.U] & ~state.Mask;
        var vCandidates = graph.NeighborMask[state.V] & ~state.Mask;

        for (var uBits = uCandidates; uBits != 0; uBits &= uBits - 1)
        {
            var nextU = BitOperations.TrailingZeroCount(uBits);

            for (var vBits = vCandidates; vBits != 0; vBits &= vBits - 1)
            {
                var nextV = BitOperations.TrailingZeroCount(vBits);

                if (nextU == nextV || graph.Label[nextU] != graph.Label[nextV])
                {
                    continue;
                }

                var nextMask = state.Mask | (1 << nextU) | (1 << nextV);
                best = Math.Max(best, rec((nextMask, nextU, nextV)));
            }
        }

        return best;
    }
}
