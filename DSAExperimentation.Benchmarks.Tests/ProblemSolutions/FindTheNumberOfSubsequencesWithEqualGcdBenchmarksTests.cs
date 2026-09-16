using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindTheNumberOfSubsequencesWithEqualGcdBenchmarks (ARCHITECTURE 17.9): its
// two arms are competing strategies for one question - walking the 3^Length join/join/neither tree
// against a memo over (index, gcd1, gcd2) states - so a harness whose arms disagree is timing two
// different problems. Setup draws the values from one fixed seed, so the same Length must rebuild
// the same array.
public sealed partial class FindTheNumberOfSubsequencesWithEqualGcdBenchmarksTests
{
    private const int SmallestLength = 8;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().BruteForce(), BuildHarness().BruteForce());

    [Fact]
    public void BruteForce_SmallestLength_AgreesWithGcdMemoization()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.GcdMemoization(), harness.BruteForce());
    }

    [Fact]
    public void GcdMemoization_SmallestLength_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForce(), harness.GcdMemoization());
    }

    private static FindTheNumberOfSubsequencesWithEqualGcdBenchmarks BuildHarness()
    {
        var harness = new FindTheNumberOfSubsequencesWithEqualGcdBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
