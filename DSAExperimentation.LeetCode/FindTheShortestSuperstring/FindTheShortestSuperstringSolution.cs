using DSAExperimentation.Algorithms.DynamicProgramming;
using TourBest = (int Best, int Prev);
using TourState = (int Mask, int Last);

namespace DSAExperimentation.LeetCode.FindTheShortestSuperstring;

// LeetCode 943. Find the Shortest Superstring: the shortest string containing every
// given word, where no word is a substring of another.
//
// WordOverlaps turns the problem into "order the words so the total overlap between
// consecutive words is as large as possible" - a maximum-weight Hamiltonian path.
// The two strategies differ only in how they search that ordering:
//
// - ShortestSuperstringByPermutations tries every order outright, O(n! * n). It is
//   the baseline the DP has to justify itself against, so its internals are plain
//   BCL arrays and recursion.
// - ShortestSuperstringByMemoizedBitmask routes the same search through this repo's
//   Memoizer over state (Mask, Last) - "these words are placed, Last is the rightmost
//   one" - which collapses it to O(2^n * n^2). That is exactly the (visited-set,
//   current) state ShortestPathVisitingAllNodes already establishes, maximizing
//   overlap instead of minimizing hop count. The memoized result carries the
//   predecessor that achieved the optimum as well as the optimum itself, because the
//   answer is a word order to rebuild, not just the metric.
internal static class FindTheShortestSuperstringSolution
{
    // No candidate order has been seen yet; any real total overlap (>= 0) beats it.
    private const int NoTourYet = -1;

    // No predecessor: the word is first in the order.
    private const int NoPredecessor = -1;

    public static string ShortestSuperstringByPermutations(string[] words) =>
        ShortestSuperstringByPermutations(WordOverlaps.Build(words));

    public static string ShortestSuperstringByPermutations(WordOverlaps overlaps)
    {
        var search = new PermutationSearch(overlaps);
        search.Explore(0, 0);

        return overlaps.Assemble(search.BestOrder);
    }

    public static string ShortestSuperstringByMemoizedBitmask(string[] words) =>
        ShortestSuperstringByMemoizedBitmask(WordOverlaps.Build(words));

    public static string ShortestSuperstringByMemoizedBitmask(WordOverlaps overlaps) =>
        overlaps.Assemble(BestOrderByMemoizedBitmask(overlaps));

    private static int[] BestOrderByMemoizedBitmask(WordOverlaps overlaps)
    {
        var (wordCount, fullMask) = TourSearchDomain(overlaps);
        var recurrence = new BestTourEndingAtWord(overlaps);

        var (last, prev) = BestFinalWord(wordCount, fullMask, recurrence);

        return BacktrackOrder(wordCount, last, prev, recurrence);
    }

    // The words to order, and the mask with every one of them placed - the domain the
    // bitmask tour search walks.
    private static (int WordCount, int FullMask) TourSearchDomain(WordOverlaps overlaps)
    {
        var wordCount = overlaps.Count;

        return (wordCount, (1 << wordCount) - 1);
    }

    // Memoizer.Memoize only ever returns the result for the one start state it is
    // given, so each candidate final word is a separate top-level call; the best of
    // them is the tour to rebuild.
    private static (int Last, int Prev) BestFinalWord(
        int wordCount,
        int fullMask,
        IRecurrence<TourState, TourBest> recurrence)
    {
        var last = 0;
        var prev = NoPredecessor;
        var bestTotal = NoTourYet;

        for (var candidate = 0; candidate < wordCount; candidate++)
        {
            var (total, candidatePrev) = Memoizer.Memoize<TourState, TourBest>((fullMask, candidate), recurrence);

            if (total > bestTotal)
            {
                bestTotal = total;
                last = candidate;
                prev = candidatePrev;
            }
        }

        return (last, prev);
    }

