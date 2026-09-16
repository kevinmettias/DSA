namespace DSAExperimentation.LeetCode.SolveTheEquation;

// LeetCode 640. Solve the Equation: parse each side of "Ax+B=Cx+D" into a running
// (coefficient of x, constant) pair via one left-to-right scan (sign, then digits,
// then an optional trailing 'x'), then combine the two sides and solve the single
// resulting linear equation. No repo container or algorithm primitive applies to a
// single closed-form parse-then-combine problem like this - the same "lighter
// repo-primitive fit" case as Complex Number Multiplication (LC 537).
//
// The combine step is already O(1) closed-form arithmetic with no naive-vs-optimal
// algorithmic split - the real cost lives entirely in scanning the equation
// string, so the two strategies below differ only in parsing: SolveBySubstringParse
// is the straightforward allocating baseline - it slices each term out as its own
// heap-allocated System.String via string.Substring before parsing it;
// SolveBySpanParse instead walks the same term boundaries over a
// ReadOnlySpan<char> and calls int.Parse directly on the slice, allocating nothing
// per term.
internal static class SolveTheEquationSolution
{
    private const string InfiniteSolutionsMessage = "Infinite solutions";
    private const string NoSolutionMessage = "No solution";

    // The textbook answer: string.Substring slices each term out as its own
    // heap-allocated string before parsing it. Deliberately written without this
    // repo's primitives - it is the arm the span-based strategy below has to
    // justify itself against.
    public static string SolveBySubstringParse(string equation)
    {
        var separator = equation.IndexOf('=');
        var (leftCoefficient, leftConstant) = ParseSideSubstring(equation[..separator]);
        var (rightCoefficient, rightConstant) = ParseSideSubstring(equation[(separator + 1)..]);
        return Combine(leftCoefficient, leftConstant, rightCoefficient, rightConstant);
    }

    private static (int CoefficientX, int Constant) ParseSideSubstring(string side)
    {
        var state = new EquationParseState(0, 0, 1, 0);

        while (state.Index < side.Length)
        {
            state = ConsumeTermSubstring(side, state);
        }

        return (state.CoefficientX, state.Constant);
    }

    private static EquationParseState ConsumeTermSubstring(string side, EquationParseState state)
    {
        var i = state.Index;

        if (TryConsumeSign(side[i], out var sign))
        {
            return state with { Sign = sign, Index = i + 1 };
        }

        var end = FindTermEnd(side, i);
        var term = side.Substring(i, end - i);
        var (kind, value) = ParseTermValue(term);

        return ApplyTerm(state, kind, value, end);
    }

    // Walks the same term boundaries over a ReadOnlySpan<char> and parses
    // directly from the slice, allocating nothing per term.
    public static string SolveBySpanParse(string equation)
    {
        var separator = equation.IndexOf('=');
        var leftSpan = equation.AsSpan(0, separator);
        var (leftCoefficient, leftConstant) = ParseSideSpan(leftSpan);
        var rightSpan = equation.AsSpan(separator + 1);
        var (rightCoefficient, rightConstant) = ParseSideSpan(rightSpan);
        return Combine(leftCoefficient, leftConstant, rightCoefficient, rightConstant);
    }

    private static (int CoefficientX, int Constant) ParseSideSpan(ReadOnlySpan<char> side)
    {
        var state = new EquationParseState(0, 0, 1, 0);

        while (state.Index < side.Length)
        {
            state = ConsumeTermSpan(side, state);
        }

        return (state.CoefficientX, state.Constant);
    }

    private static EquationParseState ConsumeTermSpan(ReadOnlySpan<char> side, EquationParseState state)
    {
        var i = state.Index;

        if (TryConsumeSign(side[i], out var sign))
        {
            return state with { Sign = sign, Index = i + 1 };
        }

        var end = FindTermEnd(side, i);
        var term = side[i..end];
        var (kind, value) = ParseTermValue(term);

        return ApplyTerm(state, kind, value, end);
    }

    private static string Combine(int leftCoefficient, int leftConstant, int rightCoefficient, int rightConstant)
    {
        var coefficientX = leftCoefficient - rightCoefficient;
        var constant = rightConstant - leftConstant;

        if (coefficientX == 0)
        {
            return constant == 0 ? InfiniteSolutionsMessage : NoSolutionMessage;
        }

        return $"x={constant / coefficientX}";
    }

    private static bool TryConsumeSign(char character, out int sign)
    {
        if (character == '+')
        {
            sign = 1;
            return true;
        }

        if (character == '-')
        {
            sign = -1;
            return true;
        }

        sign = 0;
        return false;
    }

    private static int FindTermEnd(ReadOnlySpan<char> side, int start)
    {
        var i = start;

        while (IsInsideTerm(side, i))
        {
            i++;
        }

        return i;
    }

    // A term's text runs up to the next sign, or to the end of the side it sits on.
    private static bool IsInsideTerm(ReadOnlySpan<char> side, int index) =>
        index < side.Length && side[index] != '+' && side[index] != '-';

    private static (TermKind Kind, int Value) ParseTermValue(ReadOnlySpan<char> term)
    {
        if (term[^1] == 'x')
        {
            var digits = term[..^1];
            return (TermKind.Coefficient, digits.IsEmpty ? 1 : int.Parse(digits));
        }

        return (TermKind.Constant, int.Parse(term));
    }

    private static EquationParseState ApplyTerm(EquationParseState state, TermKind kind, int value, int newIndex)
    {
        var coefficientX = state.CoefficientX;
        var constant = state.Constant;

        if (kind == TermKind.Coefficient)
        {
            coefficientX += state.Sign * value;
        }
        else
        {
            constant += state.Sign * value;
        }

        return state with { CoefficientX = coefficientX, Constant = constant, Index = newIndex };
    }

    private readonly record struct EquationParseState(int CoefficientX, int Constant, int Sign, int Index);

    // What a parsed term contributes to: Coefficient adds to the running coefficient of x,
    // Constant adds to the running constant. The sign applied is the state's, not the term's.
    private enum TermKind
    {
        Coefficient,
        Constant,
    }
}
