namespace DSAExperimentation.LeetCode.Solutions.EvaluateReversePolishNotation;

// LC 150's promise about its input: the tokens are one valid expression: every operator finds two
// operands, and one value is left. LeetCode's judge never poses an input that breaks it. Given
// operands left over, each strategy answers with a different one of them. Generated cases do not
// always keep the promise, so a harness that runs them asks this first.
internal static class EvaluateReversePolishNotationPromise
{
    private const string Operators = "+-*/";
    private const int OperandsPerOperator = 2;

    public static bool IsKeptBy(string[] tokens)
    {
        var depth = 0;

        foreach (var token in tokens)
        {
            if (!IsOperator(token))
            {
                depth++;
            }
            else if (depth < OperandsPerOperator)
            {
                return false;
            }
            else
            {
                depth--;
            }
        }

        return depth == 1;
    }

    private static bool IsOperator(string token) => token.Length == 1 && Operators.Contains(token[0]);
}
