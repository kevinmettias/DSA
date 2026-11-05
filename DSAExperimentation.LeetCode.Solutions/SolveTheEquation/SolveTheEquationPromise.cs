namespace DSAExperimentation.LeetCode.Solutions.SolveTheEquation;

// LC 640's promise about its input: when the equation has exactly one solution, x is an integer.
// LeetCode's judge never poses an input that breaks it. Both strategies divide in integers, which
// truncates a fractional x. Generated cases do not always keep the promise, so a harness that runs them
// asks this first. It reads each term on its own, rather than through the strategies' parse, so a
// fault in that parse cannot hide the case that would show it.
internal static class SolveTheEquationPromise
{
    private const char EqualsSign = '=';
    private const char Plus = '+';
    private const char Variable = 'x';
    private const string Minus = "-";

    // A minus with a plus before it, so splitting at each plus keeps every term's own sign.
    private const string SignedMinus = "+-";

    // Written as coefficient * x + constant = 0, x is -constant / coefficient.
    public static bool IsKeptBy(string equation)
    {
        var (coefficient, constant) = MovedToTheLeft(equation);
        return coefficient == 0 || constant % coefficient == 0;
    }

    // The equation as coefficient * x + constant = 0: the right side's totals taken from the left's.
    private static (int Coefficient, int Constant) MovedToTheLeft(string equation)
    {
        var sides = equation.Split(EqualsSign);
        var (left, right) = (TotalsOf(sides[0]), TotalsOf(sides[1]));
        return (left.Coefficient - right.Coefficient, left.Constant - right.Constant);
    }

    // The side's coefficients of x summed, and its constants summed. A bare x, or -x, counts one x.
    private static (int Coefficient, int Constant) TotalsOf(string side)
    {
        var (coefficient, constant) = (0, 0);

        foreach (var term in side.Replace(Minus, SignedMinus).Split(Plus, StringSplitOptions.RemoveEmptyEntries))
        {
            var value = term.TrimEnd(Variable) switch
            {
                { Length: 0 } => 1,
                Minus => -1,
                var digits => int.Parse(digits),
            };

            if (term[^1] == Variable)
            {
                coefficient += value;
            }
            else
            {
                constant += value;
            }
        }

        return (coefficient, constant);
    }
}
