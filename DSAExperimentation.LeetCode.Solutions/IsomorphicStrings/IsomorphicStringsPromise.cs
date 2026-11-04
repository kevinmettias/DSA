namespace DSAExperimentation.LeetCode.Solutions.IsomorphicStrings;

// LC 205's promise about its input: the two strings are as long as each other, and both are ASCII.
// LeetCode's judge never poses an input that breaks it. The strategies index both strings by one
// position and a table by character, so a longer string or a wider character is read past an end.
// Generated cases do not always keep the promise, so a harness that runs them asks this first.
internal static class IsomorphicStringsPromise
{
    public static bool IsKeptBy(string source, string target)
        => source.Length == target.Length && source.Concat(target).All(char.IsAscii);
}
