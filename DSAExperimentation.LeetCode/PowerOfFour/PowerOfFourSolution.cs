using DSAExperimentation.Algorithms.Searching;

namespace DSAExperimentation.LeetCode.PowerOfFour;

// LeetCode 342. Power of Four: is n exactly 4^k for some non-negative integer k?
//
// The naive strategy divides out factors of four one at a time and checks what
// survives. The binary-search strategy reframes the question as membership in a
// virtual, already-sorted sequence of every power of four that fits in a 32-bit
// int (4^0..4^15 - 4^16 overflows) - this repo's own BinarySearch.Find over a
// PowersOfFourSequence instead of a division loop, the same monotone-virtual-
// sequence idiom SqrtXSolution uses for LeetCode 69.
internal static class PowerOfFourSolution
{
    private const int PowerBase = 4;

    // The textbook approach: repeatedly divide out factors of four, then check
    // whether the survivor is 1. Written without this repo's primitives - the
    // arm IsPowerOfFourByBinarySearch below has to justify itself against.
    public static bool IsPowerOfFourByDivisionLoop(int n)
    {
        if (n <= 0)
        {
            return false;
        }

        while (n % PowerBase == 0)
        {
            n /= PowerBase;
        }

        return n == 1;
    }

    // LeetCode's own shape: search the virtual sequence of powers of four
    // directly. The sequence is stateless (each Get is a single left shift), so
    // there is no construction worth hoisting into a prepared-input overload -
    // the same reasoning that leaves PowerOfTwoSolution's bit-trick arm with a
    // single overload.
    public static bool IsPowerOfFourByBinarySearch(int n) =>
        BinarySearch.Find<int, PowersOfFourSequence>(new PowersOfFourSequence(), n) is not null;
}
