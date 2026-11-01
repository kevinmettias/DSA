using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountAllPossibleRoutesBenchmarks (ARCHITECTURE 17.9): BenchmarkArmsTests
// already holds the arms to agreeing at the smallest fuel both run, so this pins the baseline to
// the route count the recurrence's own definition fixes there. The class carries no [GlobalSetup] -
// the fuel count is the whole workload - and routes may re-visit a city, so the four hard-coded
// cities admit 72 walks from city 0 to city 3 within eight units of fuel, counted here
// independently of both arms.
public sealed partial class CountAllPossibleRoutesBenchmarksTests
{
    private const int SmallestFuel = 8;
    private const int RoutesWithinEightFuel = 72;

    [Fact]
    public void NaiveRecursion_EightFuel_CountsEveryRoute() =>
        Assert.Equal(RoutesWithinEightFuel, BuildHarness().NaiveRecursion(SmallestFuel));

    private static CountAllPossibleRoutesBenchmarks BuildHarness() =>
        new();
}
