using DSAExperimentation.LeetCode.LongestPathWithDifferentAdjacentCharacters;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LongestPathWithDifferentAdjacentCharacters;

// Harness only. The tree is DataStructures' ParentArrayTree/RootedTreeNode and both
// strategies are LongestPathWithDifferentAdjacentCharactersSolution's - this file
// just pins them to the same examples, which is what proves the O(n) fold agrees
// with the O(n^2) recomputing walk the benchmark measures it against.
public sealed class LongestPathWithDifferentAdjacentCharactersTests
{
    public static TheoryData<int[], string, int> Examples =>
        new()
        {
            // LeetCode example 1: the 0-2 edge is blocked ('a'-'a') and so is 1-4
            // ('b'-'b'), leaving 3-1-0 as the longest valid path at three nodes.
            { [-1, 0, 0, 1, 1, 2], "abacbe", 3 },

            // LeetCode example 2: every child of the root differs from it except
            // node 1, so two branches join through the root.
            { [-1, 0, 0, 0], "aabc", 3 },

            // A single node is a path of one, with no edge to validate.
            { [-1], "a", 1 },

            // A chain whose every edge is blocked collapses to one node.
            { [-1, 0, 1, 2], "aaaa", 1 },

            // A chain whose every edge is valid is the whole chain.
            { [-1, 0, 1, 2], "abab", 4 },

            // The best path is entirely inside a subtree (2-1-3) and reaches the
            // root only as an already-counted child result, since the 0-1 edge is
            // blocked - the branch that carries a child's own longest path upward.
            { [-1, 0, 1, 1], "aabc", 3 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void LongestPathByRecomputedSubtreeWalk_LeetCodeExamples_ReturnsLongestDifferentLabelPathNodeCount(
        int[] parent, string s, int expected)
    {
        var actual = LongestPathWithDifferentAdjacentCharactersSolution.LongestPathByRecomputedSubtreeWalk(parent, s);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void LongestPathByTreeFold_LeetCodeExamples_ReturnsLongestDifferentLabelPathNodeCount(
        int[] parent, string s, int expected)
    {
        var actual = LongestPathWithDifferentAdjacentCharactersSolution.LongestPathByTreeFold(parent, s);

        Assert.Equal(expected, actual);
    }
}
