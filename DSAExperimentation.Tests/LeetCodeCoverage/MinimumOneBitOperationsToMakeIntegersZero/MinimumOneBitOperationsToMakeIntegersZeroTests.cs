using DSAExperimentation.DataStructures.HashMap;
using RepoQueue = DSAExperimentation.DataStructures.Queue.Queue<int>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumOneBitOperationsToMakeIntegersZero;

// LeetCode 1611. Minimum One Bit Operations to Make Integers Zero: the two allowed
// operations turn every non-negative integer into a node of one single implicit
// graph - "flip bit 0" is always a valid move, and "flip the bit just above n's own
// lowest set bit" is the only other one, so every state has degree <= 2 and the
// whole graph is one path (the reflected-binary Gray code sequence, 0 at one end).
// The fewest operations to turn n into 0 is exactly that path's BFS distance from
// 0 - this repo's own Queue<int> as the frontier (MinimumHeightTreesTests/
// ShortestPathInBinaryMatrixTests precedent) and HashMap<int,int> as the distance
// map, hand-rolled the same way since the state space isn't a pre-built node graph.
public sealed partial class MinimumOneBitOperationsToMakeIntegersZeroTests
{
    [Fact]
    public void MinimumOneBitOperations_AlreadyZero_ReturnsZero()
        => Assert.Equal(0, MinimumOneBitOperations(0));

    [Fact]
    public void MinimumOneBitOperations_ThreeNeedsTwoFlips_ReturnsTwo()
        => Assert.Equal(2, MinimumOneBitOperations(3));

    [Fact]
    public void MinimumOneBitOperations_SixNeedsFourFlips_ReturnsFour()
        => Assert.Equal(4, MinimumOneBitOperations(6));

    private static int MinimumOneBitOperations(int n)
    {
        if (n == 0)
        {
            return 0;
        }

        var distances = new HashMap<int, int>();
        var frontier = new RepoQueue();
        distances.Set(0, 0);
        frontier.Enqueue(0);

        while (frontier.TryDequeue(out var state))
        {
            distances.TryGetValue(state, out var distance);

            if (state == n)
            {
                return distance;
            }

            foreach (var neighbor in Neighbors(state))
            {
                if (!distances.HasKey(neighbor))
                {
                    distances.Set(neighbor, distance + 1);
                    frontier.Enqueue(neighbor);
                }
            }
        }

        return -1;
    }

    private static IEnumerable<int> Neighbors(int state)
    {
        yield return state ^ 1;

        if (state != 0)
        {
            yield return state ^ (1 << (LowestSetBitIndex(state) + 1));
        }
    }

    private static int LowestSetBitIndex(int value)
    {
        var index = 0;

        while ((value & 1) == 0)
        {
            value >>= 1;
            index++;
        }

        return index;
    }
}
