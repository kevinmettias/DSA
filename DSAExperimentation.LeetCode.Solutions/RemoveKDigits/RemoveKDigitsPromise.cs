namespace DSAExperimentation.LeetCode.Solutions.RemoveKDigits;

// LC 402's promise about its input: k is at most the number's digit count, and the number has no
// leading zero. LeetCode's judge never poses an input that breaks it. Removing more digits than
// there are reads before the start. Generated cases do not always keep the promise, so a harness
// that runs them asks this first.
internal static class RemoveKDigitsPromise
{
    public static bool IsKeptBy(string num, int removalCount)
        => removalCount >= 1 && removalCount <= num.Length && (num.Length == 1 || num[0] != '0');
}
