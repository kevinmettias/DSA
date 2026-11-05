using DSAExperimentation.LeetCode.Solutions.LongestPathWithDifferentAdjacentCharacters;

namespace DSAExperimentation.LeetCode.Solutions.Tests.LongestPathWithDifferentAdjacentCharacters;

// Harness only. The tree is DataStructures' ParentArrayTree/RootedTreeNode and both
// strategies are LongestPathWithDifferentAdjacentCharactersSolution's - this file
// just pins them to the same cases, which is what proves the O(n) fold agrees
// with the O(n^2) recomputing walk the benchmark measures it against.
public sealed partial class LongestPathWithDifferentAdjacentCharactersSolutionTests
{
    public static TheoryData<int[], string, int> Cases =>
        new()
        {
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
    [MemberData(nameof(Cases))]
    public void LongestPathByRecomputedSubtreeWalk_Cases_ReturnsLongestDifferentLabelPathNodeCount(
        int[] parent, string labels, int expected)
    {
        var actual = LongestPathWithDifferentAdjacentCharactersSolution.LongestPathByRecomputedSubtreeWalk(parent, labels);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void LongestPathByTreeFold_Cases_ReturnsLongestDifferentLabelPathNodeCount(
        int[] parent, string labels, int expected)
    {
        var actual = LongestPathWithDifferentAdjacentCharactersSolution.LongestPathByTreeFold(parent, labels);

        Assert.Equal(expected, actual);
    }
}
