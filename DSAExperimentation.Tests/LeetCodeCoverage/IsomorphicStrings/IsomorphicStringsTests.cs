using DSAExperimentation.LeetCode.IsomorphicStrings;

namespace DSAExperimentation.Tests.LeetCodeCoverage.IsomorphicStrings;

// Harness only: both strategies live in IsomorphicStringsSolution. One test
// method per strategy over one shared set of LeetCode's own examples, plus a
// couple that exercise both mapping directions, so a failure names the
// strategy (and direction) that broke.
public sealed class IsomorphicStringsTests
{
    public static TheoryData<IsomorphismCase> Examples =>
        new()
        {
            { new IsomorphismCase(S: "egg", T: "add", Expected: true) },
            { new IsomorphismCase(S: "foo", T: "bar", Expected: false) },
            { new IsomorphismCase(S: "paper", T: "title", Expected: true) },
            { new IsomorphismCase(S: "badc", T: "baba", Expected: false) },
            { new IsomorphismCase(S: "ab", T: "aa", Expected: false) },
            { new IsomorphismCase(S: "", T: "", Expected: true) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsIsomorphicByDictionary_LeetCodeExamples_ReturnsExpected(IsomorphismCase example)
    {
        var actual = IsomorphicStringsSolution.IsIsomorphicByDictionary(example.S, example.T);

        Assert.Equal(example.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsIsomorphicByHashMap_LeetCodeExamples_ReturnsExpected(IsomorphismCase example)
    {
        var actual = IsomorphicStringsSolution.IsIsomorphicByHashMap(example.S, example.T);

        Assert.Equal(example.Expected, actual);
    }

    // One LeetCode example: the two strings to test for a bijection, and whether one
    // exists. The two strings are the same type and the relation is not symmetric, so
    // the row names which is which rather than leaving two interchangeable positions.
    // Nested because it is only ever used inside this test class - it is this harness's
    // own vocabulary, not a type another file would import.
    public readonly record struct IsomorphismCase(string S, string T, bool Expected);
}
