using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for StringTransformationBenchmarks (ARCHITECTURE 17.9): both arms answer the
// same question - how many LC 2851 transformations reach the target - and share the closed-form
// combine step, differing only in how the needed rotation-match count is computed, so a harness
// whose arms disagree is timing two different problems. Setup builds the target as a genuine
// rotation of the source, so the same length must rebuild the same pair.
public sealed partial class StringTransformationBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().BruteForceRotationCompare()),
            AnswerText.Of(BuildHarness().BruteForceRotationCompare()));

    [Fact]
    public void BruteForceRotationCompare_AgreesWithZFunctionSearch()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForceRotationCompare(), harness.ZFunctionSearch());
    }

    [Fact]
    public void ZFunctionSearch_AgreesWithBruteForceRotationCompare()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ZFunctionSearch(), harness.BruteForceRotationCompare());
    }

    private static StringTransformationBenchmarks BuildHarness()
    {
        var harness = new StringTransformationBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
