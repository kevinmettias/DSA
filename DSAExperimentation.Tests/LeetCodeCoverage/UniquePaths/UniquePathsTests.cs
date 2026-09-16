using DSAExperimentation.LeetCode.UniquePaths;

namespace DSAExperimentation.Tests.LeetCodeCoverage.UniquePaths;

// Harness only. Both strategies are UniquePathsSolution's - the combinatorial
// closed form and the memoized grid recurrence - checked against LeetCode's
// published examples.
public sealed class UniquePathsTests
{
    public static TheoryData<int, int, int> Examples =>
        new()
        {
            { 3, 7, 28 },
            { 3, 2, 3 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountPathsByCombinatorics_LeetCodeExamples_ReturnsExpectedCount(
        int m, int n, int expected)
    {
        var actual = UniquePathsSolution.CountPathsByCombinatorics(m, n);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountPathsByMemoizedRecurrence_LeetCodeExamples_ReturnsExpectedCount(
        int m, int n, int expected)
    {
        var actual = UniquePathsSolution.CountPathsByMemoizedRecurrence(m, n);

        Assert.Equal(expected, actual);
    }
}
