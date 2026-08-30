using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindValueOfMysteriousFunctionClosestToTarget;

// LeetCode 1521. Find a Value of a Mysterious Function Closest to Target: func(l, r) is the
// bitwise AND of arr[l..r]. Extending r by one can only clear bits, never set them, so the set
// of distinct AND values over all subarrays ending at r has size O(log(max(arr))) - tracked here
// with this repo's own HashMap<TKey,TValue> as an ad hoc set via .Keys (the same "membership via
// HashMap" idiom Set<Element> itself is built on; HashMap is used directly here, not Set, because
// this algorithm must walk every distinct value seen so far and Set doesn't expose enumeration).
public sealed partial class FindValueOfMysteriousFunctionClosestToTargetTests
{
    [Fact]
    public void ClosestToTarget_ClassicExample_ReturnsTwo()
    {
        int[] arr = [9, 12, 3, 7, 15];

        var closest = ClosestToTarget(arr, target: 5);

        Assert.Equal(2, closest);
    }

    [Fact]
    public void ClosestToTarget_SingleElementFarFromTarget_ReturnsFullDifference()
    {
        int[] arr = [1000000];

        var closest = ClosestToTarget(arr, target: 1);

        Assert.Equal(999999, closest);
    }

    [Fact]
    public void ClosestToTarget_NoSubarrayMatchesExactly_ReturnsSmallestGap()
    {
        int[] arr = [5, 89, 79, 44];

        var closest = ClosestToTarget(arr, target: 47);

        Assert.Equal(3, closest);
    }

    private static int ClosestToTarget(int[] arr, int target)
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
