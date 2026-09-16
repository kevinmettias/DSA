using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountAllValidPickupAndDeliveryOptionsBenchmarks (ARCHITECTURE 17.9): its two
// arms are competing strategies for the same question - the bottom-up running product against the
// same recurrence driven top-down through this repo's Memoizer - so a harness whose arms disagree is
// timing two different problems. Both arms return a long, so they are compared directly. The class
// carries no [GlobalSetup] - the order count is the whole workload - so the recurrence's own closed
// form f(i) = f(i-1) * i * (2i - 1) mod 1e9+7 for the smallest [Params] order count lives in the arm
// tests, computed here independently of both arms.
public sealed partial class CountAllValidPickupAndDeliveryOptionsBenchmarksTests
{
    private const int SmallestOrders = 100;
    private const long WaysForOneHundredOrders = 14159051;

    [Fact]
    public void Tabulation_OneHundredOrders_AgreesWithMemoizedAndTheClosedForm()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.Memoized(), harness.Tabulation());
        Assert.Equal(WaysForOneHundredOrders, harness.Tabulation());
    }

    [Fact]
    public void Memoized_OneHundredOrders_AgreesWithTabulationAndTheClosedForm()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.Tabulation(), harness.Memoized());
        Assert.Equal(WaysForOneHundredOrders, harness.Memoized());
    }

    private static CountAllValidPickupAndDeliveryOptionsBenchmarks BuildHarness() =>
        new() { Orders = SmallestOrders };
}
