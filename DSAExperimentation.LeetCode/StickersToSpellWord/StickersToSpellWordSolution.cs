using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.StickersToSpellWord;

// LeetCode 691. Stickers to Spell Word: fewest stickers, each reusable any number
// of times, whose combined letters cover every letter of `target` (with
// multiplicity), or -1 if no combination does.
//
// Both strategies recurse the same way over a state that canonically sorts the
// remaining target letters, so equivalent choice paths collapse onto the same
// string: try each sticker that covers the state's first remaining letter, recurse
// on what is left over, take the best. They differ only in whether repeated states
// are cached.
internal static class StickersToSpellWordSolution
{
    // The textbook answer: plain recursion with no cache, recomputing every choice
    // path from scratch even when two paths land on the identical remaining-letters
    // state. Deliberately hand-rolled without this repo's Memoizer - it is the arm
    // MinStickersByMemoizedRecursion is measured against.
    public static int MinStickersByNaiveRecursion(string[] stickers, string target) =>
        MinStickersByNaiveRecursion(PreparedStickers.Build(stickers, target));

    public static int MinStickersByNaiveRecursion(PreparedStickers input)
    {
        int NaiveMinFor(string state) => Solve(state, input.StickerCounts, NaiveMinFor);

        var result = Solve(input.SortedTarget, input.StickerCounts, NaiveMinFor);
        return result == int.MaxValue ? LeetCodeAnswer.None : result;
    }

    // Memoizer.Memoize<string,int> caches on the canonical state string, the same
    // role it plays for WordBreak/DecodeWays. Every recursive call is on a strictly
    // shorter state, since any sticker that is tried consumes at least one copy of
    // the state's own first character - the well-founded precondition Memoizer's
    // own doc comment requires.
    public static int MinStickersByMemoizedRecursion(string[] stickers, string target) =>
        MinStickersByMemoizedRecursion(PreparedStickers.Build(stickers, target));

    public static int MinStickersByMemoizedRecursion(PreparedStickers input)
    {
        var result = Memoizer.Memoize<string, int>(
            input.SortedTarget,
            (state, minFor) => Solve(state, input.StickerCounts, minFor));

        return result == int.MaxValue ? LeetCodeAnswer.None : result;
    }

    private static int Solve(string state, int[][] stickerCounts, Func<string, int> minFor)
    {
        if (state.Length == 0)
        {
            return 0;
        }

        return BestOverStickers(state, stickerCounts, minFor);
    }

    private static int BestOverStickers(string state, int[][] stickerCounts, Func<string, int> minFor)
    {
        var best = int.MaxValue;

        foreach (var counts in stickerCounts)
        {
            if (counts[state[0] - 'a'] == 0)
            {
                continue;
            }

            var nextState = ApplySticker(state, counts);
            var sub = minFor(nextState);

            if (sub != int.MaxValue)
            {
                best = Math.Min(best, sub + 1);
            }
        }

        return best;
    }

    // Consumes as many of `state`'s letters as `counts` provides for each letter,
    // returning whatever is left over (still sorted, since it's a subsequence of
    // the sorted input) - the state transition Solve's recursion branches on.
    private static string ApplySticker(string state, int[] counts)
    {
        var remainingCounts = (int[])counts.Clone();
        var leftover = new List<char>();

        foreach (var c in state)
        {
            if (remainingCounts[c - 'a'] > 0)
            {
                remainingCounts[c - 'a']--;
            }
            else
            {
                leftover.Add(c);
            }
        }

        return string.Concat(leftover);
    }
}
