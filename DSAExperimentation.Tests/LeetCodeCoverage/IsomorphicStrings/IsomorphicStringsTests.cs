using DSAExperimentation.LeetCode.IsomorphicStrings;

namespace DSAExperimentation.Tests.LeetCodeCoverage.IsomorphicStrings;

// Harness only: both strategies live in IsomorphicStringsSolution. One test
// method per strategy over one shared set of LeetCode's own examples, plus a
// couple that exercise both mapping directions, so a failure names the
// strategy (and direction) that broke.
public sealed class IsomorphicStringsTests
{
    public static TheoryData<string, string, bool> Examples =>
        new()
        {
            { "egg", "add", true },
            { "foo", "bar", false },
            { "paper", "title", true },
            { "badc", "baba", false },
            { "ab", "aa", false },
            { "", "", true },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsIsomorphicByDictionary_LeetCodeExamples_ReturnsExpected(string s, string t, bool expected) =>
        Assert.Equal(expected, IsomorphicStringsSolution.IsIsomorphicByDictionary(s, t));

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsIsomorphicByHashMap_LeetCodeExamples_ReturnsExpected(string s, string t, bool expected) =>
        Assert.Equal(expected, IsomorphicStringsSolution.IsIsomorphicByHashMap(s, t));
}
