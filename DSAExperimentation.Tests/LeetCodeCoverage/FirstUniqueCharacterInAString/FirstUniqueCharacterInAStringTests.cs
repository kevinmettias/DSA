using DSAExperimentation.LeetCode.FirstUniqueCharacterInAString;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FirstUniqueCharacterInAString;

// Harness only. Both strategies live in FirstUniqueCharacterInAStringSolution and
// are asserted against the same examples.
public sealed class FirstUniqueCharacterInAStringTests
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
    public void FirstUniqCharByBruteForce_LeetCodeExamples_ReturnsExpectedIndex(string s, int expected) =>
        Assert.Equal(expected, FirstUniqueCharacterInAStringSolution.FirstUniqCharByBruteForce(s));

    [Theory]
    [MemberData(nameof(Examples))]
    public void FirstUniqCharByHashMapTwoPass_LeetCodeExamples_ReturnsExpectedIndex(string s, int expected) =>
        Assert.Equal(expected, FirstUniqueCharacterInAStringSolution.FirstUniqCharByHashMapTwoPass(s));
}
