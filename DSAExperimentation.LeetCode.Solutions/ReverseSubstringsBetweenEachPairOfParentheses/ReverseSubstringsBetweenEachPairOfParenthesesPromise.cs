namespace DSAExperimentation.LeetCode.Solutions.ReverseSubstringsBetweenEachPairOfParentheses;

// LC 1190's promise about its input: every parenthesis is balanced. LeetCode's judge never poses an
// input that breaks it. An unclosed parenthesis is kept by the splicing strategy and dropped by the
// stack. Generated cases do not always keep the promise, so a harness that runs them asks this
// first.
internal static class ReverseSubstringsBetweenEachPairOfParenthesesPromise
{
    public static bool IsKeptBy(string text)
    {
        var depth = 0;

        foreach (var character in text)
        {
            depth += character switch
            {
                '(' => 1,
                ')' => -1,
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
