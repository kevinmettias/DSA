using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.MaximumGeneticDifferenceQuery;

// LeetCode 1938. Maximum Genetic Difference Query: parents[] encodes a rooted tree
// (DataStructures' own ParentArrayTree shape, except LC lets the -1 root sit at any
// index, not only 0) and a node's genetic value is its own id. Each query
// [node, val] asks for the largest val XOR x over every x on the root-to-node path,
// inclusive.
//
// Both strategies answer exactly that; they differ in how much of the ancestor set
// they re-walk per query.
internal static class MaximumGeneticDifferenceQuerySolution
{
    // Bit 31 is the most significant bit of a 32-bit int; every bit walk below
    // processes all 32 bits, MSB first, because a higher bit always dominates every
    // lower one and only an MSB-first greedy walk maximizes the result.
    private const int MostSignificantBitIndex = 31;

    // Marks the root in LeetCode's parent-array encoding.
    private const int NoParent = -1;

    // The textbook answer: for each query, walk the parent pointers from the queried
    // node all the way to the root and XOR against every id on the way. Deliberately
    // written without this repo's primitives - plain arrays and Math.Max - because it
    // is the arm the composed solution below has to justify itself against. O(depth)
    // per query, which a chain-shaped tree turns into O(n) per query.
    public static int[] MaxGeneticDifferenceByAncestorWalk(int[] parents, int[][] queries)
    {
        var answers = new int[queries.Length];

        for (var i = 0; i < queries.Length; i++)
        {
            answers[i] = BestXorAgainstAncestors(parents, queries[i][0], queries[i][1]);
        }

        return answers;
    }

    private static int BestXorAgainstAncestors(int[] parents, int node, int value)
    {
        var best = 0;

        for (var current = node; current != NoParent; current = parents[current])
        {
            best = Math.Max(best, current ^ value);
        }

        return best;
    }

    // Composed: one offline depth-first pass over the ParentArrayTree.Build tree.
    // Each node's id is inserted into this repo's own BitTrie on the way down and
    // undone on the way back up, so while a node is being visited the trie holds
    // exactly its ancestor set - every query parked on that node is then answered by
    // one O(32) greedy max-XOR walk instead of an O(depth) climb.
    //
    // BitTrie has no Remove, so "undo" is the subtree-count technique
    // CountPairsWithXorInARange uses: a HashMap<BitTrieNode, int> counts how many
    // currently-live values pass through each trie node, bumped +1 on the way down
    // and -1 on the way back up, and the greedy walk only prefers the opposite-bit
    // child while that count is still greater than zero. The structural Zero/One
    // links persist across a removal; only the counts say what is in scope.
    public static int[] MaxGeneticDifferenceByBitTrieDfs(int[] parents, int[][] queries)
    {
        var nodes = ParentArrayTree.Build(parents);

        return MaxGeneticDifferenceByBitTrieDfs(nodes[RootIndexOf(parents)], parents.Length, queries);
    }

    public static int[] MaxGeneticDifferenceByBitTrieDfs(RootedTreeNode root, int nodeCount, int[][] queries)
    {
        var answers = new int[queries.Length];

        Visit(root, new AncestorWalk(
            GroupQueriesByNode(queries, nodeCount), new BitTrie(), new HashMap<BitTrieNode, int>(), answers));

        return answers;
    }

    // LeetCode guarantees exactly one entry equal to -1; index 0 is the fallback so a
    // malformed array still walks a real node rather than indexing out of range.
    private static int RootIndexOf(int[] parents)
    {
        for (var i = 0; i < parents.Length; i++)
        {
            if (parents[i] < 0)
            {
                return i;
            }
        }

        return 0;
    }

    private static List<(int QueryIndex, int Value)>[] GroupQueriesByNode(int[][] queries, int nodeCount)
    {
        var queriesByNode = new List<(int QueryIndex, int Value)>[nodeCount];

        for (var i = 0; i < nodeCount; i++)
        {
            queriesByNode[i] = [];
        }

        for (var i = 0; i < queries.Length; i++)
        {
            queriesByNode[queries[i][0]].Add((i, queries[i][1]));
        }

        return queriesByNode;
    }

