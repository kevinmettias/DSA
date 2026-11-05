using DSAExperimentation.LeetCode.Solutions.IsomorphicStrings;

namespace DSAExperimentation.LeetCode.Solutions.Tests.IsomorphicStrings;

// Harness only: both strategies live in IsomorphicStringsSolution. One test
// method per strategy over one shared set of cases, so a failure names the
// strategy that broke. The first three rows are LeetCode's own examples, kept here
// because the catalog records them without their answers; the last is the empty
// pair. The cases that exercise both mapping directions are catalog cases and run
// in CatalogCaseTests.
public sealed partial class IsomorphicStringsSolutionTests
{
    public static TheoryData<IsomorphismCase> Cases =>
        new()
        {
            { new IsomorphismCase(S: "egg", T: "add", Expected: true) },
            { new IsomorphismCase(S: "foo", T: "bar", Expected: false) },
            { new IsomorphismCase(S: "paper", T: "title", Expected: true) },
            { new IsomorphismCase(S: "", T: "", Expected: true) },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void IsIsomorphicByDictionary_Cases_ReturnsExpected(IsomorphismCase testCase)
    {
        var actual = IsomorphicStringsSolution.IsIsomorphicByDictionary(testCase.S, testCase.T);

        Assert.Equal(testCase.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void IsIsomorphicByHashMap_Cases_ReturnsExpected(IsomorphismCase testCase)
    {
        var actual = IsomorphicStringsSolution.IsIsomorphicByHashMap(testCase.S, testCase.T);

        Assert.Equal(testCase.Expected, actual);
    }

    // One case: the two strings to test for a bijection, and whether one
    // exists. The two strings are the same type and the relation is not symmetric, so
    // the row names which is which rather than leaving two interchangeable positions.
    // Nested because it is only ever used inside this test class - it is this harness's
    // own vocabulary, not a type another file would import.
    public readonly record struct IsomorphismCase(string S, string T, bool Expected);
}
