namespace DSAExperimentation.LeetCode.Solutions.SubstringMatchingPattern;

// LC 3407's promise about its input: the pattern contains exactly one '*'. LeetCode's judge never poses an
// input that breaks it. Both strategies split the pattern at its first '*' and match the rest literally.
// Generated cases do not always keep the promise, so a harness that runs them asks this first.
internal static class SubstringMatchingPatternPromise
{
    private const char Star = '*';

    public static bool IsKeptBy(SubjectText subject, WildcardPattern pattern) => pattern.Text.Count(character => character == Star) == 1;
}
