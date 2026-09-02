using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.PowerOfThree;

// LeetCode 326. Power of Three: is n exactly 3^k for some non-negative integer k?
//
// The naive strategy divides out factors of three one at a time and checks what
// survives. The binary-search strategy reframes the question as membership in the
// closed set of powers of three that fit in a 32-bit int (3^0..3^19 - 3^20
// overflows): precompute that sorted table once, then this repo's own
// BinarySearch.Find over it in place of a division loop.
internal static class PowerOfThreeSolution
{
    private const int PowerBase = 3;

    // The closed set of every power of three representable in an int, ascending -
    // already sorted, so it is a BinarySearch.Find target as-is. Owned here because
    // it is meaningful to this one problem's 32-bit bound and nowhere else.
    internal static readonly int[] PowersOfThree = BuildPowersOfThree();

    // The textbook approach: repeatedly divide out factors of three, then check
    // whether the survivor is 1. Written without this repo's primitives - the arm
    // the binary search strategy below has to justify itself against.
    public static bool IsPowerOfThreeByDivisionLoop(int n)
    {
        if (n < 1)
        {
            return false;
        }

        while (n % PowerBase == 0)
        {
            n /= PowerBase;
        }

        return n == 1;
    }

    // LeetCode's own shape: build the sorted sequence, then search it.
    public static bool IsPowerOfThreeByBinarySearch(int n) =>
        IsPowerOfThreeByBinarySearch(n, new ArraySequence<int>(PowersOfThree));

    // Prepared-input overload: the caller already wrapped PowersOfThree, so no
    // construction is charged to the measured search.
    public static bool IsPowerOfThreeByBinarySearch(int n, ArraySequence<int> powersOfThree) =>
        BinarySearch.Find<int, ArraySequence<int>>(powersOfThree, n) is not null;

    private static int[] BuildPowersOfThree()
    {
        var powers = new List<int>();
        long power = 1;

        while (power <= int.MaxValue)
        {
            powers.Add((int)power);
            power *= PowerBase;
        }

        return [.. powers];
    }
}