    /// <summary>
    /// The recurrence, named: the state (Mask, Last) means these words are placed and Last
    /// is the rightmost one; the result is the best total overlap still achievable, and
    /// the predecessor achieving it.
    /// </summary>
    private sealed class BestTourEndingAtWord(WordOverlaps overlaps) : IRecurrence<TourState, TourBest>
    {
        /// <inheritdoc/>
        public TourBest Replay(TourState state, IRecurrence<TourState, TourBest> rest)
        {
            var remaining = state.Mask & ~(1 << state.Last);

            if (remaining == 0)
            {
                return (0, NoPredecessor);
            }

            var search = new PredecessorSearch(remaining, state.Last, rest, overlaps);
            var result = (Best: NoTourYet, Prev: NoPredecessor);

            for (var candidate = 0; candidate < overlaps.Count; candidate++)
            {
                result = ConsiderPredecessor(candidate, search, result);
            }

            return result;
        }
    }

    private static TourBest ConsiderPredecessor(int candidate, PredecessorSearch search, TourBest current)
    {
        if ((search.Remaining & (1 << candidate)) == 0)
        {
            return current;
        }

        var (subBest, _) = search.Rest.Replay((search.Remaining, candidate), search.Rest);
        var total = subBest + search.Overlaps.Between(candidate, search.LastWord);

        return total > current.Best ? ImprovedTour(total, candidate) : current;
    }

    // The tour this candidate improves to: its longer total, with this word as the
    // predecessor the backtrack walks through.
    private static TourBest ImprovedTour(int total, int candidate) => (total, candidate);

    // Walk the (Mask, Last) -> Prev chain backwards from the best final state,
    // rebuilding the word order the memoized recurrence discovered.
    private static int[] BacktrackOrder(
        int wordCount,
        int last,
        int prev,
        IRecurrence<TourState, TourBest> recurrence)
    {
        var order = new int[wordCount];
        var mask = (1 << wordCount) - 1;

        for (var i = wordCount - 1; i >= 0; i--)
        {
            order[i] = last;
            mask &= ~(1 << last);

            if (mask == 0)
            {
                break;
            }

            last = prev;
            var (_, nextPrev) = Memoizer.Memoize<TourState, TourBest>((mask, last), recurrence);
            prev = nextPrev;
        }

        return order;
    }

    // Exhaustive permutation search: place every word in every remaining slot,
    // keeping whichever complete order accumulated the most overlap. Deliberately
    // written with nothing but BCL arrays - it represents what you would write
    // without this repo.
    private sealed class PermutationSearch(WordOverlaps overlaps)
    {
        private readonly WordOverlaps _overlaps = overlaps;
        private readonly bool[] _used = new bool[overlaps.Count];
        private readonly int[] _order = new int[overlaps.Count];
        private int _bestOverlap = NoTourYet;

        public int[] BestOrder { get; } = new int[overlaps.Count];

        public void Explore(int depth, int overlapSoFar)
        {
            if (depth == _order.Length)
            {
                Record(overlapSoFar);
                return;
            }

            for (var next = 0; next < _order.Length; next++)
            {
                if (!_used[next])
                {
                    TryCandidate(next, depth, overlapSoFar);
                }
            }
        }

        private void Record(int overlapSoFar)
        {
            if (overlapSoFar > _bestOverlap)
            {
                _bestOverlap = overlapSoFar;
                _order.CopyTo(BestOrder, 0);
            }
        }

        private void TryCandidate(int next, int depth, int overlapSoFar)
        {
            _used[next] = true;
            _order[depth] = next;
            var added = depth == 0 ? 0 : _overlaps.Between(_order[depth - 1], next);

            Explore(depth + 1, overlapSoFar + added);

            _used[next] = false;
        }
    }

    private readonly record struct PredecessorSearch(
        int Remaining,
        int LastWord,
        IRecurrence<TourState, TourBest> Rest,
        WordOverlaps Overlaps);
}
