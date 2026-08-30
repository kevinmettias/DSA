using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SpecialBinaryString;

// LeetCode 761. Special Binary String: split into maximal special substrings at
// each balance-zero point, recurse on each substring's interior, then sort the
// resulting sibling pieces descending via this repo's own MergeSort.Sort over an
// ArrayIndexedSequence with a custom comparer - the same composition
// RussianDollEnvelopesTests/QueueReconstructionByHeightTests already exercise.
// Swapping two consecutive special substrings to maximize the string is exactly
// what a descending sort of same-level sibling pieces produces.
public sealed partial class SpecialBinaryStringTests
{
    [Fact]
    public void MakeLargestSpecial_ClassicExample_SwapsTopLevelPiecesToMaximize()
    {
        // Top-level pieces of the interior are "10" and "1100"; swapping them to
        // "1100" + "10" (then re-wrapping in the outer "1"..."0") is LeetCode's
        // own worked example for this input.
        Assert.Equal("11100100", MakeLargestSpecial("11011000"));
    }

    [Fact]
    public void MakeLargestSpecial_SingleSpecialPair_ReturnsUnchanged()
    {
        Assert.Equal("10", MakeLargestSpecial("10"));
    }

    [Fact]
    public void MakeLargestSpecial_TwoSiblingPiecesOutOfOrder_SwapsThemToMaximize()
    {
        // Top-level pieces are "10" and "1100"; "1100" > "10" lexicographically,
        // so the maximal arrangement swaps them to "1100" + "10".
        Assert.Equal("110010", MakeLargestSpecial("101100"));
    }

    private static string MakeLargestSpecial(string s)
    {
        if (s.Length <= 2)
        {
            return s;
        }

        var pieces = new List<string>();
        var balance = 0;
        var start = 0;

        for (var i = 0; i < s.Length; i++)
        {
            balance += s[i] == '1' ? 1 : -1;

            if (balance == 0)
            {
                pieces.Add("1" + MakeLargestSpecial(s.Substring(start + 1, i - start - 1)) + "0");
                start = i + 1;
            }
        }

        var items = pieces.ToArray();
        var descending = Comparer<string>.Create((a, b) => string.CompareOrdinal(b, a));

        MergeSort.Sort<string, ArrayIndexedSequence<string>>(new ArrayIndexedSequence<string>(items), descending);

        return string.Concat(items);
    }
}
