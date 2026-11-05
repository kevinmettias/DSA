namespace DSAExperimentation.LeetCode.Solutions.KthSymbolInGrammar;

// LC 779's promise about its input: k is a position within row n, which holds 2^(n - 1) symbols. LeetCode's
// judge never poses an input that breaks it. Past the row's end the row-expansion arm reads beyond the row it
// built and throws, while the recursive halving answers for a position the grammar never writes. Generated
// cases do not always keep the promise, so a harness that runs them asks this first.
internal static class KthSymbolInGrammarPromise
{
    public static bool IsKeptBy(int rowNumber, int symbolIndex)
    {
        var rowLength = 1L << (rowNumber - 1);

        return symbolIndex >= 1 && symbolIndex <= rowLength;
    }
}
