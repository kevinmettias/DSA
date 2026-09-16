using DSAExperimentation.LeetCode.FirstBadVersion;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FirstBadVersion;

// Harness only: both strategies live in FirstBadVersionSolution and are asserted
// against the same examples, including LeetCode's official large-n overflow case.
public sealed class FirstBadVersionTests
{
    public static TheoryData<int, int, int> Examples =>
        new()
        {
            { 5, 4, 4 },
            { 1, 1, 1 },
            { 2126753390, 1702766719, 1702766719 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FirstBadVersionByLinearScan_LeetCodeExamples_ReturnsFirstBadVersion(
        int n, int firstBad, int expected)
    {
        var actual = FirstBadVersionSolution.FirstBadVersionByLinearScan(n, firstBad);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void FirstBadVersionByLowerBound_LeetCodeExamples_ReturnsFirstBadVersion(
        int n, int firstBad, int expected)
    {
        var actual = FirstBadVersionSolution.FirstBadVersionByLowerBound(n, firstBad);

        Assert.Equal(expected, actual);
    }
}
