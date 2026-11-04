namespace DSAExperimentation.LeetCode.Solutions.StringTransformation;

// LC 2851's promise about its input: the two strings are as long as each other. LeetCode's judge
// never poses an input that breaks it. Rotations of one string never reach a longer one, and the
// rotation compare reads past its end trying. Generated cases do not always keep the promise, so a
// harness that runs them asks this first.
internal static class StringTransformationPromise
{
    public static bool IsKeptBy(string source, string target, long operations) => source.Length == target.Length;
}
