using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.StoneGameIX;

// LeetCode 2029. Stone Game IX: bucket stones by value mod 3 using this repo's own
// HashMap<int,int>, then apply the known closed-form parity rule over the three
// bucket counts. No game-tree search needed - a remainder-0 stone only ever flips
// whose turn effectively "counts," and only the remainder-1/remainder-2 stones can
// ever push the running sum to a multiple of 3.
public sealed class StoneGameIXTests
{
    [Theory]
    [InlineData(new[] { 2, 1 }, true)]
    [InlineData(new[] { 2 }, false)]
    [InlineData(new[] { 5, 1, 2, 4, 3 }, false)]
    public void AliceWins_LeetCodeExamples_MatchesExpectedOutcome(int[] stones, bool expected)
        => Assert.Equal(expected, AliceWins(stones));

    private static bool AliceWins(int[] stones)
    {
        var counts = new HashMap<int, int>();

        foreach (var stone in stones)
        {
            counts.TryGetValue(stone % 3, out var c);
            counts.Set(stone % 3, c + 1);
        }

        counts.TryGetValue(0, out var cnt0);
        counts.TryGetValue(1, out var cnt1);
        counts.TryGetValue(2, out var cnt2);

        return cnt0 % 2 == 0
            ? cnt1 >= 1 && cnt2 >= 1
            : Math.Abs(cnt1 - cnt2) >= 3;
    }
}
