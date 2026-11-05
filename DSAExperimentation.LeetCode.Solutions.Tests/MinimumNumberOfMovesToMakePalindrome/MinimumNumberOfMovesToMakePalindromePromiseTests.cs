using DSAExperimentation.LeetCode.Solutions.MinimumNumberOfMovesToMakePalindrome;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MinimumNumberOfMovesToMakePalindrome;

public sealed partial class MinimumNumberOfMovesToMakePalindromePromiseTests
{
    public static TheoryData<string, bool> Inputs =>
        new()
        {
            { "aabb", true },
            { "letelt", true },
            { "abcba", true },
            { "abca", false },
            { "aabbab", false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenAtMostOneLetterIsUnpaired(string text, bool expected)
        => Assert.Equal(expected, MinimumNumberOfMovesToMakePalindromePromise.IsKeptBy(text));
}
