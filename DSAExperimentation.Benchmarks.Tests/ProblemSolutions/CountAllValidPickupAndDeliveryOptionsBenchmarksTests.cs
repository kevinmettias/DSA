using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountAllValidPickupAndDeliveryOptionsBenchmarks (ARCHITECTURE 17.9):
// BenchmarkArmsTests already holds the arms to agreeing at the smallest [Params] order count, so this
// pins the baseline to the recurrence's own closed form f(i) = f(i-1) * i * (2i - 1) mod 1e9+7 there,
// computed independently of both arms. The class carries no [GlobalSetup] - the order count is the
// whole workload.
public sealed partial class CountAllValidPickupAndDeliveryOptionsBenchmarksTests
{
    private const int SmallestOrders = 100;
    private const long WaysForOneHundredOrders = 14159051;

    [Fact]
    public void Tabulation_OneHundredOrders_MatchesTheClosedForm() =>
        Assert.Equal(WaysForOneHundredOrders, BuildHarness().Tabulation());

    private static CountAllValidPickupAndDeliveryOptionsBenchmarks BuildHarness() =>
        new() { Orders = SmallestOrders };
}
