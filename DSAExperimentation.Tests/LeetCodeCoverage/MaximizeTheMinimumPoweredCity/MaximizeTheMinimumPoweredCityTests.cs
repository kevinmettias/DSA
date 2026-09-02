using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;
using RepoRangeFenwickTree = DSAExperimentation.DataStructures.RangeFenwickTree.RangeFenwickTree<long, DSAExperimentation.DataStructures.RangeFenwickTree.ScaledSumOperation<long>>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximizeTheMinimumPoweredCity;

// LeetCode 2528. Maximize the Minimum Powered City: "can every city reach at
// least `target` power after placing at most k extra stations" is monotone
// non-increasing in target (once infeasible, every larger target stays
// infeasible), so the answer is the largest feasible target - found via this
// repo's own BinarySearch.LowerBound over an on-demand IRandomAccessSequence<bool>
// "infeasible" sequence, the same "maximize the answer" shape
// MaximumNumberOfTasksYouCanAssignTests already uses (invert the predicate,
// LowerBound(sequence, true) - 1). Each feasibility check is the classic
// left-to-right greedy sweep - top up any city short of target by placing new
// stations as far right as still covers it - backed by this repo's
// RangeFenwickTree<long, ScaledSumOperation<long>> for both the initial power
// computation (RangeAdd each station's coverage once) and the greedy top-ups
// (RangeAdd again as needed), reading each city's running power via Query(i, i)
// exactly as RangeFenwickTree's own doc comment prescribes for a point read.
public sealed partial class MaximizeTheMinimumPoweredCityTests
{
    [Theory]
    [InlineData(new[] { 1, 2, 4, 5, 0 }, 1, 2, 5)]
    [InlineData(new[] { 4, 4, 4, 4 }, 0, 3, 4)]
    public void MaxPower_LeetCodeExamples_ReturnsMaximizedMinimumPower(int[] stations, int r, int k, long expected)
    {
        var actual = MaxPower(stations, r, k);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void MaxPower_SingleCityNoRange_AllExtraStationsGoToItAlone()
    {
        var actual = MaxPower([3], 0, 5);
        Assert.Equal(8, actual);
    }

    private static long MaxPower(int[] stations, int r, int k)
    {
        var upperBound = stations.Sum(s => (long)s) + k;
        var sequence = new InfeasibleSequence(stations, r, k, upperBound);
        return BinarySearch.LowerBound(sequence, true) - 1;
    }

    private static bool Feasible(int[] stations, int r, long k, long target)
    {
        var n = stations.Length;
        var tree = new RepoRangeFenwickTree(n);

        for (var i = 0; i < n; i++)
        {
            tree.RangeAdd(Math.Max(0, i - r), Math.Min(n - 1, i + r), stations[i]);
        }

        var remaining = k;

        for (var i = 0; i < n; i++)
        {
            var current = tree.Query(i, i);
            if (current >= target)
            {
                continue;
            }

            var need = target - current;
            if (need > remaining)
            {
                return false;
            }

            remaining -= need;
            var pos = Math.Min(n - 1, i + r);
            tree.RangeAdd(Math.Max(0, pos - r), Math.Min(n - 1, pos + r), need);
        }

        return true;
    }

    private readonly struct InfeasibleSequence(int[] stations, int r, int k, long upperBound)
        : IRandomAccessSequence<bool>
    {
        public int Length => (int)upperBound + 1;

        public bool Get(int index) => !Feasible(stations, r, k, index);
    }
}
