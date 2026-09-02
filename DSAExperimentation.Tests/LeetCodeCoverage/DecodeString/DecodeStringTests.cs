using DSAExperimentation.LeetCode.DecodeString;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DecodeString;

// Harness only. Both decoding strategies are DecodeStringSolution's - this file
// just pins them to LeetCode's published examples.
public sealed class DecodeStringTests
{
    public static TheoryData<string, string> Examples =>
        new()
        {
            { "3[a]2[bc]", "aaabcbc" },
            { "3[a2[c]]", "accaccacc" },
            { "2[abc]3[cd]ef", "abcabccdcdcdef" },
            { "abc", "abc" },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void DecodeByRecursiveDescent_LeetCodeExamples_ReturnsExpandedString(string encoded, string expected) =>
        Assert.Equal(expected, DecodeStringSolution.DecodeByRecursiveDescent(encoded));

    [Theory]
    [MemberData(nameof(Examples))]
    public void DecodeByStackScan_LeetCodeExamples_ReturnsExpandedString(string encoded, string expected) =>
        Assert.Equal(expected, DecodeStringSolution.DecodeByStackScan(encoded));
}
