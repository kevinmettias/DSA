namespace DSAExperimentation.Tests.LeetCodeCoverage.SolveTheEquation;

// LeetCode 640. Solve the Equation: parse each side of "Ax+B=Cx+D" into a running
// (coefficient of x, constant) pair via one left-to-right scan (sign, then digits,
// then an optional trailing 'x'), then combine the two sides and solve the single
// resulting linear equation. No repo container or algorithm primitive applies to a
// single closed-form parse-then-combine problem like this - the same "lighter
// repo-primitive fit" case as Complex Number Multiplication (LC 537), whose own
// ComplexNumberMultiplicationTests.cs already sets this precedent.
public sealed class SolveTheEquationTests
{
    [Theory]
    [InlineData("x+5-3+x=6+x-2", "x=2")]
    [InlineData("x=x", "Infinite solutions")]
    [InlineData("2x=x", "x=0")]
    [InlineData("x=x+2", "No solution")]
    public void Solve_LeetCodeAndEdgeCaseExamples_ReturnsExpectedResult(string equation, string expected)
        => Assert.Equal(expected, Solve(equation));

    private static string Solve(string equation)
    {
        var separator = equation.IndexOf('=');
        var (leftCoefficient, leftConstant) = ParseSide(equation.AsSpan(0, separator));
        var (rightCoefficient, rightConstant) = ParseSide(equation.AsSpan(separator + 1));

        var coefficientX = leftCoefficient - rightCoefficient;
        var constant = rightConstant - leftConstant;

        if (coefficientX == 0)
        {
            return constant == 0 ? "Infinite solutions" : "No solution";
        }

        return $"x={constant / coefficientX}";
    }

    private static (int CoefficientX, int Constant) ParseSide(ReadOnlySpan<char> side)
    {
        var coefficientX = 0;
        var constant = 0;
        var sign = 1;
        var i = 0;

        while (i < side.Length)
        {
            if (side[i] == '+')
            {
                sign = 1;
                i++;
                continue;
            }

            if (side[i] == '-')
            {
                sign = -1;
                i++;
                continue;
            }

            var start = i;

            while (i < side.Length && side[i] != '+' && side[i] != '-')
            {
                i++;
            }

            var term = side[start..i];

            if (term[^1] == 'x')
            {
                var digits = term[..^1];
                coefficientX += sign * (digits.IsEmpty ? 1 : int.Parse(digits));
            }
            else
            {
                constant += sign * int.Parse(term);
            }
        }

        return (coefficientX, constant);
    }
}
