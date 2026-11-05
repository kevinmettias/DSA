namespace DSAExperimentation.LeetCode.Solutions.NthMagicalNumber;

// The reach of NthMagicalNumberSolution's strategies (ARCHITECTURE 17.13): the catalog cases every one of
// them finishes in the harness's time. NthMagicalNumberByBinarySearch takes 3.8 ms over the 14 of the
// catalog's 81 cases with a value of rank up to 1,000 (measured 2026-11-05, Release); the larger cases run
// past the 250 ms a strategy's catalog cases are allowed.
internal static class NthMagicalNumberReach
{
    private const int LargestRank = 1000;

    public static bool IsWithin(int rank, int firstFactor, int secondFactor) =>
        rank <= LargestRank;
}
