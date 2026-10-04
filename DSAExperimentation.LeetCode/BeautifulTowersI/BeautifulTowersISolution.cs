using DSAExperimentation.Algorithms.Searching;

namespace DSAExperimentation.LeetCode.BeautifulTowersI;

// LeetCode 2865. Beautiful Towers I: pick heights[i] in [1, maxHeights[i]] so the
// sequence is a mountain - non-decreasing up to some peak, non-increasing after it -
// and maximize the total. For a fixed peak the optimum is forced: walking away from
// the peak every tower takes the running minimum of the maxHeights seen so far,
// because anything taller breaks monotonicity and anything shorter is wasted height.
//
// The two strategies differ only in how many times that clamped walk runs. The
// baseline re-walks the whole array once per candidate peak (O(n^2)). The composed
// strategy computes the clamped one-sided sum for EVERY index in a single sweep per
// direction, reading each index's nearest not-taller neighbour off one
// NearestBoundary sweep - the same monotonic stack of indices, each popped at most
// once, that LargestRectangleInHistogramSolution applies to rectangle area, applied
// here to clamped sums.
//
// Both strategies take LeetCode's own int[], so neither needs a hoisted
// prepared-input overload - there is no input structure to build.
internal static class BeautifulTowersISolution
{
    // Sweep directions for ClampedRunSums: forward builds the non-decreasing run
    // ending at each index, backward the non-increasing run starting at each index.
    private const int Forward = 1;
    private const int Backward = -1;

    // The textbook brute force: fix each index as the peak, clamp outward in both
    // directions to the running minimum, keep the best total. Deliberately written
    // without this repo's primitives - it is the arm the sweep below has to justify
    // itself against.
    public static long MaximumSumOfHeightsByBruteForce(int[] maxHeights)
    {
        var n = maxHeights.Length;
        var best = 0L;

        for (var peak = 0; peak < n; peak++)
        {
            var mountainSum = BestMountainPeakingAt(maxHeights, peak);

            best = Math.Max(best, mountainSum);
        }

        return best;
    }

    // The best total for the mountain whose peak is fixed at `peak`: walk left and
    // right from it, clamping every tower to the smallest maxHeight seen so far.
    private static long BestMountainPeakingAt(int[] maxHeights, int peak)
    {
        var sum = (long)maxHeights[peak];

        var cap = maxHeights[peak];
        for (var j = peak - 1; j >= 0; j--)
        {
            cap = Math.Min(cap, maxHeights[j]);
            sum += cap;
        }

        cap = maxHeights[peak];
        for (var j = peak + 1; j < maxHeights.Length; j++)
        {
            cap = Math.Min(cap, maxHeights[j]);
            sum += cap;
        }

        return sum;
    }

    // Two sweeps and a join: the best mountain peaking at i is its non-decreasing
    // prefix plus its non-increasing suffix, with maxHeights[i] counted by both and
    // so subtracted once.
    public static long MaximumSumOfHeightsByMonotonicStack(int[] maxHeights)
    {
        var left = ClampedRunSums(maxHeights, Forward);
        var right = ClampedRunSums(maxHeights, Backward);
        var best = 0L;

        for (var i = 0; i < maxHeights.Length; i++)
        {
            best = Math.Max(best, left[i] + right[i] - maxHeights[i]);
        }

        return best;
    }

    // sums[i] = the best total over the side of i the sweep has already covered,
    // every tower clamped to the running cap and maxHeights[i] itself at the
    // boundary. NearestBoundary's or-equal sweep on that side lands on the nearest
    // index whose maxHeight does not exceed maxHeights[i], whose already-clamped sum
    // covers everything beyond it, so the gap in between is a flat run at
    // maxHeights[i] and costs one multiplication. With no such index the run reaches
    // the end of the array.
    //
    // The forward and backward passes are exact mirrors - offsets negate, the
    // boundary moves from just before index 0 to just past the last index - so one
    // loop keyed on the step direction is the whole of both. Each sums[i] reads only
    // the sum at its boundary, which the loop's direction has already filled in.
    private static long[] ClampedRunSums(int[] maxHeights, int step)
    {
        var n = maxHeights.Length;
        var sums = new long[n];
        var start = step == Forward ? 0 : LastIndex(n);
        var boundary = step == Forward ? -1 : n;
        var nearestNotTaller = step == Forward
            ? NearestBoundary.SmallerOrEqualToTheLeft(maxHeights, boundary)
            : NearestBoundary.SmallerOrEqualToTheRight(maxHeights, boundary);

        for (var i = start; i >= 0 && i < n; i += step)
        {
            var nearestShorter = nearestNotTaller[i];
            var runSoFar = nearestShorter == boundary ? 0L : SumAt(sums, nearestShorter);

            sums[i] = runSoFar + ((long)maxHeights[i] * (i - nearestShorter) * step);
        }

        return sums;
    }

    private static int LastIndex(int length) => length - 1;

    private static long SumAt(long[] sums, int index) => sums[index];
}
