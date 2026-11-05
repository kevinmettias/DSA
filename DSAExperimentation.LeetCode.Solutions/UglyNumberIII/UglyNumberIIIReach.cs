namespace DSAExperimentation.LeetCode.Solutions.UglyNumberIII;

// The reach of UglyNumberIIISolution's strategies (ARCHITECTURE 17.13): the catalog cases every one of them
// finishes in the harness's time. NthUglyNumberByCountScan takes 230.2 ms over the 73 of the catalog's 131
// cases with a value of rank up to 500,000 and a value of firstFactor up to 123 (measured 2026-11-05,
// Release); the larger cases run past the 250 ms a strategy's catalog cases are allowed.
internal static class UglyNumberIIIReach
{
    private const int LargestRank = 500_000;
    private const int LargestFirstFactor = 123;

    public static bool IsWithin(int rank, int firstFactor, int secondFactor, int thirdFactor) =>
        rank <= LargestRank && firstFactor <= LargestFirstFactor;
}
