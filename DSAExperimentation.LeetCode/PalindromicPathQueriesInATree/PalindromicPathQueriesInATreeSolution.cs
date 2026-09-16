using System.Numerics;
using DSAExperimentation.Algorithms.Ancestry;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using MaskStack = DSAExperimentation.DataStructures.Stack.Stack<(
    DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.RootedTreeNode Node, int Mask)>;

namespace DSAExperimentation.LeetCode.PalindromicPathQueriesInATree;

// A tree rooted at node 0, given by LeetCode's own parent-array encoding
// (parent[0] = -1), with a lowercase letter nodeCharacters[i] written on every
// node i. For each query [u, v], the characters along the path from u to v
// (inclusive of both endpoints) may be freely reordered; report whether some
// reordering forms a palindrome - same as any anagram-of-a-palindrome check,
// that holds exactly when at most one letter occurs an odd number of times on
// the path.
internal static class PalindromicPathQueriesInATreeSolution
{
    private const int AlphabetSize = 26;
    private const int AtMostOneOddCount = 1;

    // The textbook answer: every query re-derives its own LCA and letter counts
    // straight from the raw parent[]/nodeCharacters[] arrays - nothing shared or
    // precomputed across queries, and no repo primitives - the arm the bitmask
    // strategy below has to justify itself against.
    public static bool[] GetPalindromePathFlagsByAncestorWalk(
        int[] parent, string nodeCharacters, int[][] queries)
    {
        var answers = new bool[queries.Length];

        for (var i = 0; i < queries.Length; i++)
        {
            answers[i] = IsPathReorderablePalindrome(
                parent, nodeCharacters, queries[i][0], queries[i][1]);
        }

        return answers;
    }

    private static bool IsPathReorderablePalindrome(
        int[] parent, string nodeCharacters, int startNode, int endNode)
    {
        var lca = FindLowestCommonAncestor(parent, startNode, endNode);
        var counts = new int[AlphabetSize];

        for (var node = startNode; node != lca; node = parent[node])
        {
            counts[nodeCharacters[node] - 'a']++;
        }

        for (var node = endNode; node != lca; node = parent[node])
        {
            counts[nodeCharacters[node] - 'a']++;
        }

        counts[nodeCharacters[lca] - 'a']++;

        return CountOddOccurrences(counts) <= AtMostOneOddCount;
    }

    private static int FindLowestCommonAncestor(int[] parent, int firstNode, int secondNode)
    {
        var ancestorsOfFirstNode = new HashSet<int>();

        for (var node = firstNode; node != -1; node = parent[node])
        {
            ancestorsOfFirstNode.Add(node);
        }

        var lca = secondNode;

        while (!ancestorsOfFirstNode.Contains(lca))
        {
            lca = parent[lca];
        }

        return lca;
    }

    private static int CountOddOccurrences(int[] counts)
    {
        var odd = 0;

        foreach (var count in counts)
        {
            if (count % 2 != 0)
            {
                odd++;
            }
        }

        return odd;
    }

    // Composed: one pass from the root builds every node's root-to-node letter
    // parity mask - bit c toggled each time letter c is crossed, so
    // Popcount(mask) is exactly the number of letters with an odd count on that
    // root path (the same "XOR cancels a value crossed an even number of times"
    // trick KthSmallestPathXORSumSolution uses for path XOR sums, just with a
    // 26-bit letter mask standing in for vals[i]). A query's path mask is then
    // mask[startNode] XOR mask[endNode] XOR bit(nodeCharacters[lca]): the lca's
    // own bit sits in BOTH root-to-startNode and root-to-endNode masks so it
    // cancels out entirely, then gets XORed back in once for the single time it
    // actually sits on the startNode-endNode path.
    // LowestCommonAncestor.Find (Algorithms/Ancestry) supplies the lca itself,
    // reused as-is over RootedTreeTopology rather than hand-rolling a second
    // ancestor search.
    public static bool[] GetPalindromePathFlagsByLcaBitmask(
        int[] parent, string nodeCharacters, int[][] queries)
    {
        var nodes = ParentArrayTree.Build(parent);

        return GetPalindromePathFlagsByLcaBitmask(nodes, nodeCharacters, queries);
    }

    public static bool[] GetPalindromePathFlagsByLcaBitmask(
        RootedTreeNode[] nodes, string nodeCharacters, int[][] queries)
    {
        var mask = ComputeRootToNodeMask(nodes, nodeCharacters);
        var answers = new bool[queries.Length];

        for (var i = 0; i < queries.Length; i++)
        {
            var (startNode, endNode) = (queries[i][0], queries[i][1]);
            var lca = LowestCommonAncestor.Find<
                RootedTreeNode, RootedTreeTopology, ListChildren<RootedTreeNode>,
                NaturalChildOrder<RootedTreeNode, ListChildren<RootedTreeNode>>, ListChildren<RootedTreeNode>>(
                nodes[0], nodes[startNode], nodes[endNode])!;

            var pathMask = mask[startNode] ^ mask[endNode] ^ LetterBit(nodeCharacters[lca.Id]);
            answers[i] = BitOperations.PopCount((uint)pathMask) <= AtMostOneOddCount;
        }

        return answers;
    }

    private static int[] ComputeRootToNodeMask(RootedTreeNode[] nodes, string nodeCharacters)
    {
        var mask = new int[nodes.Length];
        var stack = new MaskStack();
        stack.Push((nodes[0], LetterBit(nodeCharacters[0])));

        while (stack.TryPop(out var frame))
        {
            mask[frame.Node.Id] = frame.Mask;

            foreach (var child in frame.Node.Children)
            {
                stack.Push((child, frame.Mask ^ LetterBit(nodeCharacters[child.Id])));
            }
        }

        return mask;
    }

    private static int LetterBit(char letter) => 1 << (letter - 'a');
}
