using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for PowXnBenchmarks (ARCHITECTURE 17.9): its two arms are competing strategies for
// one question - the base raised to the exponent - so a harness whose arms disagree is timing two
// different problems. Exponent is the only [Params] axis and the base is fixed, so each axis value is
// its own harness. The answer is a double, and the repeated-multiplication arm accumulates one
// rounding error per unit of the exponent while the squaring arm accumulates one per bit, so the two
// are compared within the class's own tolerance rather than for exact double equality.
public sealed partial class PowXnBenchmarksTests
{
    private const int SmallestExponent = 10_000;

    private const double RelativeTolerance = 1e-9;

    [Fact]
    public void RepeatedMultiplication_AgreesWithExponentiationBySquaring()
    {
        var harness = Harness(SmallestExponent);

        Assert.Equal(
            harness.ExponentiationBySquaring(),
            harness.RepeatedMultiplication(),
            RelativeTolerance);
    }

    [Fact]
    public void ExponentiationBySquaring_AgreesWithRepeatedMultiplication()
    {
        var harness = Harness(SmallestExponent);

        Assert.Equal(
            harness.RepeatedMultiplication(),
            harness.ExponentiationBySquaring(),
            RelativeTolerance);
    }

    private static PowXnBenchmarks Harness(int exponent) => new() { Exponent = exponent };
}
