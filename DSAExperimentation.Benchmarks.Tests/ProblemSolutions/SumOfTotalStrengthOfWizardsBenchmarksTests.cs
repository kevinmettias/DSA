using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SumOfTotalStrengthOfWizardsBenchmarks (ARCHITECTURE 17.9): both arms are
// SumOfTotalStrengthOfWizardsSolution's - the O(n^2) walk over every subarray against the
// monotonic-stack contribution sweep - so a harness whose arms disagree is timing two different
// questions. Both answer with a bare int, reduced under the problem's own modulus, and the strengths
// are a seeded permutation, so a rebuild at the same Length has to produce the same total.
public sealed partial class SumOfTotalStrengthOfWizardsBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().BruteForce(), BuildHarness().BruteForce());

    [Fact]
    public void BruteForce_SeededPermutation_AgreesWithTheOtherArm()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MonotonicStackContribution(), harness.BruteForce());
    }

    [Fact]
    public void MonotonicStackContribution_SeededPermutation_AgreesWithTheOtherArm()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForce(), harness.MonotonicStackContribution());
    }

    private static SumOfTotalStrengthOfWizardsBenchmarks BuildHarness()
    {
        var harness = new SumOfTotalStrengthOfWizardsBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
