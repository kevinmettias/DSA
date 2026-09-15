using ParserStack = DSAExperimentation.DataStructures.Stack.Stack<char>;

namespace DSAExperimentation.LeetCode.ParsingABooleanExpression;

// LeetCode 1106. Parsing A Boolean Expression: evaluate a fully-parenthesised
// expression over 't'/'f' leaves and the operators '!', '&' and '|'.
//
// The two strategies differ only in which stack holds the pending operands: the
// CLR's own call stack, walked by a recursive-descent parser, or this repo's
// Stack<char> driven iteratively by a single left-to-right character scan.
internal static class ParsingABooleanExpressionSolution
{
    private const char TrueToken = 't';
    private const char FalseToken = 'f';
    private const char NotOperator = '!';
    private const char AndOperator = '&';

    // Length of an operator character plus the '(' that always follows it -
    // consumed together by the recursive-descent parser.
    private const int OperatorAndParenLength = 2;

    // The textbook answer: one recursive procedure per sub-expression, leaning on
    // the CLR call stack and nothing else. Deliberately written without this
    // repo's primitives - it is the arm the composed solution has to beat.
    public static bool ParseBoolExprByRecursiveDescent(string expression)
    {
        var index = 0;
        return ParseExpression(expression, ref index);
    }

    // The explicit-stack answer: push characters as they are read and collapse the
    // innermost group on every ')', so an expression of any nesting depth costs a
    // single pass and no recursion.
    public static bool ParseBoolExprByParserStack(string expression)
    {
        var stack = new ParserStack();

        foreach (var c in expression)
        {
            if (c == ',')
            {
                continue;
            }

            if (c != ')')
            {
                stack.Push(c);
                continue;
            }

            stack.Push(EvaluateGroup(stack));
        }

        stack.TryPop(out var result);
        return result == TrueToken;
    }

    // Pops the innermost group's operands and the operator beneath its '(',
    // returning the single token the whole group collapses to.
    private static char EvaluateGroup(ParserStack stack)
    {
        var (trueCount, falseCount) = CountOperands(stack);
        var op = ConsumeGroupOperator(stack);
        return EvaluateOperator(op, trueCount, falseCount) ? TrueToken : FalseToken;
    }

    // Pops every operand of the innermost group (down to, but not including, its
    // opening '(') and tallies how many were true vs. false.
    private static (int TrueCount, int FalseCount) CountOperands(ParserStack stack)
    {
        var trueCount = 0;
        var falseCount = 0;

        while (stack.TryPeek(out var top) && top != '(')
        {
            stack.TryPop(out var operand);
            if (operand == TrueToken)
            {
                trueCount++;
            }
            else
            {
                falseCount++;
            }
        }

        return (trueCount, falseCount);
    }

    // Pops the group's opening '(' and the operator character below it.
    private static char ConsumeGroupOperator(ParserStack stack)
    {
        stack.TryPop(out _); // the matching '('
        stack.TryPop(out var op);
        return op;
    }

    // Every operator here is decided by counts alone: '!' is true when its single
    // operand was false, '&' when nothing was false, '|' when anything was true.
    private static bool EvaluateOperator(char op, int trueCount, int falseCount) => op switch
    {
        NotOperator => trueCount == 0,
        AndOperator => falseCount == 0,
        _ => trueCount > 0, // '|'
    };

    // Consumes one sub-expression starting at index, advancing index past it.
    private static bool ParseExpression(string expression, ref int index)
    {
        var c = expression[index];

        if (c is TrueToken or FalseToken)
        {
            index++;
            return c == TrueToken;
        }

        index += OperatorAndParenLength;

        return c == NotOperator
            ? ParseNot(expression, ref index)
            : ParseOperands(expression, c, ref index);
    }

    // Consumes the single operand of a unary '!' and its closing ')'.
    private static bool ParseNot(string expression, ref int index)
    {
        var value = ParseExpression(expression, ref index);
        index++; // consume ')'
        return !value;
    }

    // Consumes the comma-separated operands of an n-ary '&'/'|' up to its closing
    // ')', folding them with the operator as they are parsed.
    private static bool ParseOperands(string expression, char op, ref int index)
    {
        var result = op == AndOperator;

        while (expression[index] != ')')
        {
            var operand = ParseExpression(expression, ref index);
            result = op == AndOperator ? result && operand : result || operand;

            if (expression[index] == ',')
            {
                index++;
            }
        }

        index++; // consume ')'
        return result;
    }
}
