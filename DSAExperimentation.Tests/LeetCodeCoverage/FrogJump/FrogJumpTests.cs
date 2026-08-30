using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FrogJump;

// LeetCode 403. Frog Jump: DP over the set of jump sizes that can land on each
// stone, keyed by this repo's own HashMap<TKey,TValue> nested exactly the way
// DesignTwitter's per-user followee set already is
// (HashMap<int, HashMap<int, bool>>) - a stone position maps to the set of
// jump sizes reachable at it. Stone positions double as the inner map's own
// key space too, so no separate index/HashSet primitive is needed to enumerate
// a stone's reachable jump sizes - HashMap<TKey,TValue>.Keys already does.
public sealed partial class FrogJumpTests
{
    [Fact]
    public void CanCross_ClassicReachableSequence_ReturnsTrue()
    {
        int[] stones = [0, 1, 3, 5, 6, 8, 12, 17];

        Assert.True(CanCross(stones));
    }

    [Fact]
    public void CanCross_GapTooLargeToClear_ReturnsFalse()
    {
        int[] stones = [0, 1, 2, 3, 4, 8, 9, 11];

        Assert.False(CanCross(stones));
    }

    private static bool CanCross(int[] stones)
    {
        var jumpsByStone = new HashMap<int, HashMap<int, bool>>();

        foreach (var stone in stones)
        {
            jumpsByStone.Set(stone, new HashMap<int, bool>());
        }

        jumpsByStone.TryGetValue(stones[0], out var startJumps);
        startJumps.Set(0, true);

        foreach (var stone in stones)
        {
            jumpsByStone.TryGetValue(stone, out var jumps);

            foreach (var jump in jumps.Keys)
            {
                for (var delta = -1; delta <= 1; delta++)
                {
                    var nextJump = jump + delta;

                    if (nextJump <= 0)
                    {
                        continue;
                    }

                    if (jumpsByStone.TryGetValue(stone + nextJump, out var nextJumps))
                    {
                        nextJumps.Set(nextJump, true);
                    }
                }
            }
        }

        jumpsByStone.TryGetValue(stones[^1], out var lastJumps);
        return lastJumps.Count > 0;
    }
}
