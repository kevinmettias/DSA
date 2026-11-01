using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CheckIfItIsAGoodArrayBenchmarks (ARCHITECTURE 17.9): the generic check holds
// the arms to each other, but both answer with a bare bool, so agreement says only that the two gcd
// strategies reached the same verdict on the same values. This pins the baseline's verdict to the
// one Setup's construction decides without either arm.
public sealed partial class CheckIfItIsAGoodArrayBenchmarksTests
{
    private const int SmallestLength = 50;

    // Setup multiplies every value by 3, so 3 divides each of them and therefore divides the gcd of
    // all of them: no array the seed can produce has gcd 1, and the good-array answer is false for
    // every one of them. That is exactly the shape the harness wants - the running gcd never
    // reaches 1 early, so neither arm can short-circuit and both are forced through every element -
    // and it is also what makes the workload's verdict decisive.
    private const bool ExpectedVerdict = false;

    [Fact]
    public void IsGoodArrayBySubtractionGcd_SeededMultiplesOfThree_ReturnsFalse() =>
        Assert.Equal(ExpectedVerdict, BuildHarness().IsGoodArrayBySubtractionGcd());

    private static CheckIfItIsAGoodArrayBenchmarks BuildHarness()
    {
        var harness = new CheckIfItIsAGoodArrayBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
