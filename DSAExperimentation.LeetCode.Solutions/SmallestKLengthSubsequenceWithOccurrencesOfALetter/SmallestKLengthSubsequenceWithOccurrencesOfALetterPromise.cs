namespace DSAExperimentation.LeetCode.Solutions.SmallestKLengthSubsequenceWithOccurrencesOfALetter;

// LC 2030's promise about its input: the letter appears at least `repetition` times, and
// `repetition` is at most k, at most the string's length. LeetCode's judge never poses an input
// that breaks it. Without enough of the letter no subsequence qualifies, and the window rescan
// reads past the end. Generated cases do not always keep the promise, so a harness that runs them
// asks this first.
internal static class SmallestKLengthSubsequenceWithOccurrencesOfALetterPromise
{
    public static bool IsKeptBy(string text, int subsequenceLength, char letter, int repetition)
        => text.Count(character => character == letter) >= repetition && IsOrdered(repetition, subsequenceLength, text.Length);

    private static bool IsOrdered(int repetition, int subsequenceLength, int length)
        => repetition >= 1 && repetition <= subsequenceLength && subsequenceLength <= length;
}
