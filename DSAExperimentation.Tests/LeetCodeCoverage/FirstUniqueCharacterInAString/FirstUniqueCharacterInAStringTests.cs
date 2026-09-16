using DSAExperimentation.LeetCode.FirstUniqueCharacterInAString;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FirstUniqueCharacterInAString;

// Harness only. Both strategies live in FirstUniqueCharacterInAStringSolution and
// are asserted against the same examples.
public sealed partial class FirstUniqueCharacterInAStringTests
{
    public static TheoryData<string, int> Examples =>
        new()
        {
            { "leetcode", 0 },
            { "loveleetcode", 2 },
            { "aabb", -1 },
            { "z", 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FirstUniqCharByBruteForce_LeetCodeExamples_ReturnsExpectedIndex(string text, int expected) =>
        Assert.Equal(expected, FirstUniqueCharacterInAStringSolution.FirstUniqCharByBruteForce(text));

    [Theory]
    [MemberData(nameof(Examples))]
    public void FirstUniqCharByHashMapTwoPass_LeetCodeExamples_ReturnsExpectedIndex(string text, int expected) =>
        Assert.Equal(expected, FirstUniqueCharacterInAStringSolution.FirstUniqCharByHashMapTwoPass(text));
}
