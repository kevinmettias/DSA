namespace DSAExperimentation.LeetCode.Solutions.CheckIfDfsStringsArePalindromes;

// LC 3327's promise about its input: the string gives one character to every node of the tree.
// LeetCode's judge never poses an input that breaks it. A node with no character is read past the
// string's end. Generated cases do not always keep the promise, so a harness that runs them asks
// this first.
internal static class CheckIfDfsStringsArePalindromesPromise
{
    public static bool IsKeptBy(int[] parent, string nodeCharacters) => parent.Length == nodeCharacters.Length;
}
