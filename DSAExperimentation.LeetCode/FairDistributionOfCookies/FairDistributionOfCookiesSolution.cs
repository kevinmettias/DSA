using DSAExperimentation.Algorithms.Backtracking;

namespace DSAExperimentation.LeetCode.FairDistributionOfCookies;

// LeetCode 2305. Fair Distribution of Cookies: hand every bag to one of childCount
// children and minimize the largest total any single child ends up with. This is a
// minimization over every complete assignment, not a first-solution decision
// problem, so both strategies enumerate all leaves rather than stopping at one.
//
// Both arms carry the same two prunes, and both sort the bags largest-first so the
// bound bites early:
//
// 1. Symmetry breaking - all still-empty buckets are interchangeable, so only the
//    first of them is ever tried.
// 2. Branch and bound - a bucket whose new total would already meet or exceed the
//    best answer so far cannot lead anywhere better.
//
// The strategies differ only in who owns the recursion: the baseline is the
// hand-rolled choose/recurse/unchoose loop you would write with nothing but the
// BCL, while the composed arm hands those same five steps to this repo's
// Backtrack.Search (the void, keep-enumerating overload), where Candidates is where
// all pruning lives - the shape Backtrack.cs's own doc comment states.
internal static class FairDistributionOfCookiesSolution
{
    // The textbook arm: explicit recursion over a shared int[] of running totals,
    // with best carried by ref. No primitive from this repo appears in it.
    public static int DistributeCookiesByRecursiveBacktracking(int[] cookies, int childCount)
    {
        var sorted = SortDescending(cookies);
        var buckets = new int[childCount];
        var best = sorted.Sum();

        PlaceRemainingBags(sorted, buckets, index: 0, ref best);

        return best;
    }

    // The same search expressed as Backtrack.Search's five steps over a Distribution
    // that owns the buckets, the cursor and the incumbent best.
    public static int DistributeCookiesByBacktrackSearch(int[] cookies, int childCount)
    {
        var sorted = SortDescending(cookies);
        var state = new Distribution(sorted, childCount);

        Backtrack.Search(
            state,
            isSolution: s => s.IsComplete,
            candidates: s => s.IsComplete ? NoCandidates() : s.CandidateChildren(),
            choose: (s, child) => s.Place(child),
            unchoose: (s, child) => s.Remove(child),
            onSolution: s => s.RecordIfBetter());

        return state.Best;
    }

    // Every bag is placed, so the search has no children left to offer.
    private static IEnumerable<int> NoCandidates() => [];

    private static void PlaceRemainingBags(int[] sorted, int[] buckets, int index, ref int best)
    {
        if (index == sorted.Length)
        {
            var max = buckets.Max();

            if (max < best)
            {
                best = max;
            }

            return;
        }

        TryEachBucket(sorted, buckets, index, ref best);
    }

    // Offer bag `index` to every bucket the two prunes still allow, recursing on the
    // remaining bags and undoing each placement on the way back out. `sawEmpty` is the
    // symmetry break carried across the scan: the still-empty buckets are
    // interchangeable, so only the first of them is ever tried.
    private static void TryEachBucket(int[] sorted, int[] buckets, int index, ref int best)
    {
        var sawEmpty = false;

        for (var child = 0; child < buckets.Length; child++)
        {
            var isEmpty = buckets[child] == 0;

            if (isEmpty && sawEmpty)
            {
                continue;
            }

            sawEmpty |= isEmpty;

            if (buckets[child] + sorted[index] >= best)
            {
                continue;
            }

            buckets[child] += sorted[index];
            PlaceRemainingBags(sorted, buckets, index + 1, ref best);
            buckets[child] -= sorted[index];
        }
    }

    // Largest bag first: the branch-and-bound cut only pays off once the big bags
    // are committed, so this ordering is part of both strategies, not a tweak to one.
    private static int[] SortDescending(int[] cookies)
    {
        var sorted = (int[])cookies.Clone();
        Array.Sort(sorted);
        Array.Reverse(sorted);
        return sorted;
    }

    // The mutable state Backtrack.Search threads through choose/unchoose. Meaningless
    // outside LC 2305 - it encodes this problem's buckets and its two prunes - so it
    // stays beside the solution rather than becoming a shared type (§17.3).
    private sealed class Distribution(int[] cookies, int childCount)
    {
        private readonly int[] _buckets = new int[childCount];
        private int _index;

        public bool IsComplete => _index == cookies.Length;

        public int Best { get; private set; } = cookies.Sum();

        public IEnumerable<int> CandidateChildren()
        {
            var sawEmpty = false;

            for (var child = 0; child < _buckets.Length; child++)
            {
                if (_buckets[child] == 0)
                {
                    if (sawEmpty)
                    {
                        continue;
                    }

                    sawEmpty = true;
                }

                if (_buckets[child] + cookies[_index] < Best)
                {
                    yield return child;
                }
            }
        }

        public void Place(int child)
        {
            _buckets[child] += cookies[_index];
            _index++;
        }

        public void Remove(int child)
        {
            _index--;
            _buckets[child] -= cookies[_index];
        }

        public void RecordIfBetter()
        {
            var max = _buckets.Max();

            if (max < Best)
            {
                Best = max;
            }
        }
    }
}
