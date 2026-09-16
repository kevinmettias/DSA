using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountWaysToChooseCoprimeIntegersFromRowsBenchmarks (ARCHITECTURE 17.9): its two
// arms are competing strategies for the same question - a DFS over every row's every value against a
// GCD-counting DP - so a harness whose arms disagree is timing two different problems, not two ways
// of answering one. Setup draws the matrix from one fixed seed, so the same Size must rebuild the
// same matrix; otherwise two published numbers were never comparable in the first place.
//
// The matrix is private and the way count is the only thing either arm reports, so the documented
// shape is asserted through that: a way is one value chosen from each of the Size rows, and a Size x
// Size matrix offers only Size^Size such selections in total.
public sealed partial class CountWaysToChooseCoprimeIntegersFromRowsBenchmarksTests
{
    private const int SmallestSize = 5;

    private const int SelectionCount = 3_125; // SmallestSize ^ SmallestSize

    [Fact]
    public void Setup_SameSize_RebuildsTheSameMatrix()
    {
        Assert.InRange(BuildHarness().BruteForce(), 0, SelectionCount);
        Assert.Equal(BuildHarness().BruteForce(), BuildHarness().BruteForce());
    }

    [Fact]
    public void BruteForce_SeededSquareMatrix_AgreesWithGcdCountingDp()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.GcdCountingDp(), harness.BruteForce());
    }

    [Fact]
    public void GcdCountingDp_SeededSquareMatrix_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForce(), harness.GcdCountingDp());
    }

    private static CountWaysToChooseCoprimeIntegersFromRowsBenchmarks BuildHarness()
    {
        var harness = new CountWaysToChooseCoprimeIntegersFromRowsBenchmarks { Size = SmallestSize };
        harness.Setup();

        return harness;
    }
}
