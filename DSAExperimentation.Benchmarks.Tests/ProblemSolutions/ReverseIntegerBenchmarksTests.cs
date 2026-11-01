using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ReverseIntegerBenchmarks (ARCHITECTURE 17.9). Arm agreement is
// BenchmarkArmsTests' job; this pins the baseline's answer to one derived from the digits alone.
//
// The class has no [GlobalSetup]; the whole workload is the [Params] Value, so the harness is
// constructed with the smallest tuned value directly. That value reverses to a nine-digit number
// that still fits an int, so it stays clear of the overflow branch both arms share and the answer is
// fixed by the digits themselves rather than read back out of either arm.
public sealed partial class ReverseIntegerBenchmarksTests
{
    private const int SmallestValue = 123456789;
    private const int ExpectedReversedValue = 987654321;

    [Fact]
    public void Arithmetic_NonOverflowingValue_ReversesTheDigits() =>
        Assert.Equal(ExpectedReversedValue, BuildHarness().Arithmetic());

    private static ReverseIntegerBenchmarks BuildHarness() =>
        new() { Value = SmallestValue };
}
