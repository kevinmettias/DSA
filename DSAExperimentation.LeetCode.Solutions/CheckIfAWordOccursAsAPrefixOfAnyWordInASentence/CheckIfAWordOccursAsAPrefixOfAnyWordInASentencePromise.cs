namespace DSAExperimentation.LeetCode.Solutions.CheckIfAWordOccursAsAPrefixOfAnyWordInASentence;

// LC 1455's promise about its input: the sentence holds lowercase English letters and spaces, and the
// searched word lowercase letters only. LeetCode's judge never poses an input that breaks it. The trie arm
// keeps a slot per lowercase letter, so a generated sentence holding "c++ c#" throws there while the
// StartsWith scan answers. Generated cases do not always keep the promise, so a harness that runs them asks
// this first.
internal static class CheckIfAWordOccursAsAPrefixOfAnyWordInASentencePromise
{
    private const char WordSeparator = ' ';

    public static bool IsKeptBy(
        CheckIfAWordOccursAsAPrefixOfAnyWordInASentenceSolution.SentenceText sentence,
        CheckIfAWordOccursAsAPrefixOfAnyWordInASentenceSolution.SearchedPrefix searchWord)
        => sentence.Text.All(character => char.IsAsciiLetterLower(character) || character == WordSeparator)
            && searchWord.Text.All(char.IsAsciiLetterLower);
}
