using DSAExperimentation.DataStructures.CountedBitTrie;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

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
    // Each node's id is inserted into this repo's own CountedBitTrie on the way down
    // and removed again on the way back up, so while a node is being visited the trie
    // holds exactly its ancestor set - every query parked on that node is then
    // answered by one O(32) greedy max-XOR walk over the live values instead of an
    // O(depth) climb.
    public static int[] MaxGeneticDifferenceByBitTrieDfs(int[] parents, int[][] queries)
    {
        var nodes = ParentArrayTree.Build(parents);

        return MaxGeneticDifferenceByBitTrieDfs(nodes[RootIndexOf(parents)], parents.Length, queries);
    }

    public static int[] MaxGeneticDifferenceByBitTrieDfs(RootedTreeNode root, int nodeCount, int[][] queries)
    {
        var answers = new int[queries.Length];

        Visit(root, new AncestorWalk(GroupQueriesByNode(queries, nodeCount), new CountedBitTrie(), answers));

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

    // The DFS's own query index, ancestor trie and output buffer - every field here
    // is the same reference at every level of the recursion; only the node (Visit's
    // other parameter) changes between calls.
    private readonly record struct AncestorWalk(
        List<(int QueryIndex, int Value)>[] QueriesByNode,
        CountedBitTrie Ancestors,
        int[] Answers);

    // The ancestor set always holds node itself while its queries are answered, so
    // TryMaxXor always finds a live value and its result is the answer.
    private static void Visit(RootedTreeNode node, AncestorWalk walk)
    {
        walk.Ancestors.Insert(node.Id);

        foreach (var (queryIndex, value) in walk.QueriesByNode[node.Id])
        {
            walk.Ancestors.TryMaxXor(value, out walk.Answers[queryIndex]);
        }

        foreach (var child in node.Children)
        {
            Visit(child, walk);
        }

        walk.Ancestors.TryRemove(node.Id);
    }
}
