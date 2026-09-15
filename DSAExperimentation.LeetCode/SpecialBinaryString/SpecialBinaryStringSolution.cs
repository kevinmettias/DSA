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

    // Both strategies are stateless, so one instance each serves every call and the
    // benchmark arms that measure these two methods allocate nothing to pick one.
    private static readonly ILargestSpecialString ArraySortMaximizer = new MaximizeByArraySort();
    private static readonly ILargestSpecialString MergeSortMaximizer = new MaximizeByMergeSort();

    // The textbook answer: the BCL's Array.Sort for each level's sibling pieces.
    // Deliberately written without this repo's primitives - it is the arm the
    // composed strategy below has to justify itself against.
    public static string MakeLargestSpecialByArraySort(string s) => ArraySortMaximizer.Maximize(s);

    // This repo's own MergeSort.Sort over an ArrayIndexedSequence with a descending
    // comparer, in place of Array.Sort - the same composition
    // RussianDollEnvelopesTests/QueueReconstructionByHeightTests already exercise.
    public static string MakeLargestSpecialByMergeSort(string s) => MergeSortMaximizer.Maximize(s);

    // The one question the two arms answer differently: how a special string is
    // maximized - split at its balance-zero points, maximize each piece's interior,
    // then order the sibling pieces descending. Naming that decision also gives the
    // recursion its contract: an interior is maximized by this same decision, so an
    // implementation must recurse through itself rather than through the arm whose
    // public method happened to start the walk.
    private interface ILargestSpecialString
    {
        // The maximal rearrangement of `s`, which is `s` itself once it is too short to
        // hold a pair of sibling pieces to swap.
        string Maximize(string s);
    }

    // The balance-zero split shared by both strategies: a special substring's balance
    // (1 = +1, 0 = -1) returns to zero only at its own close, so each such point marks
    // a maximal top-level piece whose interior gets recursively maximized in place.
    private static List<string> SplitIntoPieces(string s, ILargestSpecialString maximizer)
    {
        var pieces = new List<string>();
        var balance = 0;
        var start = 0;

        for (var i = 0; i < s.Length; i++)
        {
            var isOpening = s[i] == '1';
            balance += isOpening ? 1 : -1;

            if (balance == 0)
            {
                var interior = s.Substring(start + 1, i - start - 1);
                pieces.Add($"1{maximizer.Maximize(interior)}0");
                start = i + 1;
            }
        }

        return pieces;
    }

    private sealed class MaximizeByArraySort : ILargestSpecialString
    {
        public string Maximize(string s)
        {
            if (s.Length <= MinimalSpecialStringLength)
            {
                return s;
            }

            var pieces = SplitIntoPieces(s, this).ToArray();
            Array.Sort(pieces, Descending);

            return string.Concat(pieces);
        }
    }

    private sealed class MaximizeByMergeSort : ILargestSpecialString
    {
        public string Maximize(string s)
        {
            if (s.Length <= MinimalSpecialStringLength)
            {
                return s;
            }

            var pieces = SplitIntoPieces(s, this).ToArray();
            MergeSort.Sort<string, ArrayIndexedSequence<string>>(new ArrayIndexedSequence<string>(pieces), Descending);

            return string.Concat(pieces);
        }
    }
}
