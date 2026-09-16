using DSAExperimentation.DataStructures;

namespace DSAExperimentation.LeetCode.FindKthBitInNthBinaryString;

// LeetCode 1545. Find Kth Bit in Nth Binary String, over the construction rule
// S(n) = S(n-1) + "1" + invert(reverse(S(n-1))), S(1) = "0".
//
// The baseline materializes S(n) literally and indexes it - O(2^n) time and space,
// since |S(n)| = 2^n - 1. The bisection never builds the string: the middle bit of
// S(n) is always '1', a bitPosition left of the middle is the same bit at that
// position in S(n-1), and a bitPosition right of it mirrors to position
// (length - bitPosition + 1) in S(n-1) with the result inverted.
//
// Each bisection call makes exactly one recursive call, never two, so there are no
// overlapping subproblems for this repo's own Memoizer to help with - the same
// "lighter repo-primitive fit" case as PowXn.
internal static class FindKthBitInNthBinaryStringSolution
{
    private const string BaseCaseBit = "0"; // S(1)
    private const string MiddleBitSeparator = "1";

    // Baseline: BCL-only, builds S(n) in full and reads the bit at the requested
    // 1-based position.
    public static char FindKthBitByStringConstruction(int stringOrder, int bitPosition) =>
        BuildNthString(stringOrder)[bitPosition - 1];

    // Walks down one level per call, folding the mirror-and-invert rule into the
    // returned bit instead of the string.
    public static char FindKthBitByRecursiveBisection(int stringOrder, int bitPosition)
    {
        if (stringOrder == 1)
        {
            return BaseCaseBit[0];
        }

        var length = (1 << stringOrder) - 1;
        var mid = (length / AlgorithmConstants.HalvingFactor) + 1;

        if (bitPosition == mid)
        {
            return MiddleBitSeparator[0];
        }

        if (bitPosition < mid)
        {
            return FindKthBitByRecursiveBisection(stringOrder - 1, bitPosition);
        }

        var mirroredBit = FindKthBitByRecursiveBisection(stringOrder - 1, length - bitPosition + 1);
        return Invert(mirroredBit);
    }

    private static string BuildNthString(int stringOrder)
    {
        if (stringOrder == 1)
        {
            return BaseCaseBit;
        }

        var previous = BuildNthString(stringOrder - 1);
        var invertedReversed = new char[previous.Length];

        for (var i = 0; i < previous.Length; i++)
        {
            invertedReversed[previous.Length - 1 - i] = Invert(previous[i]);
        }

        return previous + MiddleBitSeparator + new string(invertedReversed);
    }

    private static char Invert(char bit) => IsBaseBit(bit) ? SeparatorBit() : BaseBit();

    private static bool IsBaseBit(char bit) => bit == BaseBit();

    private static char BaseBit() => BaseCaseBit[0];

    private static char SeparatorBit() => MiddleBitSeparator[0];
}
