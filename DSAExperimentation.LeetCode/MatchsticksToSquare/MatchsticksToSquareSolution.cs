using DSAExperimentation.Algorithms.Backtracking;

namespace DSAExperimentation.LeetCode.MatchsticksToSquare;

// LeetCode 473. Matchsticks to Square: can every matchstick be assigned to one of
// four sides so all four sides end up the same length?
//
// Both strategies assign matchsticks longest-first to one of four running bucket
// sums, backtracking whenever a placement would overshoot the target side length -
// Candidates only offers buckets that don't overshoot, which by construction
// (every bucket <= side, and the total is a multiple of 4) forces every bucket to
// land on exactly the side length once all matchsticks are placed.
internal static class MatchsticksToSquareSolution
{
    private const int SquareSideCount = 4;

    // The textbook recursion: a hand-written DFS over four running bucket sums,
    // undoing a placement on backtrack. Deliberately written without this repo's
    // Backtrack primitive - it is the arm the composed solution below has to
    // justify itself against.
    public static bool CanMakeSquareByNaiveBacktracking(int[] matchsticks)
    {
        if (!TryComputeSide(matchsticks, out var side))
        {
            return false;
        }

        var sorted = SortedDescending(matchsticks);
        var buckets = new int[SquareSideCount];

        return SearchByNaiveBacktracking(sorted, buckets, side, 0);
    }

    private static bool SearchByNaiveBacktracking(int[] sorted, int[] buckets, int side, int index)
    {
        if (index == sorted.Length)
        {
            return true;
        }

        for (var bucket = 0; bucket < SquareSideCount; bucket++)
        {
            if (TryPlaceInBucket(sorted, buckets, side, bucket, index))
            {
                return true;
            }
        }

        return false;
    }

    private static bool TryPlaceInBucket(int[] sorted, int[] buckets, int side, int bucket, int index)
    {
        if (buckets[bucket] + sorted[index] > side)
        {
            return false;
        }

        buckets[bucket] += sorted[index];

        if (SearchByNaiveBacktracking(sorted, buckets, side, index + 1))
        {
            return true;
        }

        buckets[bucket] -= sorted[index];
        return false;
    }

    // This repo's own Backtrack.TrySearch (the NQueens/SudokuSolver precedent),
    // closed over the same choose/explore/unchoose steps NaiveBacktracking writes
    // out by hand.
    public static bool CanMakeSquareByGenericBacktrack(int[] matchsticks)
    {
        if (!TryComputeSide(matchsticks, out var side))
        {
            return false;
        }

        var sorted = SortedDescending(matchsticks);
        var state = new BucketState(sorted, side);

        return Backtrack.TrySearch<BucketState, int>(state, new BacktrackingSteps<BucketState, int>(
            IsSolution: s => s.Index == sorted.Length,
            Candidates: s => s.Index == sorted.Length ? [] : Enumerable.Range(0, SquareSideCount).Where(s.CanPlace),
            Choose: (s, bucket) => s.Place(bucket),
            Unchoose: (s, bucket) => s.Remove(bucket),
            OnSolution: _ => true));
    }

    private static bool TryComputeSide(int[] matchsticks, out int side)
    {
        side = 0;

        if (matchsticks.Length < SquareSideCount)
        {
            return false;
        }

        var total = matchsticks.Sum();

        if (total % SquareSideCount != 0)
        {
            return false;
        }

        side = total / SquareSideCount;
        return matchsticks.Max() <= side;
    }

    private static int[] SortedDescending(int[] matchsticks)
    {
        var sorted = (int[])matchsticks.Clone();
        Array.Sort(sorted);
        Array.Reverse(sorted);
        return sorted;
    }

    private sealed class BucketState(int[] matchsticks, int side)
    {
        private readonly int[] _buckets = new int[SquareSideCount];

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
