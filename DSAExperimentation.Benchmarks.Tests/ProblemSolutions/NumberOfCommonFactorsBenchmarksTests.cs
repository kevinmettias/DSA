using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for NumberOfCommonFactorsBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - a scan of every divisor up to min(a, b) against the
// sqrt(gcd)-bounded divisor enumeration - so a harness whose arms disagree is counting the factors of
// two different operand pairs. Setup draws both operands from the seeded top half of [0, Magnitude],
// so the same Magnitude must rebuild the same pair.
//
// Both arms return an int, so they are compared directly.
public sealed partial class NumberOfCommonFactorsBenchmarksTests
{
    private const int SmallestMagnitude = 10_000;

    [Fact]
    public void Setup_SameMagnitude_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().LinearScan(), BuildHarness().LinearScan());

    [Fact]
    public void DivisorEnumeration_AgreesWithLinearScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.LinearScan(), harness.DivisorEnumeration());
    }

    [Fact]
    public void LinearScan_AgreesWithDivisorEnumeration()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.DivisorEnumeration(), harness.LinearScan());
    }

    private static NumberOfCommonFactorsBenchmarks BuildHarness()
    {
        var harness = new NumberOfCommonFactorsBenchmarks { Magnitude = SmallestMagnitude };
        harness.Setup();

        return harness;
    }
}
