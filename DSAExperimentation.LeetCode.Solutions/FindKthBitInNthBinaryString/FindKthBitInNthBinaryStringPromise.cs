namespace DSAExperimentation.LeetCode.Solutions.FindKthBitInNthBinaryString;

// LC 1545's promise about its input: the bit asked for is one of the 2^n - 1 the string holds.
// LeetCode's judge never poses an input that breaks it. Building the string and reading position k
// reads past its end. Generated cases do not always keep the promise, so a harness that runs them
// asks this first.
internal static class FindKthBitInNthBinaryStringPromise
{
    public static bool IsKeptBy(int stringOrder, int bitPosition) => bitPosition >= 1 && bitPosition < 1 << stringOrder;
}
