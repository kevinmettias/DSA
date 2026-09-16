using DSAExperimentation.LeetCode.SmallestPalindromicRearrangementI;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SmallestPalindromicRearrangementI;

// Harness only. Both strategies are SmallestPalindromicRearrangementISolution's -
// this file just pins them to LeetCode's published examples.
public sealed partial class SmallestPalindromicRearrangementITests
{
    public static TheoryData<RearrangementExample> Examples =>
        new()
        {
            new RearrangementExample(Input: "z", Expected: "z"),
            new RearrangementExample(Input: "babab", Expected: "abbba"),
            new RearrangementExample(Input: "daccad", Expected: "acddca"),
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void RearrangeByCharArrayReverse_LeetCodeExamples_ReturnsSmallestPalindrome(
        RearrangementExample example) =>
        Assert.Equal(example.Expected, SmallestPalindromicRearrangementISolution.RearrangeByCharArrayReverse(example.Input));

    [Theory]
    [MemberData(nameof(Examples))]
    public void RearrangeByCharStack_LeetCodeExamples_ReturnsSmallestPalindrome(
        RearrangementExample example) =>
        Assert.Equal(example.Expected, SmallestPalindromicRearrangementISolution.RearrangeByCharStack(example.Input));

    // One LeetCode example: the letter multiset and the smallest palindrome it
    // rearranges to. Both are strings in adjacent positions at the call site, so the
    // bundle names the input and the expectation rather than leaving them swappable.
    public readonly record struct RearrangementExample(string Input, string Expected);
}
