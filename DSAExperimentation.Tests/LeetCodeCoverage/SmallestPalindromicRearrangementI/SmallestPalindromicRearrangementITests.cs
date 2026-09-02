using DSAExperimentation.LeetCode.SmallestPalindromicRearrangementI;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SmallestPalindromicRearrangementI;

// Harness only. Both strategies are SmallestPalindromicRearrangementISolution's -
// this file just pins them to LeetCode's published examples.
public sealed class SmallestPalindromicRearrangementITests
{
    public static TheoryData<string, string> Examples =>
        new()
        {
            { "z", "z" },
            { "babab", "abbba" },
            { "daccad", "acddca" },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void RearrangeByCharArrayReverse_LeetCodeExamples_ReturnsSmallestPalindrome(
        string s, string expected) =>
        Assert.Equal(expected, SmallestPalindromicRearrangementISolution.RearrangeByCharArrayReverse(s));

    [Theory]
    [MemberData(nameof(Examples))]
    public void RearrangeByCharStack_LeetCodeExamples_ReturnsSmallestPalindrome(
        string s, string expected) =>
        Assert.Equal(expected, SmallestPalindromicRearrangementISolution.RearrangeByCharStack(s));
}