    // The DFS's own query index, live-XOR state and output buffer - every field here
    // is the same reference at every level of the recursion; only the node (Visit's
    // other parameter) changes between calls.
    private readonly record struct AncestorWalk(
        List<(int QueryIndex, int Value)>[] QueriesByNode,
        BitTrie Trie,
        HashMap<BitTrieNode, int> SubtreeCount,
        int[] Answers);

    private static void Visit(RootedTreeNode node, AncestorWalk walk)
    {
        Insert(walk.Trie, walk.SubtreeCount, node.Id);

        foreach (var (queryIndex, value) in walk.QueriesByNode[node.Id])
        {
            walk.Answers[queryIndex] = MaxXorAmongLive(walk.Trie.Root, walk.SubtreeCount, value);
        }

        foreach (var child in node.Children)
        {
            Visit(child, walk);
        }

        Remove(walk.Trie, walk.SubtreeCount, node.Id);
    }

    private static void Insert(BitTrie trie, HashMap<BitTrieNode, int> subtreeCount, int value)
    {
        trie.Insert(value);
        AdjustSubtreeCounts(trie.Root, subtreeCount, value, delta: 1);
    }

    private static void Remove(BitTrie trie, HashMap<BitTrieNode, int> subtreeCount, int value)
        => AdjustSubtreeCounts(trie.Root, subtreeCount, value, delta: -1);

    // Shared walk Insert/Remove both do: follow the value's bit path from the root,
    // bumping each visited trie node's live count by delta (+1 to insert, -1 to undo).
    // The path always runs the full 32 levels - a matching Insert laid every node on
    // it down first - so stopping early is the compiler's null check being answered,
    // not a case that arises.
    private static void AdjustSubtreeCounts(
        BitTrieNode root, HashMap<BitTrieNode, int> subtreeCount, int value, int delta)
    {
        var current = root;
        var bits = unchecked((uint)value);

        for (var i = MostSignificantBitIndex; i >= 0; i--)
        {
            var child = ChildForBit(current, (bits >> i) & 1u);

            if (child is null)
            {
                return;
            }

            subtreeCount.TryGetValue(child, out var existing);
            subtreeCount.Set(child, existing + delta);
            current = child;
        }
    }

    // The child a value's bit at this level selects, absent until some Insert has
    // created it.
    private static BitTrieNode? ChildForBit(BitTrieNode node, uint bit) =>
        bit == 0 ? node.Zero : node.One;

    // The same greedy "prefer the opposite bit" walk BitTrie.TryMaxXor performs,
    // except a child is only eligible while it still has at least one live value
    // passing through it. Falling back to the same-bit child always finds a node, by
    // BitTrie.Walk's own precondition note - every node reached here has at least one
    // non-null child - so the loop runs all 32 levels and the null test only answers
    // the compiler.
    private static int MaxXorAmongLive(BitTrieNode root, HashMap<BitTrieNode, int> subtreeCount, int value)
    {
        BitTrieNode? current = root;
        var bits = unchecked((uint)value);
        var xor = 0u;

        for (var i = MostSignificantBitIndex; i >= 0 && current is not null; i--)
        {
            var (next, matchedOpposite) = DescendToLiveChild(current, subtreeCount, (bits >> i) & 1u);

            if (matchedOpposite)
            {
                xor |= 1u << i;
            }

            current = next;
        }

        return unchecked((int)xor);
    }

    // BitTrie.DescendOneLevel with a liveness test bolted on: prefer the opposite-bit
    // child while it still carries a live value, and report whether that preference
    // was satisfied so the caller can set the corresponding result bit.
    private static (BitTrieNode? Next, bool MatchedOpposite) DescendToLiveChild(
        BitTrieNode current, HashMap<BitTrieNode, int> subtreeCount, uint bit)
    {
        var opposite = ChildForBit(current, bit == 0 ? 1u : 0u);

        if (HasLiveValues(opposite, subtreeCount))
        {
            return (opposite, true);
        }

        return (ChildForBit(current, bit), false);
    }

    // A child is in scope for the node currently being visited only while at least
    // one not-yet-removed value still passes through it - BitTrie has no delete, so
    // its structural links outlive the values that created them.
    private static bool HasLiveValues(BitTrieNode? child, HashMap<BitTrieNode, int> subtreeCount) =>
        child is not null && subtreeCount.TryGetValue(child, out var count) && count > 0;
}
