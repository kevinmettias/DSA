using DSAExperimentation.LeetCode.DirectionAssignmentsWithExactlyKVisiblePeople;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DirectionAssignmentsWithExactlyKVisiblePeople;

// Harness only. The Pascal-convolution and Vandermonde-identity derivations
// both live in DirectionAssignmentsWithExactlyKVisiblePeopleSolution - this
// file just pins both strategies to LeetCode's published examples.
public sealed class DirectionAssignmentsWithExactlyKVisiblePeopleTests
{
    public static TheoryData<int, int, int, int> Examples =>
        new()
        {
            { 3, 1, 0, 2 },
            { 3, 2, 1, 4 },
            { 1, 0, 0, 2 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountAssignmentsByPascalConvolution_LeetCodeExamples_ReturnsAssignmentCount(
        int n, int pos, int k, int expected)
    {
        var actual = DirectionAssignmentsWithExactlyKVisiblePeopleSolution.CountAssignmentsByPascalConvolution(n, pos, k);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountAssignmentsByVandermondeIdentity_LeetCodeExamples_ReturnsAssignmentCount(
        int n, int pos, int k, int expected)
    {
        var actual = DirectionAssignmentsWithExactlyKVisiblePeopleSolution.CountAssignmentsByVandermondeIdentity(n, pos, k);
        Assert.Equal(expected, actual);
    }
}
