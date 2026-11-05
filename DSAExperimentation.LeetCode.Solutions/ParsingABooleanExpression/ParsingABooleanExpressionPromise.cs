namespace DSAExperimentation.LeetCode.Solutions.ParsingABooleanExpression;

// LC 1106's promise about its input: the expression is valid and follows the statement's rules. It is
// 't', 'f', '!(e)' around exactly one operand, or '&(e1,...,en)' or '|(e1,...,en)' around n >= 1
// operands split by commas, with nothing before, between or after them, in at most 2 * 10^4
// characters. LeetCode's judge never poses an input that breaks it. Outside it the two strategies
// part: under a '!' given two operands the recursive descent negates the first alone, taking the ','
// after it for the ')', while the stack scan answers true only when none is true; the recursive
// descent can run off the end of a group left open, and stops at the first whole expression where the
// stack scan answers with the last. Generated cases do not always keep the promise, so a harness that
// runs them asks this first.
internal static class ParsingABooleanExpressionPromise
{
    // The statement's bound on the expression's length.
    private const int LongestExpression = 20_000;

    private const char NotOperator = '!';
    private const char AndOperator = '&';
    private const char OrOperator = '|';
    private const char Open = '(';
    private const char Close = ')';
    private const char Separator = ',';

    // What an operand may start with, a leaf or an operator, and what it may end with, a leaf or its
    // group's ')'.
    private const string OperandStarts = "tf!&|";
    private const string OperandEnds = "tf)";

    // What may follow a whole operand inside a group: the ',' before the next one, or the ')' that
    // closes the group.
    private const string OperandFollowers = ",)";

    // The grammar rejects the empty expression on its own, so only the upper bound needs asking.
    public static bool IsKeptBy(string expression)
        => expression.Length <= LongestExpression && IsWellFormed(expression);

    // One left-to-right scan that checks each character against the one before it and against the
    // groups still open, whose operators wait on a stack. The stack stands in for recursion, so nesting
    // as deep as 2 * 10^4 characters allows costs heap, not call frames.
    private static bool IsWellFormed(string expression)
    {
        var openOperators = new Stack<char>();

        // The start admits what a '(' does: the first character of an operand.
        var previous = Open;

        foreach (var next in expression)
        {
            if (!IsAllowedAfter(previous, next) || !IsAllowedIn(openOperators, next))
            {
                return false;
            }

            TrackGroups(openOperators, previous, next);
            previous = next;
        }

        return OperandEnds.Contains(previous) && openOperators.Count == 0;
    }

    // An operand starts after a '(' or a ','; an operator is followed by its '(' and nothing else; and a
    // whole operand, a leaf or a group's ')', is followed by a ',' or a ')'. A character outside the
    // statement's alphabet is allowed nowhere.
    private static bool IsAllowedAfter(char previous, char next) => previous switch
    {
        Open or Separator => OperandStarts.Contains(next),
        NotOperator or AndOperator or OrOperator => next == Open,
        _ => OperandEnds.Contains(previous) && OperandFollowers.Contains(next),
    };

    // A ')' needs a group open to close, and a ',' a group whose operator takes more than one operand:
    // '&' or '|', never '!'.
    private static bool IsAllowedIn(Stack<char> openOperators, char next) => next switch
    {
        Close => openOperators.Count > 0,
        Separator => openOperators.TryPeek(out var innermost) && innermost != NotOperator,
        _ => true,
    };

    // A '(' opens a group under the operator before it, and a ')' closes the innermost one.
    private static void TrackGroups(Stack<char> openOperators, char previous, char next)
    {
        if (next == Open)
        {
            openOperators.Push(previous);
        }
        else if (next == Close)
        {
            openOperators.Pop();
        }
    }
}
