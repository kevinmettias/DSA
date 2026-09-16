using RepoStack = DSAExperimentation.DataStructures.Stack.Stack<(int Result, int Sign)>;

namespace DSAExperimentation.LeetCode.BasicCalculator;

// LeetCode 224. Basic Calculator: evaluate a string expression containing
// '+', '-', '(', ')', digits and spaces - no built-in eval, no multiplication
// or division.
//
// CalculateByRecursiveDescent leans on the CLR's own call stack per '(' - the
// textbook baseline, deliberately without any repo primitive (the same
// "lighter repo-primitive fit" precedent Pow(x, n)/Rectangle Area already
// accept). CalculateByStackScan instead pushes the (result-so-far, sign)
// pair this repo's own Stack<(int,int)> is holding onto at each '(' and pops
// it back at ')' - a single left-to-right pass, no recursion.
//
// Migration note: the pre-migration benchmark's RecursiveDescent arm treated
// any character that was not a digit, '(' or ')' as an operator-sign token -
// ' ' fell into that branch too and silently flipped the running sign
// negative. Nothing caught it because the benchmark's generated workload
// never contains a space, but LeetCode's own examples do (" 2-1 + 2 ").
// Bringing the baseline under test (ARCHITECTURE.md 17.3) surfaced it;
// fixed here by skipping whitespace explicitly, which is what
// CalculateByStackScan already did via its switch's implicit default case.
internal static class BasicCalculatorSolution
{
    private const int DecimalBase = 10;

    public static int CalculateByRecursiveDescent(string expression)
    {
        var position = 0;
        return EvaluateRecursive(expression, ref position);
    }

    public static int CalculateByStackScan(string expression)
    {
        var stack = new RepoStack();
        var state = (Result: 0, Sign: 1);
        var position = 0;

        while (position < expression.Length)
        {
            ApplyToken(expression, stack, ref position, ref state);
        }

        return state.Result;
    }

    private static void ApplyToken(string expression, RepoStack stack, ref int position, ref (int Result, int Sign) state)
    {
        var c = expression[position];

        if (char.IsDigit(c))
        {
            state.Result += state.Sign * ParseNumber(expression, ref position);
            return;
        }

        ApplyOperatorToken(c, stack, ref state);
        position++;
    }

    private static void ApplyOperatorToken(char token, RepoStack stack, ref (int Result, int Sign) state)
    {
        switch (token)
        {
            case '+':
                state.Sign = 1;
                break;
            case '-':
                state.Sign = -1;
                break;
            case '(':
                stack.Push(state);
                state = (0, 1);
                break;
            case ')':
                stack.TryPop(out var outer);
                state.Result = outer.Result + outer.Sign * state.Result;
                break;
        }
    }

    private static int EvaluateRecursive(string expression, ref int position)
    {
        var result = 0;
        var sign = 1;

        while (position < expression.Length)
        {
            if (!TryConsumeToken(expression, ref position, ref result, ref sign))
            {
                return result;
            }
        }

        return result;
    }

    private static bool TryConsumeToken(string expression, ref int position, ref int result, ref int sign)
    {
        if (TryConsumeValue(expression, ref position, ref result, ref sign))
        {
            return true;
        }

        var c = expression[position];

        if (c == ')')
        {
            return false;
        }

        if (c != ' ')
        {
            sign = ApplyOperatorSign(c);
        }

        position++;
        return true;
    }

    // Digits and parenthesized groups are the two operands this grammar has, and
    // both fold into the running result through the sign currently pending.
    private static bool TryConsumeValue(string expression, ref int position, ref int result, ref int sign)
    {
        var c = expression[position];

        if (char.IsDigit(c))
        {
            result += sign * ParseNumber(expression, ref position);
            return true;
        }

        if (c != '(')
        {
            return false;
        }

        result += sign * ConsumeGroup(expression, ref position);
        return true;
    }

    private static int ConsumeGroup(string expression, ref int position)
    {
        position++;
        var value = EvaluateRecursive(expression, ref position);
        position++;
        return value;
    }

    private static int ApplyOperatorSign(char token) => token == '+' ? 1 : -1;

    private static int ParseNumber(string expression, ref int position)
    {
        var number = 0;

        while (position < expression.Length && char.IsDigit(expression[position]))
        {
            number = number * DecimalBase + (expression[position] - '0');
            position++;
        }

        return number;
    }
}
