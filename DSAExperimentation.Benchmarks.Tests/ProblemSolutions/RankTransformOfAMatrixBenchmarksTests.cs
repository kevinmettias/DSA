using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for RankTransformOfAMatrixBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - the rank-transformed matrix LeetCode asks for - so a
// harness whose arms disagree is timing two different problems. Both arms return an int[][], and
// Assert.Equal on a jagged array falls back to reference equality for the inner rows, so
// AnswerText.Of is what actually compares them. Setup fills the matrix from one fixed seed, so the
// same Size must rebuild the same matrix.
public sealed partial class RankTransformOfAMatrixBenchmarksTests
{
    private const int SmallestSize = 8;

    [Fact]
    public void Setup_SameSize_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().IterativeRelaxation()),
            AnswerText.Of(BuildHarness().IterativeRelaxation()));

    [Fact]
    public void IterativeRelaxation_SmallestMatrix_AgreesWithDisjointSetRanking()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.IterativeRelaxation()),
            AnswerText.Of(harness.DisjointSetRanking()));
    }

    [Fact]
    public void DisjointSetRanking_SmallestMatrix_AgreesWithTheComposedArm()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.DisjointSetRanking()),
            AnswerText.Of(harness.IterativeRelaxation()));
    }

    private static RankTransformOfAMatrixBenchmarks BuildHarness()
    {
        var harness = new RankTransformOfAMatrixBenchmarks { Size = SmallestSize };
        harness.Setup();

        return harness;
    }
}
