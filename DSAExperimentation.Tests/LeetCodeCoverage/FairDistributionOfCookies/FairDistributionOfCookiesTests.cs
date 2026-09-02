using DSAExperimentation.Algorithms.Backtracking;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FairDistributionOfCookies;

// LeetCode 2305. Fair Distribution of Cookies: this repo's own Backtrack.Search (the
// void, keep-enumerating overload - this is a minimization over every leaf, not a
// first-solution decision problem like PartitionToKEqualSumSubsetsTests') assigns
// each cookie bag, largest first, to one of k children's running totals, recording
// the best (lowest) max-any-child total seen at every complete assignment.
// Candidates carries both prunes: skip every empty bucket after the first (buckets
// are interchangeable while still empty, so trying more than one is redundant), and
// skip any bucket whose new total would already meet-or-exceed the best answer found
// so far (branch-and-bound) - the same "Candidates is where all pruning lives" shape
// Backtrack.cs's own doc comment states.
public sealed partial class FairDistributionOfCookiesTests
{
    [Fact]
    public void DistributeCookies_LeetCodeExampleOne_ReturnsThirtyOne()
    {
        var actual = DistributeCookies([8, 15, 10, 20, 8], k: 2);
        Assert.Equal(31, actual);
    }

    [Fact]
    public void DistributeCookies_LeetCodeExampleTwo_ReturnsSeven()
    {
        var actual = DistributeCookies([6, 1, 3, 2, 2, 4, 1, 2], k: 3);
        Assert.Equal(7, actual);
    }

    [Fact]
    public void DistributeCookies_MoreChildrenThanNeeded_EachChildGetsAtMostOneBag()
    {
        var actual = DistributeCookies([3, 1, 2], k: 3);
        Assert.Equal(3, actual);
    }

    private static int DistributeCookies(int[] cookies, int k)
    {
        var sorted = SortDescending(cookies);
        var state = new State(sorted, k);

        Backtrack.Search(
            state,
            isSolution: s => s.Index == sorted.Length,
            candidates: s => s.Index == sorted.Length ? [] : s.CandidateChildren(),
            choose: (s, child) => s.Place(child),
            unchoose: (s, child) => s.Remove(child),
            onSolution: s => s.RecordIfBetter());

        return state.Best;
    }

    private static int[] SortDescending(int[] cookies)
    {
        var sorted = (int[])cookies.Clone();
        Array.Sort(sorted);
        Array.Reverse(sorted);
        return sorted;
    }

    private sealed class State(int[] cookies, int k)
    {
        private readonly int[] _buckets = new int[k];

        public int Index { get; private set; }

        public int Best { get; private set; } = cookies.Sum();

        public IEnumerable<int> CandidateChildren()
        {
            var sawEmpty = false;

            for (var child = 0; child < k; child++)
            {
                if (_buckets[child] == 0)
                {
                    if (sawEmpty)
                    {
                        continue;
                    }

                    sawEmpty = true;
                }

                if (_buckets[child] + cookies[Index] < Best)
                {
                    yield return child;
                }
            }
        }

        public void Place(int child)
        {
            _buckets[child] += cookies[Index];
            Index++;
        }

        public void Remove(int child)
        {
            Index--;
            _buckets[child] -= cookies[Index];
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
