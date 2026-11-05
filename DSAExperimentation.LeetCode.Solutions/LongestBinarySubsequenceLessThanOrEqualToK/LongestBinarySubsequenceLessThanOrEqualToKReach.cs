namespace DSAExperimentation.LeetCode.Solutions.LongestBinarySubsequenceLessThanOrEqualToK;

// The reach of LongestBinarySubsequenceLessThanOrEqualToKSolution's strategies (ARCHITECTURE 17.13): the
// catalog cases every one of them finishes in the harness's time. LongestSubsequenceBySubsetEnumeration takes
// 199.5 ms over the 32 of the catalog's 92 cases with a length of bits up to 20 (measured 2026-11-05,
// Release); the larger cases run past the 250 ms a strategy's catalog cases are allowed.
internal static class LongestBinarySubsequenceLessThanOrEqualToKReach
{
    private const int LargestBitsLength = 20;

    public static bool IsWithin(string bits, int maxValue) =>
        bits.Length <= LargestBitsLength;
}
