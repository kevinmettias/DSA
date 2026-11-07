namespace DSAExperimentation.LeetCode.Solutions.CouplesHoldingHands;

// LC 765's promise about its input: "0 <= row[i] < 2n" and "All the elements of row are unique",
// so the row seats each of the 2n people once and is a permutation of them. LeetCode's judge never
// poses an input that breaks it. The permutation-cycle strategy reads the row as a permutation and
// refuses one that is not; the other two answer for whoever they find. Generated cases do not
// always keep the promise (two seat person 29 twice and leave out 31), so a harness that runs them
// asks this first.
internal static class CouplesHoldingHandsPromise
{
    public static bool IsKeptBy(int[] row)
    {
        var everyone = Enumerable.Range(0, row.Length);
        return row.Order().SequenceEqual(everyone);
    }
}
