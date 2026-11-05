using DSAExperimentation.Domain.Mathematics.Numerals;

namespace DSAExperimentation.LeetCode.Solutions.SortingTheSentence;

// LC 1859's promise about its input: the sentence is shuffled from one whose words were numbered from 1, so
// the digits that end its words are 1 to n, each once. LeetCode's judge never poses an input that breaks
// it. Both strategies place a word by its digit, so a word numbered 0 lands before the first. Generated
// cases do not always keep the promise, so a harness that runs them asks this first.
internal static class SortingTheSentencePromise
{
    private const char WordSeparator = ' ';

    public static bool IsKeptBy(string sentence)
    {
        var words = sentence.Split(WordSeparator);
        var positions = words.Select(word => DecimalDigits.TryIndexOf(word[^1], out var digit) ? digit : -1);
        var numberedFromOne = Enumerable.Range(1, words.Length);
        return positions.Order().SequenceEqual(numberedFromOne);
    }
}
