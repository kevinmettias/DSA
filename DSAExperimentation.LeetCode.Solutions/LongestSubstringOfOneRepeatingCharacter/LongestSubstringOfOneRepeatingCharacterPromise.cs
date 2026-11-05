namespace DSAExperimentation.LeetCode.Solutions.LongestSubstringOfOneRepeatingCharacter;

// LC 2213's promise about its input: there are as many query characters as query indices, k of each, and
// every index lies inside the string. LeetCode's judge never poses an input that breaks it. Both strategies
// pair the ith character with the ith index. Generated cases do not always keep the promise, so a harness
// that runs them asks this first.
internal static class LongestSubstringOfOneRepeatingCharacterPromise
{
    public static bool IsKeptBy(LongestSubstringOfOneRepeatingCharacterSolution.BaseText baseText, LongestSubstringOfOneRepeatingCharacterSolution.ReplacementCharacters queryCharacters, int[] queryIndices)
        => queryCharacters.Text.Length == queryIndices.Length && queryIndices.All(index => index >= 0 && index < baseText.Text.Length);
}
