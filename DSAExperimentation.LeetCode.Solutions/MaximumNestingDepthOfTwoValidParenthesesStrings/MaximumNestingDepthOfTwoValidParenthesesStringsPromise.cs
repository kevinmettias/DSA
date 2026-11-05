namespace DSAExperimentation.LeetCode.Solutions.MaximumNestingDepthOfTwoValidParenthesesStrings;

// LC 1111's promise about its input: the sequence is a valid parentheses string. LeetCode's judge never
// poses an input that breaks it. On a sequence left open, no split makes two valid strings, and the depth
// rescan marks a closing parenthesis it cannot place -1. Generated cases do not always keep the promise,
// so a harness that runs them asks this first.
internal static class MaximumNestingDepthOfTwoValidParenthesesStringsPromise
{
    public static bool IsKeptBy(string seq)
    {
        var depth = 0;

        foreach (var character in seq)
        {
            depth += character == MaximumNestingDepthOfTwoValidParenthesesStringsAnswerCheck.Open ? 1 : -1;

            if (depth < 0)
            {
                return false;
            }
        }

        return depth == 0;
    }
}
