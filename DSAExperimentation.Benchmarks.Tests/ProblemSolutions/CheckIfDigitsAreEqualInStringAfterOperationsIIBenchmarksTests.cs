using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CheckIfDigitsAreEqualInStringAfterOperationsIIBenchmarks (ARCHITECTURE 17.9):
// its two arms are competing strategies for the same question - reducing the long string pair by
// pair under the modulus against evaluating the same reduction through Lucas' binomial coefficients
// - so a harness whose arms disagree is timing two different problems. Both arms answer with a bare
// bool, so agreement between them says the two strategies reached the same verdict on the same
// digit string.
public sealed partial class CheckIfDigitsAreEqualInStringAfterOperationsIIBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameDigitString()
    {
        var first = BuildHarness();
        var second = BuildHarness();

        // Setup's whole job is the digit string drawn from one fixed seed. That string is private
        // and each arm reduces it to one bit, so two harnesses built from the same Length reporting
        // the same two verdicts is the reading the rebuild can be pinned to; the length itself is
        // what the arms' cost is measured against.
        Assert.Equal(
            (first.IsEqualByAdjacentSumReduction(), first.IsEqualByLucasBinomialCoefficients()),
            (second.IsEqualByAdjacentSumReduction(), second.IsEqualByLucasBinomialCoefficients()));
    }

    [Fact]
    public void IsEqualByAdjacentSumReduction_SeededLongDigitString_AgreesWithLucasBinomialCoefficients()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.IsEqualByLucasBinomialCoefficients(), harness.IsEqualByAdjacentSumReduction());
    }

    [Fact]
    public void IsEqualByLucasBinomialCoefficients_SeededLongDigitString_AgreesWithAdjacentSumReduction()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.IsEqualByAdjacentSumReduction(), harness.IsEqualByLucasBinomialCoefficients());
    }

    private static CheckIfDigitsAreEqualInStringAfterOperationsIIBenchmarks BuildHarness()
    {
        var harness = new CheckIfDigitsAreEqualInStringAfterOperationsIIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
