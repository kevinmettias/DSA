namespace DSAExperimentation.LeetCode.Solutions.MinimumNumberOfSwapsToMakeTheStringBalanced;

// LC 1963's promise about its input: the string holds as many '[' as ']'. LeetCode's judge never
// poses an input that breaks it. With brackets left over no swaps balance it, and the strategies
// count the surplus differently. Generated cases do not always keep the promise, so a harness that
// runs them asks this first.
internal static class MinimumNumberOfSwapsToMakeTheStringBalancedPromise
{
    private const char Opener = '[';
    private const char Closer = ']';

    public static bool IsKeptBy(string text) => text.Count(bracket => bracket == Opener) == text.Count(bracket => bracket == Closer);
}
