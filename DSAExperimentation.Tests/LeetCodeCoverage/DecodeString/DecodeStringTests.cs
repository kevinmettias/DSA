using DSAExperimentation.LeetCode.DecodeString;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DecodeString;

// Harness only. Both decoding strategies are DecodeStringSolution's - this file
// just pins them to LeetCode's published examples.
public sealed partial class DecodeStringTests
{
    public static TheoryData<DecodeCase> Examples =>
        new()
        {
            { new DecodeCase(Encoded: "3[a]2[bc]", Expected: "aaabcbc") },
            { new DecodeCase(Encoded: "3[a2[c]]", Expected: "accaccacc") },
            { new DecodeCase(Encoded: "2[abc]3[cd]ef", Expected: "abcabccdcdcdef") },
            { new DecodeCase(Encoded: "abc", Expected: "abc") },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void DecodeByRecursiveDescent_LeetCodeExamples_ReturnsExpandedString(DecodeCase example) =>
        Assert.Equal(example.Expected, DecodeStringSolution.DecodeByRecursiveDescent(example.Encoded));

    [Theory]
    [MemberData(nameof(Examples))]
    public void DecodeByStackScan_LeetCodeExamples_ReturnsExpandedString(DecodeCase example) =>
        Assert.Equal(example.Expected, DecodeStringSolution.DecodeByStackScan(example.Encoded));
}
