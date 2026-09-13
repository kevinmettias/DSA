using DSAExperimentation.DataStructures.HashMap;
using StateQueue = DSAExperimentation.DataStructures.Queue.Queue<int>;

namespace DSAExperimentation.LeetCode.MinimumOneBitOperationsToMakeIntegersZero;

// LeetCode 1611. Minimum One Bit Operations to Make Integers Zero: the two allowed
// operations turn every non-negative integer into a node of one single implicit
// graph - "flip bit 0" is always a valid move, and "flip the bit just above n's own
// lowest set bit" is the only other one, so every state has degree <= 2 and the
// whole graph is one path (the reflected-binary Gray code sequence, 0 at one end).
//
// Both strategies answer the same question from that observation. The search walks
// the path a hop at a time and therefore does work proportional to the answer
// itself; the closed form recognizes the answer as the inverse Gray code of n and
// only ever touches n's own ~log2(n) bits.
internal static class MinimumOneBitOperationsToMakeIntegersZeroSolution
{
    // The state every operation count is measured to, and therefore the search root.
    private const int ZeroState = 0;

    // n is already the goal, so nothing has to be flipped.
    private const int NoOperations = 0;

    // Bit 0, the one operation one may always apply.
    private const int LowestBit = 1;

    // The explicit answer: breadth-first search over the implicit move graph, with
    // this repo's own Queue<int> as the frontier and HashMap<int, int> as the
    // distance map, since the state space is generated on the fly rather than being
    // a pre-built node graph. It is the arm the closed form has to justify itself
    // against - and, being a plain shortest-path walk, it is also the one that
    // demonstrates the answer rather than asserting it.
    public static int MinimumOneBitOperationsByBreadthFirstSearch(int n)
    {
        if (n == ZeroState)
        {
            return NoOperations;
        }

        var distances = new HashMap<int, int>();
        var frontier = new StateQueue();
        distances.Set(ZeroState, NoOperations);
        frontier.Enqueue(ZeroState);

        return WalkToTarget(frontier, distances, n);
    }

    private static int WalkToTarget(StateQueue frontier, HashMap<int, int> distances, int n)
    {
        while (frontier.TryDequeue(out var state))
        {
            distances.TryGetValue(state, out var distance);

            if (state == n)
            {
                return distance;
            }

            EnqueueUnseenNeighbors(frontier, distances, state, distance);
        }

        return LeetCodeAnswer.None;
    }

    private static void EnqueueUnseenNeighbors(
        StateQueue frontier,
        HashMap<int, int> distances,
        int state,
        int distance)
    {
        foreach (var neighbor in Neighbors(state))
        {
            if (!distances.HasKey(neighbor))
            {
                distances.Set(neighbor, distance + 1);
                frontier.Enqueue(neighbor);
            }
        }
    }

    // Operation 1 flips bit 0 unconditionally; operation 2 flips the bit directly
    // above the lowest set bit, which only exists once something is set at all.
    private static IEnumerable<int> Neighbors(int state)
    {
        yield return state ^ LowestBit;

        if (state != ZeroState)
        {
            yield return state ^ (LowestBit << (LowestSetBitIndex(state) + 1));
        }
    }

    private static int LowestSetBitIndex(int value)
    {
        var index = 0;

        while ((value & LowestBit) == 0)
        {
            value >>= 1;
            index++;
        }

        return index;
    }

    // The closed form: the path position of n in the reflected-binary Gray code is
    // its inverse Gray code, the running xor of n's successive right shifts. Same
    // answer as the search, in one pass over n's bits.
    public static int MinimumOneBitOperationsByInverseGrayCode(int n)
    {
        var result = NoOperations;
        var remaining = n;

        while (remaining > ZeroState)
        {
            result ^= remaining;
            remaining >>= 1;
        }

        return result;
    }
}
