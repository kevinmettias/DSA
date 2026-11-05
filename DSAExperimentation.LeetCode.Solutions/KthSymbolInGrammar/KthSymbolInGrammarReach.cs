namespace DSAExperimentation.LeetCode.Solutions.KthSymbolInGrammar;

// The reach of KthSymbolInGrammarSolution's strategies (ARCHITECTURE 17.13): the catalog cases every one of
// them finishes in the harness's time. KthGrammarByRowExpansion builds every row up to n, doubling each time,
// and takes 25.3 ms over the 45 of the 50 cases the promise keeps with n up to 22 (measured 2026-11-05,
// Release); the cases at n = 24 add 314 ms, past the 250 ms a strategy's catalog cases are allowed.
internal static class KthSymbolInGrammarReach
{
    private const int LargestRowNumber = 22;

    public static bool IsWithin(int rowNumber, int symbolIndex) =>
        rowNumber <= LargestRowNumber;
}
