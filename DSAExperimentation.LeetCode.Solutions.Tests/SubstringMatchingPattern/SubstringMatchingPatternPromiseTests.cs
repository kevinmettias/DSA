using DSAExperimentation.LeetCode.Solutions.SubstringMatchingPattern;

namespace DSAExperimentation.LeetCode.Solutions.Tests.SubstringMatchingPattern;

public sealed partial class SubstringMatchingPatternPromiseTests
{
    public static TheoryData<string, string, bool> Inputs =>
        new()
        {
            { "leetcode", "ee*e", true },
            { "car", "c*v", true },
            { "abcdefg", "a*d*g", false },
            { "abc", "abc", false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsForAPatternWithExactlyOneStar(string subject, string pattern, bool expected)
        => Assert.Equal(expected, SubstringMatchingPatternPromise.IsKeptBy(new(subject), new(pattern)));
}
