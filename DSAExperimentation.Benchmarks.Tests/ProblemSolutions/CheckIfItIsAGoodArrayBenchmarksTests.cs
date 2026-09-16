using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CheckIfItIsAGoodArrayBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - the repeated-subtraction gcd against the
// modulo-based Euclidean gcd - so a harness whose arms disagree is timing two different problems.
// Both arms answer with a bare bool, so agreement between them says the two gcd strategies reached
// the same verdict on the same values.
public sealed partial class CheckIfItIsAGoodArrayBenchmarksTests
{
    private const int SmallestLength = 50;

    // Setup multiplies every value by 3, so 3 divides each of them and therefore divides the gcd of
    // all of them: no array the seed can produce has gcd 1, and the good-array answer is false for
    // every one of them. That is exactly the shape the harness wants - the running gcd never
    // reaches 1 early, so neither arm can short-circuit and both are forced through every element -
    // and it is also what makes the rebuilt workload's verdict decisive.
    private const bool ExpectedVerdict = false;

    [Fact]
    public void Setup_SameLength_RebuildsTheAllMultiplesOfThreeValues()
    {
        var first = BuildHarness();
        var second = BuildHarness();

        Assert.Equal(ExpectedVerdict, first.IsGoodArrayByEuclideanGcd());
        Assert.Equal(ExpectedVerdict, second.IsGoodArrayBySubtractionGcd());
    }

    [Fact]
    public void IsGoodArrayByEuclideanGcd_SeededMultiplesOfThree_AgreesWithSubtractionGcd()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.IsGoodArrayBySubtractionGcd(), harness.IsGoodArrayByEuclideanGcd());
    }

    [Fact]
    public void IsGoodArrayBySubtractionGcd_SeededMultiplesOfThree_AgreesWithEuclideanGcd()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.IsGoodArrayByEuclideanGcd(), harness.IsGoodArrayBySubtractionGcd());
    }

    private static CheckIfItIsAGoodArrayBenchmarks BuildHarness()
    {
        var harness = new CheckIfItIsAGoodArrayBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
