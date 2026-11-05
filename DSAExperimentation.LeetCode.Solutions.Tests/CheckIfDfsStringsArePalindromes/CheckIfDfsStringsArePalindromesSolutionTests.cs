using DSAExperimentation.LeetCode.Solutions.CheckIfDfsStringsArePalindromes;

namespace DSAExperimentation.LeetCode.Solutions.Tests.CheckIfDfsStringsArePalindromes;

// Harness only: the tree is DataStructures' own ParentArrayTree/RootedTreeNode and
// both strategies are CheckIfDfsStringsArePalindromesSolution's - this file just
// pins them to shapes the catalog lacks (TwoSumTests precedent): a root with four
// leaf children, whose own string is not a palindrome, and a lone root. LeetCode's
// published examples run in CatalogCaseTests.
public sealed partial class CheckIfDfsStringsArePalindromesSolutionTests
{
    public static TheoryData<int[], string, bool[]> Cases =>
        new()
        {
            { [-1, 0, 0, 0, 0], "abcbc", [false, true, true, true, true] },
            { [-1], "a", [true] },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void GetPalindromeFlagsByBruteForce_Cases_ReturnsPerNodePalindromeFlags(
        int[] parent, string nodeCharacters, bool[] expected)
    {
        var flags = CheckIfDfsStringsArePalindromesSolution.GetPalindromeFlagsByBruteForce(parent, nodeCharacters);

        Assert.Equal(expected, flags);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void GetPalindromeFlagsByEulerTourRollingHash_Cases_ReturnsPerNodePalindromeFlags(
        int[] parent, string nodeCharacters, bool[] expected)
    {
        var flags =
            CheckIfDfsStringsArePalindromesSolution.GetPalindromeFlagsByEulerTourRollingHash(parent, nodeCharacters);

        Assert.Equal(expected, flags);
    }
}
