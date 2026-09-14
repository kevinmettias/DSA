using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;
using RepoRangeFenwickTree = DSAExperimentation.DataStructures.RangeFenwickTree.RangeFenwickTree<
    long, DSAExperimentation.DataStructures.RangeFenwickTree.ScaledSumOperation<long>>;

namespace DSAExperimentation.LeetCode.MaximizeTheMinimumPoweredCity;

// LeetCode 2528. Maximize the Minimum Powered City: a city's power is the number of
// stations within distance r of it, at most k further stations may be built anywhere,
// and the answer is the largest value the weakest city's power can be raised to.
//
// "Can every city reach at least `target`?" is monotone non-increasing in target -
// once a target is infeasible every larger target stays infeasible - so the answer is
// the last feasible target in [0, sum(stations) + k]. Both strategies decide one
// target the same way, with the classic left-to-right greedy sweep: top up any city
// short of target by building the shortfall as far right as still covers that city,
// which is the placement that helps the most cities still ahead. They differ in how
// they search for the target, and in what they sweep with - the baseline walks the
// targets down one at a time over a plain difference array, the composed strategy
// bisects them with this repo's own BinarySearch.LowerBound over an on-demand
// IRandomAccessSequence<bool> and sweeps with RangeFenwickTree, the same
// search-on-the-answer shape MaximumNumberOfTasksYouCanAssignSolution uses.
internal static class MaximizeTheMinimumPoweredCitySolution
{
    // The prepared input both hoisted overloads take (ARCHITECTURE.md section 17.4):
    // every strategy needs the stations, the radius, the station budget and the
    // highest target worth asking about, so a benchmark can total the stations once in
    // [GlobalSetup] instead of paying for it inside the measured call. A record struct,
    // so it can never be confused with the raw LeetCode-shaped overload.
    internal readonly record struct PoweredCityPlan(int[] Stations, int Range, int ExtraStations, long UpperBound)
    {
        // Every station in the array plus every station still to build, all landing on
        // one city, is as high as the weakest city's power could conceivably go.
        public static PoweredCityPlan From(int[] stations, int r, int k) =>
            new(stations, r, k, stations.Sum(station => (long)station) + k);
    }

    // The textbook answer: try target = upperBound, upperBound - 1, ... and stop at the
    // first one that works, with a BCL long[] difference array carrying the running
    // power as the sweep walks right. Deliberately without this repo's primitives - it
    // is the arm the composed strategy below has to justify itself against.
    public static long MaxPowerByDescendingLinearScan(int[] stations, int r, int k) =>
        MaxPowerByDescendingLinearScan(PoweredCityPlan.From(stations, r, k));

    public static long MaxPowerByDescendingLinearScan(PoweredCityPlan plan)
    {
        for (var target = plan.UpperBound; target >= 0; target--)
        {
            if (FeasibleByDifferenceArray(plan, target))
            {
                return target;
            }
        }

        return 0;
    }

    // This repo's own BinarySearch.LowerBound over the infeasibility sequence: the
    // candidate targets are never materialized, each probe just reruns the greedy
    // sweep, and the leftmost infeasible target sits one past the answer.
    public static long MaxPowerBySequenceLowerBound(int[] stations, int r, int k) =>
        MaxPowerBySequenceLowerBound(PoweredCityPlan.From(stations, r, k));

    public static long MaxPowerBySequenceLowerBound(PoweredCityPlan plan)
    {
        var sequence = new InfeasibleTargetSequence(plan);

        return BinarySearch.LowerBound<bool, InfeasibleTargetSequence>(sequence, true) - 1;
    }

    // The composed sweep: RangeFenwickTree<long, ScaledSumOperation<long>> absorbs both
    // the initial coverage (one RangeAdd per existing station) and every greedy top-up
    // (another RangeAdd), and each city's running power is read back with Query(i, i) -
    // exactly the point read RangeFenwickTree's own doc comment prescribes.
    private static bool FeasibleByRangeFenwickTree(PoweredCityPlan plan, long target)
    {
        var stations = plan.Stations;
        var n = stations.Length;
        var tree = new RepoRangeFenwickTree(n);

        for (var i = 0; i < n; i++)
        {
            tree.RangeAdd(Math.Max(0, i - plan.Range), Math.Min(n - 1, i + plan.Range), stations[i]);
        }

        var remaining = (long)plan.ExtraStations;

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
            var pos = Math.Min(n - 1, i + plan.Range);
            tree.RangeAdd(Math.Max(0, pos - plan.Range), Math.Min(n - 1, pos + plan.Range), need);
        }

        return true;
    }

    // The baseline's sweep, deciding the identical question over a BCL long[]
    // difference array - the range-update structure you reach for without this repo's
    // RangeFenwickTree. Every range this sweep opens starts at or before the city being
    // topped up, so a single left-to-right running total is enough: add the shortfall
    // now and post its removal one past where the new stations stop reaching.
    private static bool FeasibleByDifferenceArray(PoweredCityPlan plan, long target)
    {
        var stations = plan.Stations;
        var n = stations.Length;
        var deltas = new long[n + 1];

        for (var i = 0; i < n; i++)
        {
            deltas[Math.Max(0, i - plan.Range)] += stations[i];
            deltas[Math.Min(n - 1, i + plan.Range) + 1] -= stations[i];
        }

        var remaining = (long)plan.ExtraStations;
        var current = 0L;

        for (var i = 0; i < n; i++)
        {
            current += deltas[i];

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
            current = target;
            var pos = Math.Min(n - 1, i + plan.Range);
            deltas[Math.Min(n - 1, pos + plan.Range) + 1] -= need;
        }

        return true;
    }

    // Get(index) is "target = index cannot be reached by every city" - false up to the
    // answer and true from there on, the monotonicity BinarySearch.LowerBound assumes
    // but never checks. A witness for this problem alone: the feasibility rule is LC
    // 2528's own content, not a general monotone-predicate shape.
    private readonly struct InfeasibleTargetSequence(PoweredCityPlan plan) : IRandomAccessSequence<bool>
    {
        public int Length => (int)plan.UpperBound + 1;

        public bool Get(int index) => !FeasibleByRangeFenwickTree(plan, index);
    }
}
