using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.FindValueOfMysteriousFunctionClosestToTarget;

// LeetCode 1521. Find a Value of a Mysterious Function Closest to Target: func(l, r) is the
// bitwise AND of arr[l..r], and the answer is the smallest |func(l, r) - target| over every
// subarray.
//
// The two strategies differ in how many subarrays they actually evaluate. The baseline ANDs
// every subarray in place, O(n^2). The composed strategy exploits the fact that extending r by
// one can only clear bits, never set them, so the set of distinct AND values ending at r has
// size O(log(max(arr))) - tracked with this repo's own HashMap<TKey, TValue> as an ad hoc set
// via .Keys (the same "membership via HashMap" idiom Set<Element> itself is built on; HashMap
// is used directly rather than Set because this algorithm must walk every distinct value seen
// so far and Set does not expose enumeration).
internal static class FindValueOfMysteriousFunctionClosestToTargetSolution
{
    // Baseline: extend each left endpoint one element at a time, ANDing as it goes, and score
    // every prefix of that walk. Textbook O(n^2), BCL-only internals.
    public static int ClosestToTargetByBruteForce(int[] arr, int target)
    {
        var best = int.MaxValue;

        for (var l = 0; l < arr.Length; l++)
        {
            var current = arr[l];
            best = Math.Min(best, Math.Abs(current - target));

            for (var r = l + 1; r < arr.Length; r++)
            {
                current &= arr[r];
                best = Math.Min(best, Math.Abs(current - target));
            }
        }

        return best;
    }

    // Carry forward only the distinct AND values of subarrays ending at the previous index;
    // ANDing each of them with the current element (plus the element alone) yields the distinct
    // values ending here. That set stays O(log(max(arr))) wide, so this is O(n log(max(arr))).
    public static int ClosestToTargetByDistinctAndValues(int[] arr, int target)
    {
        var best = int.MaxValue;
        var endingHere = new HashMap<int, bool>();

        foreach (var value in arr)
        {
            var next = new HashMap<int, bool>();
            next.Set(value, true);

            foreach (var previous in endingHere.Keys)
            {
                next.Set(previous & value, true);
            }

            foreach (var candidate in next.Keys)
            {
                best = Math.Min(best, Math.Abs(candidate - target));
            }

            endingHere = next;
        }

        return best;
    }
}
