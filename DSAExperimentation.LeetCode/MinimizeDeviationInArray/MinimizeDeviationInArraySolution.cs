using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.LeetCode.MinimizeDeviationInArray;

// LeetCode 1675. Minimize Deviation in Array: any element may be doubled while it
// is odd and halved while it is even, any number of times; report the smallest
// difference between the array's largest and smallest values that is reachable.
//
// Doubling is a one-shot move - an odd value's only reachable even form is itself
// doubled once, and doubling an even value is always undone by halving it back -
// so normalizing every element to that "doubled if odd" form leaves halving as the
// only remaining move. From there the running maximum can only come down, so the
// answer is the smallest max-min seen while repeatedly halving the current largest
// value until it turns odd and can be reduced no further.
//
// The two strategies differ only in how "the current largest" is found: a full
// rescan of the working values on every step, or this repo's own
// Heap<int, MaxHeapOrder<int>>, which offers it up in O(log n) - the same
// "repeatedly reduce the current max" pairing LastStoneWeightSolution runs for a
// different per-step reduction rule.
internal static class MinimizeDeviationInArraySolution
{
    private const int ParityDivisor = 2;
    private const int EvenizingMultiplier = 2;
    private const int HalvingDivisor = 2;

    // Baseline: an O(n) scan for the largest value on every halving step.
    // Deliberately plain BCL - this is what you would write without this repo.
    public static int MinimumDeviationByLinearRescan(int[] nums)
    {
        var values = new List<int>(nums.Length);
        var min = int.MaxValue;

        foreach (var num in nums)
        {
            var value = EvenForm(num);
            values.Add(value);
            min = Math.Min(min, value);
        }

        var deviation = int.MaxValue;

        while (true)
        {
            var maxIndex = IndexOfLargest(values);
            var max = values[maxIndex];
            deviation = Math.Min(deviation, max - min);

            if (max % ParityDivisor != 0)
            {
                return deviation;
            }

            var half = max / HalvingDivisor;
            min = Math.Min(min, half);
            values[maxIndex] = half;
        }
    }

    // An odd value can only ever be doubled once, so this is the single starting
    // point from which every reachable form of the element is a halving away.
    private static int EvenForm(int num) =>
        num % ParityDivisor == 1 ? num * EvenizingMultiplier : num;

    private static int IndexOfLargest(List<int> values)
    {
        var best = 0;

        for (var i = 1; i < values.Count; i++)
        {
            if (values[i] > values[best])
            {
                best = i;
            }
        }

        return best;
    }

    // Composed: a max-heap offers up the current largest value in O(log n), so the
    // whole reduction costs O(n log n log max) instead of the baseline's O(n^2 log max).
    public static int MinimumDeviationByMaxHeap(int[] nums)
    {
        var heap = new Heap<int, MaxHeapOrder<int>>();
        var min = int.MaxValue;

        foreach (var num in nums)
        {
            var value = EvenForm(num);
            heap.Push(value);
            min = Math.Min(min, value);
        }

        var deviation = int.MaxValue;

        while (true)
        {
            heap.TryPop(out var max);
            deviation = Math.Min(deviation, max - min);

            if (max % ParityDivisor != 0)
            {
                return deviation;
            }

            var half = max / HalvingDivisor;
            min = Math.Min(min, half);
            heap.Push(half);
        }
    }
}
