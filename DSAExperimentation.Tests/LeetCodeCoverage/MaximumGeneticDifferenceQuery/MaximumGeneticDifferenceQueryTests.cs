using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumGeneticDifferenceQuery;

// LeetCode 1938. Maximum Genetic Difference Query: node x's own genetic value is x
// itself, so answering "max XOR against any ancestor of node (including node)" is
// an offline DFS over the parent-array tree - insert each node's value into this
// repo's own BitTrie (DataStructures/Graph/Engines/Dags/Trees/BitTrie.cs) on the way
// down, answer every query assigned to that node while it (and every real ancestor)
// is still inserted, then undo the insert on the way back up. BitTrie itself has no
// Remove, so "undo" reuses CountPairsWithXorInARangeTests' own technique -
// HashMap<BitTrieNode,int> tracks how many currently-live values pass through each
// trie node, incremented on the way down and decremented on the way back up - and
// the greedy max-XOR walk prefers the opposite bit only when that child's live count
// is still > 0, falling back to the same-bit child otherwise (which is always live
// by the same "current node has >=1 live child" invariant BitTrie.Walk's own
// precondition note relies on).
public sealed partial class MaximumGeneticDifferenceQueryTests
{
    [Fact]
    public void MaxGeneticDifference_LeetCodeExampleOne_MatchesExpectedAnswers()
    {
        // node 0 (root) -> 1 -> {2, 3}
        int[] parents = [-1, 0, 1, 1];
        int[][] queries = [[0, 2], [3, 2], [2, 5]];

        var answers = MaxGeneticDifference(parents, queries);
        Assert.Equal([2, 3, 7], answers);
    }

    [Fact]
    public void MaxGeneticDifference_LeetCodeExampleTwo_MatchesExpectedAnswers()
    {
        // root=2 -> {3,7}; 3 -> {0}; 7 -> {1,5}; 0 -> {4,6}
        int[] parents = [3, 7, -1, 2, 0, 7, 0, 2];
        int[][] queries = [[4, 6], [1, 15], [0, 5]];

        var answers = MaxGeneticDifference(parents, queries);
        Assert.Equal([6, 14, 7], answers);
    }

    private static int[] MaxGeneticDifference(int[] parents, int[][] queries)
    {
        var children = BuildChildren(parents, out var root);
        var queriesByNode = BuildQueriesByNode(queries, parents.Length);

        var trie = new BitTrie();
        var subtreeCount = new HashMap<BitTrieNode, int>();
        var answers = new int[queries.Length];

        Visit(root, new VisitContext(children, queriesByNode, trie, subtreeCount, answers));

        return answers;
    }

    // The DFS's own tree shape, live-XOR state and output buffer - every field
    // here is the same reference at every level of the recursion; only `node`
    // (Visit's other parameter) changes between calls.
    private readonly record struct VisitContext(
        List<int>[] Children,
        List<(int QueryIndex, int Value)>[] QueriesByNode,
        BitTrie Trie,
        HashMap<BitTrieNode, int> SubtreeCount,
        int[] Answers);

    private static void Visit(int node, VisitContext context)
    {
        Insert(context.Trie, context.SubtreeCount, node);

        foreach (var (queryIndex, value) in context.QueriesByNode[node])
        {
            context.Answers[queryIndex] = MaxXorAmongLive(context.Trie.Root, context.SubtreeCount, value);
        }

        foreach (var child in context.Children[node])
        {
            Visit(child, context);
        }

        Remove(context.Trie, context.SubtreeCount, node);
    }

    private static List<int>[] BuildChildren(int[] parents, out int root)
    {
        var children = new List<int>[parents.Length];
        for (var i = 0; i < parents.Length; i++)
        {
            children[i] = [];
        }

        root = -1;

        for (var i = 0; i < parents.Length; i++)
        {
            if (parents[i] == -1)
            {
                root = i;
            }
            else
            {
                children[parents[i]].Add(i);
            }
        }

        return children;
    }

    private static List<(int QueryIndex, int Value)>[] BuildQueriesByNode(int[][] queries, int n)
    {
        var queriesByNode = new List<(int, int)>[n];
        for (var i = 0; i < n; i++)
        {
            queriesByNode[i] = [];
        }

        for (var i = 0; i < queries.Length; i++)
        {
            queriesByNode[queries[i][0]].Add((i, queries[i][1]));
        }

        return queriesByNode;
    }

    private static void Insert(BitTrie trie, HashMap<BitTrieNode, int> subtreeCount, int value)
    {
        trie.Insert(value);
        AdjustSubtreeCounts(trie, subtreeCount, value, delta: 1);
    }

    private static void Remove(BitTrie trie, HashMap<BitTrieNode, int> subtreeCount, int value)
        => AdjustSubtreeCounts(trie, subtreeCount, value, delta: -1);

    // Shared walk Insert/Remove both do: follow value's bit path from the root,
    // bumping each visited trie node's live count by delta (+1 to insert, -1 to
    // undo).
    private static void AdjustSubtreeCounts(BitTrie trie, HashMap<BitTrieNode, int> subtreeCount, int value, int delta)
    {
        var current = trie.Root;
        var bits = unchecked((uint)value);

        for (var i = 31; i >= 0; i--)
        {
            var bit = (bits >> i) & 1u;
            current = bit == 0 ? current.Zero! : current.One!;
            subtreeCount.TryGetValue(current, out var existing);
            subtreeCount.Set(current, existing + delta);
        }
    }

    // Same greedy "prefer the opposite bit" walk BitTrie.TryMaxXor uses, except a
    // child is only eligible when it still has >=1 live (not-yet-removed) value
    // passing through it - the trie's own structural Zero/One links persist across
    // Remove (BitTrie has no delete), only subtreeCount reflects what's currently
    // "in scope" for this query.
    private static int MaxXorAmongLive(BitTrieNode root, HashMap<BitTrieNode, int> subtreeCount, int value)
    {
        var current = root;
        var bits = unchecked((uint)value);
        var xor = 0u;

        for (var i = 31; i >= 0; i--)
        {
            var bit = (bits >> i) & 1u;
            var opposite = bit == 0 ? current.One : current.Zero;
            var same = bit == 0 ? current.Zero : current.One;

            if (opposite is not null && subtreeCount.TryGetValue(opposite, out var count) && count > 0)
            {
                xor |= 1u << i;
                current = opposite;
            }
            else
            {
                current = same!;
            }
        }

        return unchecked((int)xor);
    }
}
