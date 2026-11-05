using DSAExperimentation.LeetCode.Solutions.FirstBadVersion;

namespace DSAExperimentation.LeetCode.Solutions.Tests.FirstBadVersion;

// Harness only: both strategies live in FirstBadVersionSolution and are asserted
// against the same cases, here LeetCode's official large-n overflow case.
// LeetCode's published examples run in CatalogCaseTests.
public sealed partial class FirstBadVersionSolutionTests
{
    public static TheoryData<int, int, int> Cases =>
        new()
        {
            { 2126753390, 1702766719, 1702766719 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void FirstBadVersionByLinearScan_Cases_ReturnsFirstBadVersion(
        int versionCount, int firstBad, int expected)
    {
        var actual = FirstBadVersionSolution.FirstBadVersionByLinearScan(versionCount, firstBad);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void FirstBadVersionByPredicateSearch_Cases_ReturnsFirstBadVersion(
        int versionCount, int firstBad, int expected)
    {
        var actual = FirstBadVersionSolution.FirstBadVersionByPredicateSearch(versionCount, firstBad);

        Assert.Equal(expected, actual);
    }

    // LeetCode's own upper limit, n = 2^31 - 1, with the last version the first bad
    // one: the answer is that version by definition. Kept off the shared cases
    // because the linear scan would walk all 2^31 - 1 versions to reach it.
    [Fact]
    public void FirstBadVersionByPredicateSearch_VersionCountAtIntMaxValue_ReturnsLastVersion() =>
        Assert.Equal(int.MaxValue, FirstBadVersionSolution.FirstBadVersionByPredicateSearch(int.MaxValue, int.MaxValue));
}
