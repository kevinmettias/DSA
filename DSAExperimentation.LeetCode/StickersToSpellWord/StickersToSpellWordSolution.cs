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
    public static int MinStickersByNaiveRecursion(string[] stickers, string target)
    {
        var prepared = PreparedStickers.Build(stickers, target);
        return MinStickersByNaiveRecursion(prepared);
    }

    public static int MinStickersByNaiveRecursion(PreparedStickers input)
    {
        var order = new StickerCoverOrder(input.StickerCounts);
        var result = order.Replay(input.SortedTarget, order);

        return result == int.MaxValue ? LeetCodeAnswer.None : result;
    }

    // Memoizer.Memoize<string,int> caches on the canonical state string, the same
    // role it plays for WordBreak/DecodeWays. Every recursive call is on a strictly
    // shorter state, since any sticker that is tried consumes at least one copy of
    // the state's own first character - the well-founded precondition Memoizer's
    // own doc comment requires.
    public static int MinStickersByMemoizedRecursion(string[] stickers, string target)
    {
        var prepared = PreparedStickers.Build(stickers, target);
        return MinStickersByMemoizedRecursion(prepared);
    }

    public static int MinStickersByMemoizedRecursion(PreparedStickers input)
    {
        var result = Memoizer.Memoize<string, int>(
            input.SortedTarget,
            new StickerCoverOrder(input.StickerCounts));

        return result == int.MaxValue ? LeetCodeAnswer.None : result;
    }

    // The rule, named: a state is solved by whichever sticker to apply first leaves the
    // fewest stickers for the rest - one for the sticker itself plus whatever the state
    // it leaves behind costs - or stays unsolvable when no sticker covers the state's
    // first remaining letter. The same named rule answers both strategies: the memoized
    // arm hands it the cache-backed recursion, and the naive arm replays it straight back
    // into itself with no cache in between. The sticker letter counts are the whole of
    // what the rule needs from its caller, so they are the constructor's only input.
    private sealed class StickerCoverOrder(int[][] stickerCounts) : IRecurrence<string, int>
    {
        public int Replay(string state, IRecurrence<string, int> rest)
        {
            if (state.Length == 0)
            {
                return 0;
            }

            return BestOverStickers(state, rest);
        }

        // Which sticker to apply first is the whole of the choice: any sticker covering
        // the state's first remaining letter is a candidate, and each one costs itself
        // plus whatever the letters it leaves behind cost.
        private int BestOverStickers(string state, IRecurrence<string, int> rest)
        {
            var best = int.MaxValue;

            foreach (var counts in stickerCounts)
            {
                if (counts[state[0] - 'a'] == 0)
                {
                    continue;
                }

                var nextState = ApplySticker(state, counts);
                var sub = rest.Replay(nextState, rest);

                if (sub != int.MaxValue)
                {
                    best = Math.Min(best, sub + 1);
                }
            }

            return best;
        }
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
