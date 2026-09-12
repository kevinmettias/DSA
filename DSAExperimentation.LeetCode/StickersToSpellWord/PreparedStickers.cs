namespace DSAExperimentation.LeetCode.StickersToSpellWord;

// Precomputed per-sticker letter counts and the canonically sorted target letters -
// the state both StickersToSpellWordSolution strategies recurse over. Bespoke to
// this one problem: unlike Domain/Locks' wheel geometry or Domain/Modular's 1e9+7,
// nothing here fixes a modulus or a vertex set that another problem could share -
// it is simply this problem's own input shape, hoisted out of the measured
// strategies so a benchmark can build it once in [GlobalSetup].
internal readonly record struct PreparedStickers(int[][] StickerCounts, string SortedTarget)
{
    private const int AlphabetSize = 26;

    public static PreparedStickers Build(string[] stickers, string target)
    {
        var stickerCounts = stickers.Select(BuildCounts).ToArray();
        var sortedTarget = string.Concat(target.OrderBy(c => c));

        return new PreparedStickers(stickerCounts, sortedTarget);
    }

    private static int[] BuildCounts(string sticker)
    {
        var counts = new int[AlphabetSize];

        foreach (var c in sticker)
        {
            counts[c - 'a']++;
        }

        return counts;
    }
}
