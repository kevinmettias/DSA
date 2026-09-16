using DSAExperimentation.LeetCode.SolveTheEquation;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SolveTheEquation;

// LeetCode 640. Solve the Equation: SolveTheEquationSolution parses each side of
// "Ax+B=Cx+D" into a running (coefficient of x, constant) pair via one
// left-to-right scan, then combines the two sides and solves the single
// resulting linear equation.
public sealed class SolveTheEquationTests
{
    public static TheoryData<EquationExample> Examples => new()
    {
        new EquationExample(Equation: "x+5-3+x=6+x-2", Expected: "x=2"),
        new EquationExample(Equation: "x=x", Expected: "Infinite solutions"),
        new EquationExample(Equation: "2x=x", Expected: "x=0"),
        new EquationExample(Equation: "x=x+2", Expected: "No solution"),
    };

    [Theory]
    [MemberData(nameof(Examples))]
    public void SolveBySubstringParse_LeetCodeAndEdgeCaseExamples_ReturnsExpectedResult(EquationExample example)
        => Assert.Equal(example.Expected, SolveTheEquationSolution.SolveBySubstringParse(example.Equation));

    [Theory]
    [MemberData(nameof(Examples))]
    public void SolveBySpanParse_LeetCodeAndEdgeCaseExamples_ReturnsExpectedResult(EquationExample example)
        => Assert.Equal(example.Expected, SolveTheEquationSolution.SolveBySpanParse(example.Equation));

    // One LeetCode example: the equation to solve and the answer it solves to. The
    // equation and the answer are adjacent strings at the call site, so the bundle
    // names the input and the expectation rather than leaving them swappable.
    public readonly record struct EquationExample(string Equation, string Expected);
}
