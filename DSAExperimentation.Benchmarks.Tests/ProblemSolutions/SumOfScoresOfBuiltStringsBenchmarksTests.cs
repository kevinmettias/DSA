using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SumOfScoresOfBuiltStringsBenchmarks (ARCHITECTURE 17.9): both arms are
// SumOfScoresOfBuiltStringsSolution's - the O(n^2) suffix comparison against this repo's own ZFunction -
// so a harness whose arms disagree is timing two different questions. Both answer with a bare long, and
// the text is a seeded draw over a two-letter alphabet, so a rebuild at the same Length has to produce
// the same total.
public sealed partial class SumOfScoresOfBuiltStringsBenchmarksTests
{
    private const int SmallestLength = 500;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            BuildHarness().BruteForceSuffixComparison(),
            BuildHarness().BruteForceSuffixComparison());

    [Fact]
    public void BruteForceSuffixComparison_SeededTwoLetterText_AgreesWithTheOtherArm()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ZFunctionOverText(), harness.BruteForceSuffixComparison());
    }

    [Fact]
    public void ZFunctionOverText_SeededTwoLetterText_AgreesWithTheOtherArm()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForceSuffixComparison(), harness.ZFunctionOverText());
    }

    private static SumOfScoresOfBuiltStringsBenchmarks BuildHarness()
    {
        var harness = new SumOfScoresOfBuiltStringsBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
