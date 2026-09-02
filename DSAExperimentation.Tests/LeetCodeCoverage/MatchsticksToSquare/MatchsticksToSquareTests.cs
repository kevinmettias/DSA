using DSAExperimentation.Algorithms.Backtracking;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MatchsticksToSquare;

// LeetCode 473. Matchsticks to Square: this repo's own Backtrack.TrySearch (the
// NQueens/SudokuSolver precedent) assigns each matchstick, longest first, to one of
// four running bucket sums - Candidates only offers buckets that don't overshoot the
// side length, which by construction (every bucket <= side, and the total is a
// multiple of 4) forces every bucket to land on exactly the side length once all
// matchsticks are placed.
public sealed partial class MatchsticksToSquareTests
{
    [Fact]
    public void MakeSquare_FourEqualSidesPossible_ReturnsTrue()
        => Assert.True(CanMakeSquare([1, 1, 2, 2, 2]));

    [Fact]
    public void MakeSquare_TotalIsMultipleOfFourButNoValidSplit_ReturnsFalse()
        => Assert.False(CanMakeSquare([3, 3, 3, 3, 4]));

    [Fact]
    public void MakeSquare_FewerThanFourMatchsticks_ReturnsFalse()
        => Assert.False(CanMakeSquare([1, 1, 1]));

    private static bool CanMakeSquare(int[] matchsticks)
    {
        if (!TryComputeSide(matchsticks, out var side))
        {
            return false;
        }

        var sorted = SortedDescending(matchsticks);
        var state = new State(sorted, side);

        return Backtrack.TrySearch<State, int>(state, new BacktrackingSteps<State, int>(
            IsSolution: s => s.Index == sorted.Length,
            Candidates: s => s.Index == sorted.Length ? [] : Enumerable.Range(0, 4).Where(s.CanPlace),
            Choose: (s, bucket) => s.Place(bucket),
            Unchoose: (s, bucket) => s.Remove(bucket),
            OnSolution: _ => true));
    }

    private static bool TryComputeSide(int[] matchsticks, out int side)
    {
        side = 0;

        if (matchsticks.Length < 4)
        {
            return false;
        }

        var total = matchsticks.Sum();

        if (total % 4 != 0)
        {
            return false;
        }

        side = total / 4;
        return matchsticks.Max() <= side;
    }

    private static int[] SortedDescending(int[] matchsticks)
    {
        var sorted = (int[])matchsticks.Clone();
        Array.Sort(sorted);
        Array.Reverse(sorted);
        return sorted;
    }

    private sealed class State(int[] matchsticks, int side)
    {
        private readonly int[] _buckets = new int[4];

        public int Index { get; private set; }

        public bool CanPlace(int bucket) => _buckets[bucket] + matchsticks[Index] <= side;

        public void Place(int bucket)
        {
            _buckets[bucket] += matchsticks[Index];
            Index++;
        }

        public void Remove(int bucket)
        {
            Index--;
            _buckets[bucket] -= matchsticks[Index];
        }
    }
}
