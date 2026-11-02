using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountNumberOfBalancedPermutationsBenchmarks (ARCHITECTURE 17.9):
// BenchmarkArmsTests already holds the arms to agreeing, and to answering the same on a rebuilt
// harness and when called again on one, so this asserts what it cannot: a ceiling on the answer
// that comes from the workload's shape rather than from either arm.
public sealed partial class CountNumberOfBalancedPermutationsBenchmarksTests
{
    private const int SmallestLength = 6;

    // Distinct permutations of a six-digit num, the ceiling on how many of them can be balanced.
    private const long MostSixDigitPermutations = 720;

    // The documented shape: num holds SmallestLength digits, so the answer can never exceed
    // the SmallestLength! distinct permutations they can be arranged into.
    [Fact]
    public void BruteForce_SixDigitNumber_CountsNoMoreThanItsPermutations() =>
        Assert.InRange(BuildHarness().BruteForce(SmallestLength), 0L, MostSixDigitPermutations);

    private static CountNumberOfBalancedPermutationsBenchmarks BuildHarness()
    {
        var harness = new CountNumberOfBalancedPermutationsBenchmarks();
        harness.Setup();

        return harness;
    }
}
