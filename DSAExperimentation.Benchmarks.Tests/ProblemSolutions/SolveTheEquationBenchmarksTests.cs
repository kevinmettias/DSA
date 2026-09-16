using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SolveTheEquationBenchmarks (ARCHITECTURE 17.9): both arms are
// SolveTheEquationSolution's parsers of the same generated equation, so a harness whose arms
// disagree is timing two different problems. Setup is a pure function of TermsPerSide, so the
// same TermsPerSide must rebuild the same equation.
//
// Each arm answers with the solution text itself - "x=1", "No solution", "Infinite solutions" -
// rather than a proxy for it, so comparing the two renderings compares the whole answer.
public sealed partial class SolveTheEquationBenchmarksTests
{
    private const int SmallestTermsPerSide = 200;

    [Fact]
    public void Setup_SameTermsPerSide_RebuildsTheSameEquation() =>
        Assert.Equal(BuildHarness().SubstringParse(), BuildHarness().SubstringParse());

    [Fact]
    public void SubstringParse_TwoHundredTermsPerSide_AgreesWithSpanParse()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.SpanParse(), harness.SubstringParse());
    }

    [Fact]
    public void SpanParse_TwoHundredTermsPerSide_AgreesWithSubstringParse()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.SubstringParse(), harness.SpanParse());
    }

    private static SolveTheEquationBenchmarks BuildHarness()
    {
        var harness = new SolveTheEquationBenchmarks { TermsPerSide = SmallestTermsPerSide };
        harness.Setup();

        return harness;
    }
}
