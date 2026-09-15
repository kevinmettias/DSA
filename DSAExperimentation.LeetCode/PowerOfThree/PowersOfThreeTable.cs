namespace DSAExperimentation.LeetCode.PowerOfThree;

// The closed set of every power of three representable in an int, ascending: 3^0
// through 3^19, since 3^20 = 3_486_784_401 overflows int.MaxValue. Already sorted,
// so it is a BinarySearch.Find target as-is.
//
// Owned here rather than in PowerOfThreeSolution because the binary-search benchmark
// reads it by name to build its prepared input, so it is reached from outside the
// declaring type - and because it is meaningful to this one problem's 32-bit bound
// and nowhere else.
internal static class PowersOfThreeTable
{
    public static readonly int[] Powers =
    [
        1,
        3,
        9,
        27,
        81,
        243,
        729,
        2_187,
        6_561,
        19_683,
        59_049,
        177_147,
        531_441,
        1_594_323,
        4_782_969,
        14_348_907,
        43_046_721,
        129_140_163,
        387_420_489,
        1_162_261_467,
    ];
}
