using DSAExperimentation.LeetCode.CountAllPossibleRoutes;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountAllPossibleRoutes;

// Harness only. Both strategies are CountAllPossibleRoutesSolution's - this file pins
// them to LeetCode's three published examples plus two cases the original coverage
// left untested: a start that equals the finish (the route of length zero counts on
// its own), and a fuel budget large enough to force real back-and-forth revisiting.
public sealed class CountAllPossibleRoutesTests
{
    public static TheoryData<int[], int, int, int, int> Examples =>
        new()
        {
            { new[] { 2, 3, 6, 8, 4 }, 1, 3, 5, 4 },
            { new[] { 4, 3, 1 }, 1, 0, 6, 5 },
            { new[] { 5, 2, 1 }, 0, 2, 3, 0 },
            { new[] { 1, 2, 3 }, 0, 0, 0, 1 },
            { new[] { 1, 2, 3 }, 0, 2, 5, 6 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountRoutesByNaiveRecursion_LeetCodeExamples_ReturnsRouteCount(
        int[] locations, int start, int finish, int fuel, int expected) =>
        Assert.Equal(
            expected,
            CountAllPossibleRoutesSolution.CountRoutesByNaiveRecursion(locations, start, finish, fuel));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountRoutesByMemoizedRecurrence_LeetCodeExamples_ReturnsRouteCount(
        int[] locations, int start, int finish, int fuel, int expected) =>
        Assert.Equal(
            expected,
            CountAllPossibleRoutesSolution.CountRoutesByMemoizedRecurrence(locations, start, finish, fuel));
}
