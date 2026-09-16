using DSAExperimentation.LeetCode.CountAllPossibleRoutes;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountAllPossibleRoutes;

// Harness only. Both strategies are CountAllPossibleRoutesSolution's - this file pins
// them to LeetCode's three published examples plus two cases the original coverage
// left untested: a start that equals the finish (the route of length zero counts on
// its own), and a fuel budget large enough to force real back-and-forth revisiting.
public sealed partial class CountAllPossibleRoutesTests
{
    public static TheoryData<CountRoutesCase> Examples =>
        new()
        {
            { new CountRoutesCase(Locations: [2, 3, 6, 8, 4], Start: 1, Finish: 3, Fuel: 5, Expected: 4) },
            { new CountRoutesCase(Locations: [4, 3, 1], Start: 1, Finish: 0, Fuel: 6, Expected: 5) },
            { new CountRoutesCase(Locations: [5, 2, 1], Start: 0, Finish: 2, Fuel: 3, Expected: 0) },
            { new CountRoutesCase(Locations: [1, 2, 3], Start: 0, Finish: 0, Fuel: 0, Expected: 1) },
            { new CountRoutesCase(Locations: [1, 2, 3], Start: 0, Finish: 2, Fuel: 5, Expected: 6) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountRoutesByNaiveRecursion_LeetCodeExamples_ReturnsRouteCount(CountRoutesCase example)
    {
        var actual = CountAllPossibleRoutesSolution.CountRoutesByNaiveRecursion(
            example.Locations, example.Start, example.Finish, example.Fuel);

        Assert.Equal(example.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountRoutesByMemoizedRecurrence_LeetCodeExamples_ReturnsRouteCount(CountRoutesCase example)
    {
        var actual = CountAllPossibleRoutesSolution.CountRoutesByMemoizedRecurrence(
            example.Locations, example.Start, example.Finish, example.Fuel);

        Assert.Equal(example.Expected, actual);
    }

    // One LeetCode example: the city locations, the route's two endpoints and the fuel
    // budget, plus the number of routes they allow. The five values are one thing - a
    // route query - so they travel as one named case rather than as five positions a
    // caller has to count off. Nested because it is only ever used inside this test
    // class: it is this harness's own vocabulary, not a type another file would import.
    public readonly record struct CountRoutesCase(int[] Locations, int Start, int Finish, int Fuel, int Expected);
}
