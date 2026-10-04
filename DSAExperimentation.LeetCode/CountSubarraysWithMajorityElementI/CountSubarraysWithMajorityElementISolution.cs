using DSAExperimentation.LeetCode.CountSubarraysWithMajorityElementII;

namespace DSAExperimentation.LeetCode.CountSubarraysWithMajorityElementI;

// LeetCode 3737. Count Subarrays With Majority Element I: count contiguous
// subarrays of nums whose majority element (the value occurring strictly more
// than half the subarray's length) equals target.
//
// Map every element to +1 (equals target) or -1 (does not); target is a
// subarray's majority element exactly when that mapped subarray sums to a
// positive value. With prefix[0..n] the running sum, subarray (i, j] sums
// positive iff prefix[j] > prefix[i] - so the whole problem reduces to counting
// index pairs i < j whose prefix sums are strictly increasing, the same
// coordinate-compression-plus-Fenwick-sweep shape
// CountOfRangeSumSolutionTests/CountOfSmallerNumbersAfterSelfSolutionTests already use for LC
// 327/315, just one-sided instead of two-sided.
internal static class CountSubarraysWithMajorityElementISolution
{
    // Textbook O(n^2): fix the start, extend the end one element at a time, and
    // track how many of the extended window's elements equal target directly -
    // the arm the Fenwick prefix-sum sweep below has to beat. LC 3739 owns it
    // (ARCHITECTURE 17.3); at this problem's bound its total fits an int.
    public static int CountByBruteForce(int[] nums, int target) =>
        (int)CountSubarraysWithMajorityElementIISolution.CountByBruteForce(nums, target);

    // Composed: build prefix[0..n] from the +1/-1 mapping, coordinate-compress it, then
    // sweep left to right through a FenwickTree<int, SumOperation<int>> (this repo's own
    // Binary Indexed Tree), counting - before inserting prefix[j] - how many earlier
    // prefix values were strictly smaller. Summing that count across every j is exactly
    // the number of majority subarrays. LC 3739's bound is where that sweep has to carry a
    // 64-bit accumulator, so its arm is the one implementation of it; at n <= 1000 (this
    // problem's bound) the total is at most 1000 * 1001 / 2 = 500,500 subarrays and fits an
    // int, and the narrowing cast is the only difference between the two problems' sweeps.
    public static int CountByFenwickPrefixSum(int[] nums, int target) =>
        (int)CountSubarraysWithMajorityElementIISolution.CountByFenwickPrefixSum(nums, target);
}
