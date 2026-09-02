using DSAExperimentation.DataStructures.DynamicArray;

namespace DSAExperimentation.LeetCode.ShuffleAnArray;

// LeetCode 384. Shuffle an Array: Solution(nums) stores the original configuration
// once; shuffle() returns a uniformly random permutation of it and reset() returns
// to the stored original. reset() is the identity on the caller's own array and
// shuffle() always starts back from the stored original rather than a previous
// shuffle's result, so both strategies below are pure functions of that array plus
// a Random - no persistent shuffle state is needed, and neither strategy mutates
// the array it is given.
internal static class ShuffleAnArraySolution
{
    // The naive baseline: repeatedly draw a random *remaining* element and remove
    // it from a shrinking pool. BCL List<int>.RemoveAt shifts every trailing
    // element on almost every draw, so this is worst-case O(n^2).
    public static int[] ShuffleByRemoveRandomRemaining(int[] original, Random random)
    {
        var remaining = new List<int>(original);
        var result = new int[remaining.Count];

        for (var i = 0; i < result.Length; i++)
        {
            var index = random.Next(remaining.Count);
            result[i] = remaining[index];
            remaining.RemoveAt(index);
        }

        return result;
    }

    // In-place Fisher-Yates over this repo's own DynamicArray<int>: swap each
    // position against a random earlier-or-equal one, walking from the end down
    // to index 1. O(n), no auxiliary "remaining pool" collection at all.
    public static int[] ShuffleByFisherYatesDynamicArray(int[] original, Random random)
    {
        var values = new DynamicArray<int>();

        foreach (var value in original)
        {
            values.Add(value);
        }

        for (var i = values.Count - 1; i > 0; i--)
        {
            var j = random.Next(i + 1);
            var temp = values.Get(i);
            values.Set(i, values.Get(j));
            values.Set(j, temp);
        }

        var result = new int[values.Count];
        for (var i = 0; i < result.Length; i++)
        {
            result[i] = values.Get(i);
        }

        return result;
    }
}
