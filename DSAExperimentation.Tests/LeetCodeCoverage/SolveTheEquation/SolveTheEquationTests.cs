using DSAExperimentation.LeetCode.SolveTheEquation;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SolveTheEquation;

// LeetCode 640. Solve the Equation: SolveTheEquationSolution parses each side of
// "Ax+B=Cx+D" into a running (coefficient of x, constant) pair via one
// left-to-right scan, then combines the two sides and solves the single
// resulting linear equation.
public sealed class SolveTheEquationTests
{
    public static TheoryData<string, string> Examples => new()
    {
        { "x+5-3+x=6+x-2", "x=2" },
        { "x=x", "Infinite solutions" },
        { "2x=x", "x=0" },
        { "x=x+2", "No solution" },
    };

    [Theory]
    [MemberData(nameof(Examples))]
    public void SolveBySubstringParse_LeetCodeAndEdgeCaseExamples_ReturnsExpectedResult(string equation, string expected)
        => Assert.Equal(expected, SolveTheEquationSolution.SolveBySubstringParse(equation));

    [Theory]
    [MemberData(nameof(Examples))]
    public void SolveBySpanParse_LeetCodeAndEdgeCaseExamples_ReturnsExpectedResult(string equation, string expected)
        => Assert.Equal(expected, SolveTheEquationSolution.SolveBySpanParse(equation));
}
