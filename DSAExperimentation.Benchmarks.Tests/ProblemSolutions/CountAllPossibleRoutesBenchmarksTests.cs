using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountAllPossibleRoutesBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - the unmemoized route recursion against the same
// recurrence driven through this repo's Memoizer - so a harness whose arms disagree is timing two
// different problems. Both arms return an int, so they are compared directly. The class carries no
// [GlobalSetup] - the fuel count is the whole workload - so the route count the recurrence's own
// definition fixes for the smallest [Params] fuel lives in the arm tests: routes may re-visit a
// city, so the four hard-coded cities admit 72 walks from city 0 to city 3 within eight units of
// fuel, counted here independently of both arms.
public sealed partial class CountAllPossibleRoutesBenchmarksTests
{
    private const int SmallestFuel = 8;
    private const int RoutesWithinEightFuel = 72;

    [Fact]
    public void NaiveRecursion_EightFuel_AgreesWithMemoizedTopDownAndTheRouteCount()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MemoizedTopDown(), harness.NaiveRecursion());
        Assert.Equal(RoutesWithinEightFuel, harness.NaiveRecursion());
    }

    [Fact]
    public void MemoizedTopDown_EightFuel_AgreesWithNaiveRecursionAndTheRouteCount()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.NaiveRecursion(), harness.MemoizedTopDown());
        Assert.Equal(RoutesWithinEightFuel, harness.MemoizedTopDown());
    }

    private static CountAllPossibleRoutesBenchmarks BuildHarness() =>
        new() { Fuel = SmallestFuel };
}
