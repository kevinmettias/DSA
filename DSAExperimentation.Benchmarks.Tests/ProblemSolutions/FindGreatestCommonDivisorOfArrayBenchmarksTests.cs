using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindGreatestCommonDivisorOfArrayBenchmarks (ARCHITECTURE 17.9): its two arms
// are competing strategies for the same question - repeated subtraction against the modulo-based
// Euclidean algorithm - over the same min/max scan, so a harness whose arms disagree has computed
// two different gcds. Both answers are one int, so they are compared directly.
//
// Setup pins _nums[0] to 1, so the array's minimum is 1 on every run and the gcd of the smallest and
// largest elements is 1 whatever the seed produced for the maximum - the answer is decisive, and it
// is also subtraction's own worst case (gcd(1, max) costs max - 1 single-unit decrements).
public sealed partial class FindGreatestCommonDivisorOfArrayBenchmarksTests
{
    // The smaller of Setup's [Params(200, 5_000)] lengths.
    private const int SmallestLength = 200;

    // Setup's own guarantee: a minimum of 1 makes the gcd of the two extremes 1 for any maximum.
    private const int ExpectedGreatestCommonDivisor = 1;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameGreatestCommonDivisor() =>
        Assert.Equal(
            BuildHarness().SubtractionGcdOfMinAndMax(),
            BuildHarness().SubtractionGcdOfMinAndMax());

    [Fact]
    public void SubtractionGcdOfMinAndMax_MinimumPinnedToOne_AgreesWithEuclideanGcdOfMinAndMax()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedGreatestCommonDivisor, harness.SubtractionGcdOfMinAndMax());
        Assert.Equal(harness.EuclideanGcdOfMinAndMax(), harness.SubtractionGcdOfMinAndMax());
    }

    [Fact]
    public void EuclideanGcdOfMinAndMax_MinimumPinnedToOne_AgreesWithSubtractionGcdOfMinAndMax()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedGreatestCommonDivisor, harness.EuclideanGcdOfMinAndMax());
        Assert.Equal(harness.SubtractionGcdOfMinAndMax(), harness.EuclideanGcdOfMinAndMax());
    }

    private static FindGreatestCommonDivisorOfArrayBenchmarks BuildHarness()
    {
        var harness = new FindGreatestCommonDivisorOfArrayBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
