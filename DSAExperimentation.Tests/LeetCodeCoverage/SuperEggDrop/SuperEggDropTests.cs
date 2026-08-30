using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SuperEggDrop;

// LeetCode 887. Super Egg Drop: minimize the worst-case number of trial drops
// needed to find the critical floor with a fixed number of eggs. Memoized minimax
// over an (eggs, floors) state - the same pair-state shape
// GuessNumberHigherOrLowerII already uses for (low, high) via this repo's own
// Memoizer<TState,TResult> - except the next trial floor is found by binary search
// instead of an exhaustive scan, since WorstCaseMoves(trial) is monotonic: raising
// trial can only help the "breaks" branch and hurt the "survives" branch, so the
// two curves cross at (or straddle) the optimal trial floor.
public sealed partial class SuperEggDropTests
{
    [Theory]
    [InlineData(1, 2, 2)]
    [InlineData(2, 6, 3)]
    [InlineData(3, 14, 4)]
    public void SuperEggDrop_LeetCodeExamples_ReturnsMinimumWorstCaseMoves(int eggs, int floors, int expected)
        => Assert.Equal(expected, MinMoves(eggs, floors));

    private static int MinMoves(int eggs, int floors)
        => Memoizer.Memoize<(int Eggs, int Floors), int>((eggs, floors), WorstCaseMoves);

    private static int WorstCaseMoves((int Eggs, int Floors) state, Func<(int Eggs, int Floors), int> movesFor)
    {
        var (eggs, floors) = state;
        if (floors == 0) return 0;
        if (eggs == 1) return floors;

        var low = 1;
        var high = floors;
        var best = int.MaxValue;

        while (low <= high)
        {
            var trial = (low + high) / 2;
            var breaks = movesFor((eggs - 1, trial - 1));
            var survives = movesFor((eggs, floors - trial));
            best = Math.Min(best, 1 + Math.Max(breaks, survives));

            if (breaks < survives)
            {
                low = trial + 1;
            }
            else
            {
                high = trial - 1;
            }
        }

        return best;
    }
}
