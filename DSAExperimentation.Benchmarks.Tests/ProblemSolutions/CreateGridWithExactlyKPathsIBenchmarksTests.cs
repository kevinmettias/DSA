using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CreateGridWithExactlyKPathsIBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - a candidate rectangle's monotone path count decided by
// simulating the rectangle against the closed-form binomial coefficient - and the solution states both
// build the identical grid, so a harness whose arms disagree is timing two different problems. The
// class has no [GlobalSetup]: K is fixed at 4 (the widest search, and the one target count no single
// a x b rectangle reaches inside a 3x3 footprint) and Side is the only [Params] value, so the smallest
// Side is 4 - a real search over a 4x4 footprint. Rows are returned in grid order, so the comparison
// keeps each row's position as part of the answer.
public sealed partial class CreateGridWithExactlyKPathsIBenchmarksTests
{
    private const int SmallestSide = 4;

    [Fact]
    public void PathCountDp_FourByFourGridWithFourPaths_AgreesWithBinomialFormula()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.BinomialFormula()), AnswerText.Of(harness.PathCountDp()));
    }

    [Fact]
    public void BinomialFormula_FourByFourGridWithFourPaths_AgreesWithPathCountDp()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.PathCountDp()), AnswerText.Of(harness.BinomialFormula()));
    }

    private static CreateGridWithExactlyKPathsIBenchmarks BuildHarness() =>
        new() { Side = SmallestSide };
}
