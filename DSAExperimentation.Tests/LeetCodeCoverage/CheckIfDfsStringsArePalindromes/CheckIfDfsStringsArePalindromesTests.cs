using DSAExperimentation.LeetCode.CheckIfDfsStringsArePalindromes;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CheckIfDfsStringsArePalindromes;

// Harness only: the tree is DataStructures' own ParentArrayTree/RootedTreeNode and
// both strategies are CheckIfDfsStringsArePalindromesSolution's - this file just
// pins them to LeetCode's published examples (TwoSumTests precedent).
public sealed partial class CheckIfDfsStringsArePalindromesTests
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
    public void GetPalindromeFlagsByBruteForce_LeetCodeExamples_ReturnsPerNodePalindromeFlags(
        int[] parent, string nodeCharacters, bool[] expected)
    {
        var flags = CheckIfDfsStringsArePalindromesSolution.GetPalindromeFlagsByBruteForce(parent, nodeCharacters);

        Assert.Equal(expected, flags);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void GetPalindromeFlagsByEulerTourRollingHash_LeetCodeExamples_ReturnsPerNodePalindromeFlags(
        int[] parent, string nodeCharacters, bool[] expected)
    {
        var flags =
            CheckIfDfsStringsArePalindromesSolution.GetPalindromeFlagsByEulerTourRollingHash(parent, nodeCharacters);

        Assert.Equal(expected, flags);
    }
}
