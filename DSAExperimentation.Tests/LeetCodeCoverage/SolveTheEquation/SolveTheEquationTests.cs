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
        var leftSpan = equation.AsSpan(0, separator);
        var (leftCoefficient, leftConstant) = ParseSide(leftSpan);
        var rightSpan = equation.AsSpan(separator + 1);
        var (rightCoefficient, rightConstant) = ParseSide(rightSpan);

        var coefficientX = leftCoefficient - rightCoefficient;
        var constant = rightConstant - leftConstant;

        if (coefficientX == 0)
        {
            return constant == 0 ? "Infinite solutions" : "No solution";
        }

        return $"x={constant / coefficientX}";
    }

    private sealed class ParseState
    {
        public int Index;
        public int Sign = 1;
        public int CoefficientX;
        public int Constant;
    }

    private static (int CoefficientX, int Constant) ParseSide(ReadOnlySpan<char> side)
    {
        var state = new ParseState();

        while (state.Index < side.Length)
        {
            ParseNextToken(side, state);
        }

        return (state.CoefficientX, state.Constant);
    }

    private static void ParseNextToken(ReadOnlySpan<char> side, ParseState state)
    {
        if (TryConsumeSign(side, state))
        {
            return;
        }

        ConsumeTerm(side, state);
    }

    private static bool TryConsumeSign(ReadOnlySpan<char> side, ParseState state)
    {
        if (side[state.Index] == '+')
        {
            state.Sign = 1;
            state.Index++;
            return true;
        }

        if (side[state.Index] == '-')
        {
            state.Sign = -1;
            state.Index++;
            return true;
        }

        return false;
    }

    private static void ConsumeTerm(ReadOnlySpan<char> side, ParseState state)
    {
        var start = state.Index;

        while (state.Index < side.Length && side[state.Index] != '+' && side[state.Index] != '-')
        {
            state.Index++;
        }

        var term = side[start..state.Index];
        AccumulateTerm(term, state);
    }

    private static void AccumulateTerm(ReadOnlySpan<char> term, ParseState state)
    {
        if (term[^1] == 'x')
        {
            var digits = term[..^1];
            state.CoefficientX += state.Sign * (digits.IsEmpty ? 1 : int.Parse(digits));
        }
        else
        {
            state.Constant += state.Sign * int.Parse(term);
        }
    }
}
