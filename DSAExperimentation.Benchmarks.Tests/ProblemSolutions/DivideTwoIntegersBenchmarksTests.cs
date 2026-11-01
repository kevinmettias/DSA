using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DivideTwoIntegersBenchmarks (ARCHITECTURE 17.9): an expected quotient known
// without either arm. The class carries no [GlobalSetup]: the arms read Dividend and a Divisor of 7,
// and trunc(1_000_000 / 7) is 142857 at the benchmark's smallest dividend.
public sealed partial class DivideTwoIntegersBenchmarksTests
{
    private const int SmallestDividend = 1_000_000;
    private const int ExpectedQuotientOfAMillionBySeven = 142_857;

    [Fact]
    public void BuiltInDivide_OneMillionBySeven_QuotientIsTheTruncation() =>
        Assert.Equal(ExpectedQuotientOfAMillionBySeven, BuildHarness().BuiltInDivide());

    private static DivideTwoIntegersBenchmarks BuildHarness() => new() { Dividend = SmallestDividend };
}
