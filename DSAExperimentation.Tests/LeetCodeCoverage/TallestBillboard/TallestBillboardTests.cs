using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.TallestBillboard;

// LeetCode 956. Tallest Billboard: a top-down recursion over (index, diff) - diff being
// the running height difference between the two support piles - memoized through this
// repo's own Memoizer<TState,TResult>, the same (index, runningState) shape
// TargetSumTests already uses. Each rod is skipped, added to the taller pile, or added
// to the shorter pile; a state is only ever re-solved once no matter how many
// skip/taller/shorter paths reach the same (index, diff) pair.
public sealed partial class TallestBillboardTests
{
    private const int Unreachable = int.MinValue / 2;

    [Fact]
    public void MaxTaller_ClassicExample_SplitsIntoTwoEqualSixes()
    {
        int[] rods = [1, 2, 3, 6];

        var tallest = TallestBillboardHeight(rods);

        Assert.Equal(6, tallest);
    }

    [Fact]
    public void MaxTaller_NoEqualNonZeroSplitPossible_ReturnsZero()
    {
        int[] rods = [1, 2];

        var tallest = TallestBillboardHeight(rods);

        Assert.Equal(0, tallest);
    }

    private static int TallestBillboardHeight(int[] rods)
    {
        return Memoizer.Memoize<(int Index, int Diff), int>((0, 0), Solve);

        int Solve((int Index, int Diff) state, Func<(int Index, int Diff), int> solve)
        {
            if (state.Index == rods.Length)
            {
                return state.Diff == 0 ? 0 : Unreachable;
            }

            var rod = rods[state.Index];
            var skip = solve((state.Index + 1, state.Diff));
            var addToTaller = rod + solve((state.Index + 1, state.Diff + rod));
            var addToShorter = solve((state.Index + 1, state.Diff - rod));

            return Math.Max(skip, Math.Max(addToTaller, addToShorter));
        }
    }
}
