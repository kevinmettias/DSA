using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CheckIfDigitsAreEqualInStringAfterOperationsIBenchmarks (ARCHITECTURE 17.9):
// its two arms are competing strategies for the same question - reducing the string pair by pair
// against evaluating the same reduction's Pascal-row coefficients - so a harness whose arms disagree
// is timing two different problems. Both arms answer with a bare bool, so agreement between them says
// the two strategies reached the same verdict on the same digit string.
public sealed partial class CheckIfDigitsAreEqualInStringAfterOperationsIBenchmarksTests
{
    private const int SmallestLength = 3;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameDigitString()
    {
        var first = BuildHarness();
        var second = BuildHarness();

        // Neither strategy has anything to hoist out of the measured call, so Setup's whole job is
        // the digit string the fixture draws from its seed. That string is private and each arm
        // reduces it to one bit, so two harnesses built from the same Length reporting the same two
        // verdicts is the reading the rebuild can be pinned to.
        Assert.Equal(
            (first.IsEqualByAdjacentSumReduction(), first.IsEqualByPascalRowCoefficients()),
            (second.IsEqualByAdjacentSumReduction(), second.IsEqualByPascalRowCoefficients()));
    }

    [Fact]
    public void IsEqualByAdjacentSumReduction_SeededThreeDigitString_AgreesWithPascalRowCoefficients()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.IsEqualByPascalRowCoefficients(), harness.IsEqualByAdjacentSumReduction());
    }

    [Fact]
    public void IsEqualByPascalRowCoefficients_SeededThreeDigitString_AgreesWithAdjacentSumReduction()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.IsEqualByAdjacentSumReduction(), harness.IsEqualByPascalRowCoefficients());
    }

    private static CheckIfDigitsAreEqualInStringAfterOperationsIBenchmarks BuildHarness()
    {
        var harness = new CheckIfDigitsAreEqualInStringAfterOperationsIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
