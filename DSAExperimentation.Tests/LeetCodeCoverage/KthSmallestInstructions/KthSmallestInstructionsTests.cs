using System.Text;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.KthSmallestInstructions;

// LeetCode 1643. Kth Smallest Instructions: Memoizer caches the exact same
// Pascal's-triangle recurrence UniquePathsTests already uses for LeetCode 62
// (ways(v,h) = ways(v-1,h) + ways(v,h-1), the count of H/V arrangements reaching
// the target from a cell with v vertical and h horizontal moves left, i.e.
// C(v+h,v)) - greedily deciding one character at a time whether the
// lexicographically-smaller choice 'H' still leaves at least k sequences
// reachable, walking k down past it otherwise.
public sealed partial class KthSmallestInstructionsTests
{
    [Fact]
    public void KthSmallestPath_SmallestK_IsAllHBeforeV()
        => Assert.Equal("HHHVV", KthSmallestPath([2, 3], 1));

    [Fact]
    public void KthSmallestPath_MiddleK_MixesHAndV()
        => Assert.Equal("HHVHV", KthSmallestPath([2, 3], 2));

    [Fact]
    public void KthSmallestPath_LargestK_IsAllVBeforeH()
        => Assert.Equal("VVHHH", KthSmallestPath([2, 3], 10));

    private static string KthSmallestPath(int[] destination, long k)
    {
        var remainingV = destination[0];
        var remainingH = destination[1];
        var path = new StringBuilder();

        while (remainingV > 0 || remainingH > 0)
        {
            if (remainingH == 0)
            {
                path.Append('V');
                remainingV--;
                continue;
            }

            if (remainingV == 0)
            {
                path.Append('H');
                remainingH--;
                continue;
            }

            var waysIfH = Memoizer.Memoize<(int V, int H), long>((remainingV, remainingH - 1), Ways);

            if (k <= waysIfH)
            {
                path.Append('H');
                remainingH--;
            }
            else
            {
                k -= waysIfH;
                path.Append('V');
                remainingV--;
            }
        }

        return path.ToString();

        static long Ways((int V, int H) state, Func<(int V, int H), long> ways)
        {
            var (v, h) = state;
            return v == 0 || h == 0 ? 1 : ways((v - 1, h)) + ways((v, h - 1));
        }
    }
}
