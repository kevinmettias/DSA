using DSAExperimentation.LeetCode.DecodeWays;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DecodeWays;

// Harness only. Both strategies live in DecodeWaysSolution and are asserted
// against the same examples, including the leading- and embedded-zero cases that
// end a decoding branch outright.
public sealed partial class DecodeWaysTests
{
    public static TheoryData<string, int> Examples =>
        new()
        {
            { "12", 2 },
            { "226", 3 },
            { "06", 0 },
            { "0", 0 },
            { "10", 1 },
            { "100", 0 },
            { "27", 1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountDecodingsByTabulation_LeetCodeExamples_ReturnsDecodingCount(string digits, int expected) =>
        Assert.Equal(expected, DecodeWaysSolution.CountDecodingsByTabulation(digits));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountDecodingsByMemoization_LeetCodeExamples_ReturnsDecodingCount(string digits, int expected) =>
        Assert.Equal(expected, DecodeWaysSolution.CountDecodingsByMemoization(digits));
}
