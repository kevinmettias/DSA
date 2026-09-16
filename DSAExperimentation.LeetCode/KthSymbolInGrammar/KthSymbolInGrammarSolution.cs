using DSAExperimentation.DataStructures;

namespace DSAExperimentation.LeetCode.KthSymbolInGrammar;

// LeetCode 779. K-th Symbol in Grammar: row 1 is "0", and every later row replaces
// each 0 with "01" and each 1 with "10". Report the symbol at symbolIndex
// (1-indexed) in the rowNumber-th row.
//
// The two strategies differ in how much of the table they are willing to build:
// RowExpansion materializes all 2^(rowNumber-1) symbols and indexes into them, while
// RecursiveHalving never holds more than one symbol per row, walking straight to
// symbolIndex's ancestor - symbolIndex's parent in the previous row is at
// (symbolIndex+1)/2, and symbolIndex picks up a bit flip exactly when it lands in the
// second half of that parent's expansion.
//
// No repo container or algorithm primitive applies here - there is nothing to compose
// over one running (row, index) pair, the same "lighter repo-primitive fit" case
// Pow(x, n)'s exponentiation by squaring already is.
internal static class KthSymbolInGrammarSolution
{
    // Baseline: build every row in full, then index the requested position. O(2^n)
    // time and space - what you would write without thinking about the recurrence.
    // Deliberately BCL-only internals (§17.5).
    public static int KthGrammarByRowExpansion(int rowNumber, int symbolIndex)
    {
        var row = new List<char> { '0' };

        for (var level = 1; level < rowNumber; level++)
        {
            row = ExpandRow(row);
        }

        return row[symbolIndex - 1] - '0';
    }

    private static List<char> ExpandRow(List<char> row)
    {
        var next = new List<char>(row.Count * AlgorithmConstants.BranchingFactor);

        foreach (var symbol in row)
        {
            if (symbol == '0')
            {
                next.Add('0');
                next.Add('1');
            }
            else
            {
                next.Add('1');
                next.Add('0');
            }
        }

        return next;
    }

    // Walk symbolIndex up to row 1 one level at a time, flipping whenever symbolIndex
    // sits in the second half of its parent's two-symbol expansion. O(n) time,
    // O(1) symbols held.
    public static int KthGrammarByRecursiveHalving(int rowNumber, int symbolIndex)
    {
        if (rowNumber == 1)
        {
            return 0;
        }

        var parent = KthGrammarByRecursiveHalving(
            rowNumber - 1, (symbolIndex + 1) / AlgorithmConstants.BranchingFactor);
        var isSecondHalfOfParent = symbolIndex % AlgorithmConstants.BranchingFactor == 0;

        return isSecondHalfOfParent ? FlippedSymbol(parent) : parent;
    }

    // The second symbol of a parent's expansion is its first symbol's complement.
    private static int FlippedSymbol(int parent) => 1 - parent;
}
