using DSAExperimentation.LeetCode.DecodeWaysII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DecodeWaysII;

// LeetCode 639. Decode Ways II: DecodeWaysIISolution widens DecodeWays' own
// single/pair recurrence so '*' stands for "any digit 1-9" at a single position
// and the matching range of valid two-digit combinations at a pair, with every
// running total reduced mod 1e9+7 as LeetCode requires.
public sealed class DecodeWaysIITests
{
    public static TheoryData<string, long> Examples => new()
    {
        { "*", 9 },
        { "1*", 18 },
        { "2*", 15 },
        { "**", 96 },
        { "*6*", 99 },
    };

    [Theory]
    [MemberData(nameof(Examples))]
    public void NumDecodingsByTabulation_LeetCodeExamples_ReturnsCount(string s, long expected)
        => Assert.Equal(expected, DecodeWaysIISolution.NumDecodingsByTabulation(s));

    [Theory]
    [MemberData(nameof(Examples))]
    public void NumDecodingsByMemoization_LeetCodeExamples_ReturnsCount(string s, long expected)
        => Assert.Equal(expected, DecodeWaysIISolution.NumDecodingsByMemoization(s));
}
