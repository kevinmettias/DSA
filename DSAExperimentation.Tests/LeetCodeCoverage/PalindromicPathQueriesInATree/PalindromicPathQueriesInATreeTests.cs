using DSAExperimentation.LeetCode.PalindromicPathQueriesInATree;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PalindromicPathQueriesInATree;

// Harness only. The tree itself is DataStructures' own ParentArrayTree/RootedTreeNode
// and both query strategies are PalindromicPathQueriesInATreeSolution's - this file
// just pins them to two small hand-built trees, including a query that has to fail
// (two odd-count letters on the path) alongside ones that succeed.
public sealed class PalindromicPathQueriesInATreeTests
{
    // Tree: 0(a) -> 1(a), 2(b); 1(a) -> 3(a), 4(b).
    private static readonly int[] FiveNodeParent = [-1, 0, 0, 1, 1];
    private const string FiveNodeLabels = "aabab";

    // Tree: 0(a) -> 1(a), 2(b).
    private static readonly int[] ThreeNodeParent = [-1, 0, 0];
    private const string ThreeNodeLabels = "aab";

    public static TheoryData<int[], string, int[][], bool[]> Examples =>
        new()
        {
            {
                FiveNodeParent, FiveNodeLabels,
                [[3, 4], [3, 2], [2, 4], [0, 0]],
                [true, false, true, true]
            },
            {
                ThreeNodeParent, ThreeNodeLabels,
                [[1, 2], [1, 1]],
                [true, true]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void GetPalindromePathFlagsByAncestorWalk_LeetCodeExamples_ReturnsWhetherEachPathReorders(
        int[] parent, string nodeCharacters, int[][] queries, bool[] expected)
    {
        var actual = PalindromicPathQueriesInATreeSolution.GetPalindromePathFlagsByAncestorWalk(
            parent, nodeCharacters, queries);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void GetPalindromePathFlagsByLcaBitmask_LeetCodeExamples_ReturnsWhetherEachPathReorders(
        int[] parent, string nodeCharacters, int[][] queries, bool[] expected)
    {
        var actual = PalindromicPathQueriesInATreeSolution.GetPalindromePathFlagsByLcaBitmask(
            parent, nodeCharacters, queries);

        Assert.Equal(expected, actual);
    }
}
