using DSAExperimentation.LeetCode.PalindromicPathQueriesInATree;

namespace DSAExperimentation.LeetCode.Tests.PalindromicPathQueriesInATree;

// Harness only. Both strategies are PalindromicPathQueriesInATreeSolution's. The first
// two rows are LeetCode's published examples. The rest are derived by hand below - each
// path's letters listed and their odd counts read off - and were checked against an
// independent brute force that searches outward from one endpoint for the other and
// counts the letters it finds, sharing nothing with either strategy.
public sealed partial class PalindromicPathQueriesInATreeSolutionTests
{
    public static TheoryData<int, int[][], string, string[], bool[]> Examples =>
        new()
        {
            {
                3, [[0, 1], [1, 2]], "aac",
                ["query 0 2", "update 1 b", "query 0 2"],
                [true, false]
            },
            {
                4, [[0, 1], [0, 2], [0, 3]], "abca",
                ["query 1 2", "update 0 b", "query 2 3", "update 3 a", "query 1 3"],
                [false, false, true]
            },

            // One node: its path is its own letter, a palindrome before and after the
            // update, and its subtree runs to the end of the tour.
            {
                1, [], "z",
                ["query 0 0", "update 0 y", "query 0 0"],
                [true, true]
            },

            // Edges 0-1, 0-2, 1-3, 1-4, 2-5, 3-6, written child-first in places; letters
            // 0:a 1:b 2:c 3:a 4:b 5:c 6:b.
            //   query 6 4: b a b b       - a and b odd        -> false
            //   query 6 5: b a b a c c   - all even           -> true
            //   update 0 b (the lca of 6 and 5)
            //   query 6 5: b a b b c c   - a and b odd        -> false
            //   update 4 a (off the 6-5 path)
            //   query 6 5: unchanged                          -> false
            //   query 4 6: a b a b       - all even, lca 1    -> true
            //   query 1 6: b a b         - only a odd, 1 is 6's ancestor -> true
            //   update 3 c
            //   query 1 6: b c b         - only c odd         -> true
            //   query 4 6: a b c b       - a and c odd        -> false
            //   query 5 5: c                                  -> true
            //   update 1 c
            //   query 2 3: c b c c       - b and c odd        -> false
            //   query 6 2: b c c b c     - only c odd         -> true
            //   query 5 2: c c           - all even; 2 is 5's ancestor and its letter
            //              differs from its parent's, so counting the parent in its
            //              place would leave b and c odd    -> true
            {
                7, [[1, 0], [3, 1], [2, 0], [4, 1], [5, 2], [6, 3]], "abcabcb",
                [
                    "query 6 4", "query 6 5", "update 0 b", "query 6 5", "update 4 a", "query 6 5",
                    "query 4 6", "query 1 6", "update 3 c", "query 1 6", "query 4 6", "query 5 5",
                    "update 1 c", "query 2 3", "query 6 2", "query 5 2",
                ],
                [false, true, false, false, true, true, true, false, true, false, true, true]
            },

            // The chain 0-1-2-3-4-5, its edges shuffled and reversed; letters a a b b c c.
            //   query 0 5: a a b b c c   - all even           -> true
            //   update 2 c
            //   query 0 5: a a c b c c   - b and c odd        -> false
            //   query 1 4: a c b c       - a and b odd        -> false
            //   update 3 a
            //   query 1 4: a c a c       - all even           -> true
            //   query 0 5: a a c a c c   - a and c odd        -> false
            //   query 2 2: c                                  -> true
            {
                6, [[5, 4], [2, 3], [1, 0], [3, 4], [2, 1]], "aabbcc",
                [
                    "query 0 5", "update 2 c", "query 0 5", "query 1 4", "update 3 a", "query 1 4",
                    "query 0 5", "query 2 2",
                ],
                [true, false, false, true, false, true]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void GetPalindromePathFlagsByAncestorWalk_Examples_ReturnsWhetherEachQueriedPathRearranges(
        int nodeCount, int[][] edges, string nodeLetters, string[] queries, bool[] expected)
    {
        var actual = PalindromicPathQueriesInATreeSolution.GetPalindromePathFlagsByAncestorWalk(
            nodeCount, edges, nodeLetters, queries);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void GetPalindromePathFlagsByEulerFenwick_Examples_ReturnsWhetherEachQueriedPathRearranges(
        int nodeCount, int[][] edges, string nodeLetters, string[] queries, bool[] expected)
    {
        var actual = PalindromicPathQueriesInATreeSolution.GetPalindromePathFlagsByEulerFenwick(
            nodeCount, edges, nodeLetters, queries);

        Assert.Equal(expected, actual);
    }
}
