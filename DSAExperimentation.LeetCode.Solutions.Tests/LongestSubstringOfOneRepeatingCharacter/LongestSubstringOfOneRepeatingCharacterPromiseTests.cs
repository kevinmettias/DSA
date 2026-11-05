using DSAExperimentation.LeetCode.Solutions.LongestSubstringOfOneRepeatingCharacter;

namespace DSAExperimentation.LeetCode.Solutions.Tests.LongestSubstringOfOneRepeatingCharacter;

public sealed partial class LongestSubstringOfOneRepeatingCharacterPromiseTests
{
    public static TheoryData<string, string, int[], bool> Inputs =>
        new()
        {
            { "babacc", "bcb", [1, 3, 3], true },
            { "abyzz", "aa", [2, 1], true },
            { "zzzaaabbbccc", "aaabbbccc", [2, 3, 4, 5, 6, 7, 8], false },
            { "abc", "a", [3], false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenEachCharacterHasAnIndexInsideTheString(string text, string characters, int[] indices, bool expected)
        => Assert.Equal(expected, LongestSubstringOfOneRepeatingCharacterPromise.IsKeptBy(new(text), new(characters), indices));
}
