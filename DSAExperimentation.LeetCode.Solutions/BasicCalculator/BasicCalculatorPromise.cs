namespace DSAExperimentation.LeetCode.Solutions.BasicCalculator;

// LC 224's promise about its input: the expression is valid, with every parenthesis closed, and
// holds digits, '+', '-', parentheses and spaces, and no other operator. LeetCode's judge never
// poses an input that breaks it. The recursive descent and the stack scan read what is outside it
// differently, so the two part on a '*' or a parenthesis left open. Generated cases do not always
// keep the promise, so a harness that runs them asks this first.
internal static class BasicCalculatorPromise
{
    // What an expression may hold besides its digits.
    private const string Symbols = "+-() ";

    private const char Open = '(';
    private const char Close = ')';

    public static bool IsKeptBy(string expression)
        => expression.All(character => char.IsAsciiDigit(character) || Symbols.Contains(character))
            && IsBalanced(expression);

    // Read left to right, no parenthesis closes before it opens, and every one that opens closes.
    private static bool IsBalanced(string expression)
    {
        var depth = 0;

        foreach (var character in expression)
        {
            depth += character switch
            {
                Open => 1,
                Close => -1,
                _ => 0,
            };

            if (depth < 0)
            {
                return false;
            }
        }

        return depth == 0;
    }
}
