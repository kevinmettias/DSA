using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindGreatestCommonDivisorOfArrayBenchmarks (ARCHITECTURE 17.9): the generic
// arms check only compares the arms with each other, so this pins the baseline's gcd to the value
// Setup's construction fixes independently of any arm.
//
// Setup pins _nums[0] to 1, so the array's minimum is 1 on every run and the gcd of the smallest and
// largest elements is 1 whatever the seed produced for the maximum - the answer is decisive, and it
// is also subtraction's own worst case (gcd(1, max) costs max - 1 single-unit decrements).
public sealed partial class FindGreatestCommonDivisorOfArrayBenchmarksTests
{
    // The smaller of Setup's [Params(200, 1_000)] lengths.
    private const int SmallestLength = 200;

    // Setup's own guarantee: a minimum of 1 makes the gcd of the two extremes 1 for any maximum.
    private const int ExpectedGreatestCommonDivisor = 1;

    [Fact]
    public void SubtractionGcdOfMinAndMax_MinimumPinnedToOne_ReturnsOne() =>
        Assert.Equal(ExpectedGreatestCommonDivisor, BuildHarness().SubtractionGcdOfMinAndMax());

    private static FindGreatestCommonDivisorOfArrayBenchmarks BuildHarness()
    {
        var harness = new FindGreatestCommonDivisorOfArrayBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
