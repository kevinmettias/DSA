using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.FrogJump;

// LeetCode 403. Frog Jump: decide whether the frog can cross a river by stones at
// given positions, where each jump size may differ from the last jump by at most 1
// (and must stay positive).
//
// The two strategies differ in how they avoid re-exploring the same (stone, jump
// size) state - the baseline retries every branch via unmemoized recursion with a
// binary search for the landing stone; the other strategy is a DP over the set of
// jump sizes that can land on each stone, keyed by this repo's own
// HashMap<TKey,TValue> nested exactly the way DesignTwitter's per-user followee set
// already is (HashMap<int, HashMap<int, bool>>) - a stone position maps to the set
// of jump sizes reachable at it. Stone positions double as the inner map's own key
// space too, so no separate index/HashSet primitive is needed to enumerate a
// stone's reachable jump sizes - HashMap<TKey,TValue>.Keys already does.
internal static class FrogJumpSolution
{
    // The textbook arm: unmemoized recursive DFS, retrying every {-1,0,+1} jump-size
    // delta with a binary search for the landing stone. Deliberately written without
    // this repo's primitives - it is the arm the DP strategy below has to justify
    // itself against.
    public static bool CanCrossByRecursiveBruteForce(int[] stones) => TryJump(stones, 0, 0);

    // DP over the set of jump sizes known to reach each stone, so no state is ever
    // re-explored.
    public static bool CanCrossByHashMapDynamicProgramming(int[] stones)
    {
        var jumpsByStone = BuildEmptyJumpsByStone(stones);
        SeedStartJump(jumpsByStone, stones[0]);
        PropagateJumps(jumpsByStone, stones);

        return HasReachableJump(jumpsByStone, stones[^1]);
    }

    private static HashMap<int, HashMap<int, bool>> BuildEmptyJumpsByStone(int[] stones)
    {
        var jumpsByStone = new HashMap<int, HashMap<int, bool>>();

        foreach (var stone in stones)
        {
            jumpsByStone.Set(stone, new HashMap<int, bool>());
        }

        return jumpsByStone;
    }

    private static void SeedStartJump(HashMap<int, HashMap<int, bool>> jumpsByStone, int firstStone)
    {
        jumpsByStone.TryGetValue(firstStone, out var startJumps);
        startJumps.Set(0, true);
    }

    private static void PropagateJumps(HashMap<int, HashMap<int, bool>> jumpsByStone, int[] stones)
    {
        foreach (var stone in stones)
        {
            jumpsByStone.TryGetValue(stone, out var jumps);

            foreach (var jump in jumps.Keys)
            {
                ExpandJump(jumpsByStone, stone, jump);
            }
        }
    }

    private static void ExpandJump(HashMap<int, HashMap<int, bool>> jumpsByStone, int stone, int jump)
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

    private static bool HasReachableJump(HashMap<int, HashMap<int, bool>> jumpsByStone, int lastStone)
    {
        jumpsByStone.TryGetValue(lastStone, out var lastJumps);
        return lastJumps.Count > 0;
    }

    private static bool TryJump(int[] stones, int index, int lastJump)
    {
        if (index == stones.Length - 1)
        {
            return true;
        }

        for (var delta = -1; delta <= 1; delta++)
        {
            if (TryDelta(stones, index, lastJump, delta))
            {
                return true;
            }
        }

        return false;
    }

    private static bool TryDelta(int[] stones, int index, int lastJump, int delta)
    {
        var jump = lastJump + delta;

        if (jump <= 0)
        {
            return false;
        }

        var nextIndex = Array.BinarySearch(stones, index + 1, stones.Length - index - 1, stones[index] + jump);

        return nextIndex >= 0 && TryJump(stones, nextIndex, jump);
    }
}
