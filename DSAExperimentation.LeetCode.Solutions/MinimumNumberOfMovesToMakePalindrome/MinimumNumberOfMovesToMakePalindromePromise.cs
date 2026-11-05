namespace DSAExperimentation.LeetCode.Solutions.MinimumNumberOfMovesToMakePalindrome;

// LC 2193's promise about its input: "s can always be converted to a palindrome", so at most one letter
// appears an odd number of times - the one that ends in the middle. LeetCode's judge never poses an input
// that breaks it. Both strategies nudge an unmatched letter toward the middle and retry, so a second
// unmatched letter sends them round forever. Generated cases do not always keep the promise, so a harness
// that runs them asks this first.
internal static class MinimumNumberOfMovesToMakePalindromePromise
{
    // The letters a palindrome may leave unpaired: the one in its middle, when its length is odd.
    private const int MostUnpairedLetters = 1;

    public static bool IsKeptBy(string text) =>
        text.GroupBy(letter => letter).Count(occurrences => occurrences.Count() % 2 == 1) <= MostUnpairedLetters;
}
