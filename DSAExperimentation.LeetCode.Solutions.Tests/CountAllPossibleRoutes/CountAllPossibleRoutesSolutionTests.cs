using DSAExperimentation.LeetCode.Solutions.CountAllPossibleRoutes;

namespace DSAExperimentation.LeetCode.Solutions.Tests.CountAllPossibleRoutes;

// Harness only. Both strategies are CountAllPossibleRoutesSolution's - this file pins
// them to two cases the original coverage
// left untested: a start that equals the finish (the route of length zero counts on
// its own), and a fuel budget large enough to force real back-and-forth revisiting.
public sealed partial class CountAllPossibleRoutesSolutionTests
{
    public static TheoryData<CountRoutesCase> Cases =>
        new()
        {
            { new CountRoutesCase(Locations: [1, 2, 3], Start: 0, Finish: 0, Fuel: 0, Expected: 1) },
            { new CountRoutesCase(Locations: [1, 2, 3], Start: 0, Finish: 2, Fuel: 5, Expected: 6) },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountRoutesByNaiveRecursion_Cases_ReturnsRouteCount(CountRoutesCase testCase)
    {
        var actual = CountAllPossibleRoutesSolution.CountRoutesByNaiveRecursion(
            testCase.Locations, testCase.Start, testCase.Finish, testCase.Fuel);

        Assert.Equal(testCase.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountRoutesByMemoizedRecurrence_Cases_ReturnsRouteCount(CountRoutesCase testCase)
    {
        var actual = CountAllPossibleRoutesSolution.CountRoutesByMemoizedRecurrence(
            testCase.Locations, testCase.Start, testCase.Finish, testCase.Fuel);

        Assert.Equal(testCase.Expected, actual);
    }

    // One case: the city locations, the route's two endpoints and the fuel
    // budget, plus the number of routes they allow. The five values are one thing - a
    // route query - so they travel as one named case rather than as five positions a
    // caller has to count off. Nested because it is only ever used inside this test
    // class: it is this harness's own vocabulary, not a type another file would import.
    public readonly record struct CountRoutesCase(int[] Locations, int Start, int Finish, int Fuel, int Expected);
}
