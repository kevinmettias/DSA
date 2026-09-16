using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ReverseIntegerBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - pulling digits off with division and rebuilding with
// multiplication against pushing each digit onto a stack and popping them back out - so a harness
// whose arms disagree is timing two different problems.
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
    public void Arithmetic_NonOverflowingValue_AgreesWithStackDigits()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedReversedValue, harness.Arithmetic());
        Assert.Equal(harness.StackDigits(), harness.Arithmetic());
    }

    [Fact]
    public void StackDigits_NonOverflowingValue_AgreesWithArithmetic()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedReversedValue, harness.StackDigits());
        Assert.Equal(harness.Arithmetic(), harness.StackDigits());
    }

    private static ReverseIntegerBenchmarks BuildHarness() =>
        new() { Value = SmallestValue };
}
