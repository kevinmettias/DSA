using DSAExperimentation.LeetCode.CheckIfDfsStringsArePalindromes;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CheckIfDfsStringsArePalindromes;

// Harness only: the tree is DataStructures' own ParentArrayTree/RootedTreeNode and
// both strategies are CheckIfDfsStringsArePalindromesSolution's - this file just
// pins them to LeetCode's published examples (TwoSumTests precedent).
public sealed class CheckIfDfsStringsArePalindromesTests
{
    public static TheoryData<int[], string, bool[]> Examples =>
        new()
        {
            { [-1, 0, 0, 1, 1, 2], "aababa", [true, true, false, true, true, true] },
            { [-1, 0, 0, 0, 0], "abcbc", [false, true, true, true, true] },
            { [-1], "a", [true] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsPalindromeByBruteForce_LeetCodeExamples_ReturnsPerNodePalindromeFlags(
        int[] parent, string s, bool[] expected) =>
        Assert.Equal(expected, CheckIfDfsStringsArePalindromesSolution.IsPalindromeByBruteForce(parent, s));

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsPalindromeByEulerTourRollingHash_LeetCodeExamples_ReturnsPerNodePalindromeFlags(
        int[] parent, string s, bool[] expected) =>
        Assert.Equal(expected, CheckIfDfsStringsArePalindromesSolution.IsPalindromeByEulerTourRollingHash(parent, s));
}
