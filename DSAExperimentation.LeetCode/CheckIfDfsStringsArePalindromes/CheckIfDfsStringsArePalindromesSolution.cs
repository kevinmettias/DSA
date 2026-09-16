using System.Text;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.RollingHash;

namespace DSAExperimentation.LeetCode.CheckIfDfsStringsArePalindromes;

// LeetCode 3327. Check if DFS Strings Are Palindromes: parent[] describes a tree
// rooted at 0 (DataStructures' own parent-array encoding), and dfs(x) visits x's
// children in increasing order before appending nodeCharacters[x] - exactly the post-order walk
// ParentArrayTree.Build already sets up, since it appends child i to its parent's
// list in increasing index order as it scans the array once (CountWaysToBuild
// RoomsInAnAntColony precedent for reusing this Domain type as-is).
//
// The key structural fact both strategies share: dfs(0)'s string contains dfs(i)'s
// string as one CONTIGUOUS run for every node i, because a subtree's own walk
// always finishes before its next sibling starts. The brute-force arm ignores that
// and rebuilds dfs(i) from scratch per node - O(subtree size) each, O(n^2) worst
// case on a deep tree. The composed arm walks the whole tree exactly ONCE to
// record every node's [start, end) run inside that single global string, then
// answers each node's palindrome query in O(1) with DataStructures.RollingHash:
// the forward hash of the run against the same run read off a second RollingHash
// built over the reversed string - RollingHashSearch's own "screen with an O(1)
// hash comparison" use of the primitive, aimed at a palindrome check instead of a
// pattern match.
internal static class CheckIfDfsStringsArePalindromesSolution
{
    // Textbook baseline: dfs(i) rebuilt from scratch for every node, checked with a
    // plain two-pointer scan - the O(n^2) arm the single-tour RollingHash strategy
    // has to beat.
    public static bool[] GetPalindromeFlagsByBruteForce(int[] parent, string nodeCharacters)
    {
        var nodes = ParentArrayTree.Build(parent);

        return GetPalindromeFlagsByBruteForce(nodes, nodeCharacters);
    }

    public static bool[] GetPalindromeFlagsByBruteForce(RootedTreeNode[] nodes, string nodeCharacters)
    {
        var answer = new bool[nodes.Length];

        for (var i = 0; i < nodes.Length; i++)
        {
            var builder = new StringBuilder();
            AppendDfsString(nodes[i], nodeCharacters, builder);
            answer[i] = IsPalindrome(builder);
        }

        return answer;
    }

    private static void AppendDfsString(RootedTreeNode node, string nodeCharacters, StringBuilder builder)
    {
        foreach (var child in node.Children)
        {
            AppendDfsString(child, nodeCharacters, builder);
        }

        builder.Append(nodeCharacters[node.Id]);
    }

    private static bool IsPalindrome(StringBuilder builder)
    {
        var left = 0;
        var right = builder.Length - 1;

        while (left < right)
        {
            if (builder[left] != builder[right])
            {
                return false;
            }

            left++;
            right--;
        }

        return true;
    }

    // Composed: one global post-order walk from the root builds dfs(0) exactly
    // once, recording every node's [start, end) run inside it; RollingHash over
    // that string and RollingHash over its reverse then answer every node's
    // palindrome query in O(1) each.
    public static bool[] GetPalindromeFlagsByEulerTourRollingHash(int[] parent, string nodeCharacters)
    {
        var nodes = ParentArrayTree.Build(parent);

        return GetPalindromeFlagsByEulerTourRollingHash(nodes, nodeCharacters);
    }

    public static bool[] GetPalindromeFlagsByEulerTourRollingHash(RootedTreeNode[] nodes, string nodeCharacters)
    {
        var n = nodes.Length;
        var tour = new char[n];
        var start = new int[n];
        var end = new int[n];
        var position = 0;

        BuildTour(nodes[0], nodeCharacters, (Text: tour, Start: start, End: end), ref position);

        var forward = new RollingHash(tour);
        var backward = new RollingHash(ReverseOf(tour));
        var answer = new bool[n];

        for (var i = 0; i < n; i++)
        {
            var length = end[i] - start[i];
            var forwardHash = forward.Hash(start[i], length);
            var backwardHash = backward.Hash(n - end[i], length);
            answer[i] = forwardHash == backwardHash;
        }

        return answer;
    }

    // Records node.Id's run as [begin, position) once every child has appended its
    // own run ahead of it - begin is captured before recursing, so it always equals
    // the write cursor's position at entry, exactly where this subtree's first
    // character (some descendant's, or its own if it's a leaf) will land.
    //
    // The three arrays this walk writes are one recording, not three: the tour's own
    // characters, and each node's [start, end) run inside them. They are allocated
    // together, filled together, and read together by the queries that follow.
    private static void BuildTour(
        RootedTreeNode node, string nodeCharacters, (char[] Text, int[] Start, int[] End) tour, ref int position)
    {
        var begin = position;

        AppendSubtreeCharacters(node, nodeCharacters, tour, ref position);

        tour.Start[node.Id] = begin;
        tour.End[node.Id] = position;
    }

    // Appends every character of node's subtree to the tour in post-order - each child's
    // whole run ahead of it, node's own character last - leaving the cursor just past
    // them. The character-writing half of the walk above.
    private static void AppendSubtreeCharacters(
        RootedTreeNode node, string nodeCharacters, (char[] Text, int[] Start, int[] End) tour, ref int position)
    {
        foreach (var child in node.Children)
        {
            BuildTour(child, nodeCharacters, tour, ref position);
        }

        tour.Text[position] = nodeCharacters[node.Id];
        position++;
    }

    private static char[] ReverseOf(char[] tour)
    {
        var reversed = new char[tour.Length];

        for (var i = 0; i < tour.Length; i++)
        {
            reversed[i] = tour[tour.Length - 1 - i];
        }

        return reversed;
    }
}
