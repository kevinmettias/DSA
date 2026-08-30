using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.StickersToSpellWord;

// LeetCode 691. Stickers to Spell Word: memoized recursion over the remaining
// (canonically sorted) target letters, playing the same role Memoizer.Memoize<string,int>
// already plays for WordBreak/DecodeWays. The recurrence branches over stickers
// instead of a fixed left-to-right split, but every recursive call is still on a
// strictly shorter state - whichever sticker is tried always consumes at least one
// copy of the state's own first character - which is exactly the well-founded
// precondition Memoizer's own doc comment requires.
public sealed partial class StickersToSpellWordTests
{
    [Fact]
    public void MinStickers_ClassicExample_ReturnsThreeStickers()
    {
        string[] stickers = ["with", "example", "science"];

        var result = MinStickers(stickers, "thehat");

        Assert.Equal(3, result);
    }

    [Fact]
    public void MinStickers_TargetLettersNeverCovered_ReturnsNegativeOne()
    {
        string[] stickers = ["notice", "possible"];

        var result = MinStickers(stickers, "basicbasic");

        Assert.Equal(-1, result);
    }

    private static int MinStickers(string[] stickers, string target)
    {
        var stickerCounts = stickers.Select(BuildCounts).ToArray();
        var sortedTarget = string.Concat(target.OrderBy(c => c));

        var result = Memoizer.Memoize<string, int>(sortedTarget, (state, minFor) => Solve(state, stickerCounts, minFor));
        return result == int.MaxValue ? -1 : result;
    }

    private static int Solve(string state, int[][] stickerCounts, Func<string, int> minFor)
    {
        if (state.Length == 0)
        {
            return 0;
        }

        var best = int.MaxValue;

        foreach (var counts in stickerCounts)
        {
            if (counts[state[0] - 'a'] == 0)
            {
                continue;
            }

            var sub = minFor(ApplySticker(state, counts));

            if (sub != int.MaxValue)
            {
                best = Math.Min(best, sub + 1);
            }
        }

        return best;
    }

    // Consumes as many of `state`'s letters as `counts` provides for each letter,
    // returning whatever is left over (still sorted, since it's a subsequence of the
    // sorted input) - the state transition Solve's memoized recursion branches on.
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

    private static int[] BuildCounts(string sticker)
    {
        var counts = new int[26];

        foreach (var c in sticker)
        {
            counts[c - 'a']++;
        }

        return counts;
    }
}
