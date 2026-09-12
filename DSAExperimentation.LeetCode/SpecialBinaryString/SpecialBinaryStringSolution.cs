using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.SpecialBinaryString;

// LeetCode 761. Special Binary String: split into maximal special substrings at each
// balance-zero point, recurse on each substring's interior, then sort the resulting
// sibling pieces descending. Swapping two consecutive special substrings to maximize
// the string is exactly what a descending sort of same-level sibling pieces produces.
//
// Both strategies do the identical split-and-recurse walk (SplitIntoPieces is shared,
// non-differentiating scaffolding, the same role FindEmptyCell/IsValidPlacement play
// across SudokuSolverBenchmarks' strategies) and differ only in how each level's
// sibling pieces get sorted descending.
internal static class SpecialBinaryStringSolution
{
    private const int MinimalSpecialStringLength = 2;

    private static readonly Comparer<string> Descending =
        Comparer<string>.Create((a, b) => string.CompareOrdinal(b, a));

    // The textbook answer: the BCL's Array.Sort for each level's sibling pieces.
    // Deliberately written without this repo's primitives - it is the arm the
    // composed strategy below has to justify itself against.
    public static string MakeLargestSpecialByArraySort(string s)
    {
        if (s.Length <= MinimalSpecialStringLength)
        {
            return s;
        }

        var pieces = SplitIntoPieces(s, MakeLargestSpecialByArraySort).ToArray();
        Array.Sort(pieces, Descending);

        return string.Concat(pieces);
    }

    // This repo's own MergeSort.Sort over an ArrayIndexedSequence with a descending
    // comparer, in place of Array.Sort - the same composition
    // RussianDollEnvelopesTests/QueueReconstructionByHeightTests already exercise.
    public static string MakeLargestSpecialByMergeSort(string s)
    {
        if (s.Length <= MinimalSpecialStringLength)
        {
            return s;
        }

        var pieces = SplitIntoPieces(s, MakeLargestSpecialByMergeSort).ToArray();
        MergeSort.Sort<string, ArrayIndexedSequence<string>>(new ArrayIndexedSequence<string>(pieces), Descending);

        return string.Concat(pieces);
    }

    // The balance-zero split shared by both strategies: a special substring's balance
    // (1 = +1, 0 = -1) returns to zero only at its own close, so each such point marks
    // a maximal top-level piece whose interior gets recursively maximized in place.
    private static List<string> SplitIntoPieces(string s, Func<string, string> recurse)
    {
        var pieces = new List<string>();
        var balance = 0;
        var start = 0;

        for (var i = 0; i < s.Length; i++)
        {
            balance += s[i] == '1' ? 1 : -1;

            if (balance == 0)
            {
                var interior = s.Substring(start + 1, i - start - 1);
                pieces.Add("1" + recurse(interior) + "0");
                start = i + 1;
            }
        }

        return pieces;
    }
}
