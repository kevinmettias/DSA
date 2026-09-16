using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DivideTwoIntegersBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - the built-in division LC 29 forbids, against searching for the
// first quotient candidate whose product overshoots the dividend via this repo's BinarySearch.
// LowerBound - so a harness whose arms disagree is timing two different problems. The class carries
// no [GlobalSetup]: both arms read the same Dividend and the same Divisor, and trunc(1_000_000 / 7)
// is 142857 at the benchmark's smallest dividend.
public sealed partial class DivideTwoIntegersBenchmarksTests
{
    private const int Divisor = 7;
    private const int SmallestDividend = 1_000_000;
    private const int ExpectedQuotientOfAMillionBySeven = 142_857;

    [Fact]
    public void BuiltInDivide_OneMillionBySeven_QuotientIsTheTruncationAndAgreesWithBinarySearchProduct()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedQuotientOfAMillionBySeven, harness.BuiltInDivide());
        Assert.Equal(harness.BinarySearchProduct(), harness.BuiltInDivide());
    }

    [Fact]
    public void BinarySearchProduct_OneMillionBySeven_QuotientIsTheTruncationAndAgreesWithBuiltInDivide()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedQuotientOfAMillionBySeven, harness.BinarySearchProduct());
        Assert.Equal(harness.BuiltInDivide(), harness.BinarySearchProduct());
    }

    private static DivideTwoIntegersBenchmarks BuildHarness() => new() { Dividend = SmallestDividend };
}
