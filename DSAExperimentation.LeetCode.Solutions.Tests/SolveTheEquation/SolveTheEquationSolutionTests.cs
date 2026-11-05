using DSAExperimentation.LeetCode.Solutions.SolveTheEquation;

namespace DSAExperimentation.LeetCode.Solutions.Tests.SolveTheEquation;

// LeetCode 640. Solve the Equation: SolveTheEquationSolution parses each side of
// "Ax+B=Cx+D" into a running (coefficient of x, constant) pair via one
// left-to-right scan, then combines the two sides and solves the single
// resulting linear equation.
public sealed partial class SolveTheEquationSolutionTests
{
    public static TheoryData<EquationCase> Cases => new()
    {
        new EquationCase(Equation: "x=x+2", Expected: "No solution"),
    };

    [Theory]
    [MemberData(nameof(Cases))]
    public void SolveBySubstringParse_LeetCodeAndEdgeCaseExamples_ReturnsExpectedResult(EquationCase testCase)
        => Assert.Equal(testCase.Expected, SolveTheEquationSolution.SolveBySubstringParse(testCase.Equation));

    [Theory]
    [MemberData(nameof(Cases))]
    public void SolveBySpanParse_LeetCodeAndEdgeCaseExamples_ReturnsExpectedResult(EquationCase testCase)
        => Assert.Equal(testCase.Expected, SolveTheEquationSolution.SolveBySpanParse(testCase.Equation));

    // One case: the equation to solve and the answer it solves to. The
    // equation and the answer are adjacent strings at the call site, so the bundle
    // names the input and the expectation rather than leaving them swappable.
    public readonly record struct EquationCase(string Equation, string Expected);
}
