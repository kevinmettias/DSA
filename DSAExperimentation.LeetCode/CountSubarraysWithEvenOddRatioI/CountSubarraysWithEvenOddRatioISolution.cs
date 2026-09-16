using DSAExperimentation.LeetCode.CountSubarraysWithEvenOddRatioII;

namespace DSAExperimentation.LeetCode.CountSubarraysWithEvenOddRatioI;

// LeetCode 4011. Count Subarrays With Even Odd Ratio I: for a subarray with x
// even elements and y odd elements, it is valid when y > 0 and x/y <= a/b -
// compared via the equivalent cross-multiplied integer inequality x*b <= y*a
// so no floating point ever enters the comparison. n <= 1000 keeps the
// O(n^2) enumeration cheap enough to serve as the textbook baseline here.
//
// Rearranged the other way, a*y - b*x >= 0 is a subarray-sum sign question:
// score every odd element +a and every even element -b, and count index
// pairs (L, R) whose prefix sums satisfy prefix[L] <= prefix[R]. That is
// exactly the coordinate-compression-plus-Fenwick-sweep shape
// CountOfRangeSumTests already uses for LC 327. LC 4013 (same rule, n up to
// 1e5) is where that sweep lives, since its bound is the one whose count needs
// a 64-bit accumulator; this class reads its own total off that implementation.
internal static class CountSubarraysWithEvenOddRatioISolution
{
    // Every subarray scanned directly, extending y one element at a time -
    // O(n^2), BCL only. The arm the Fenwick sweep below has to beat.
    public static int CountByBruteForce(int[] nums, int a, int b)
    {
        var count = 0L;

        for (var left = 0; left < nums.Length; left++)
        {
            var odd = 0;

            for (var right = left; right < nums.Length; right++)
            {
                odd += nums[right] % 2;
                var even = right - left + 1 - odd;

                if (odd > 0 && (long)even * b <= (long)odd * a)
                {
                    count++;
                }
            }
        }

        return (int)count;
    }

    // LC 4013's own sweep - build the +a/-b weighted prefix sums, coordinate-compress
    // them, then query this repo's FenwickTree<int, SumOperation<int>> for how many
    // earlier prefixes are <= prefix[R] before inserting prefix[R] itself - kept there
    // because its bound is the one that needs the count to stay a long. At n <= 1000 the
    // total cannot leave int range, so narrowing that arm is the whole difference between
    // the two problems and a second copy of the loop would buy nothing.
    public static int CountByFenwickPrefixSweep(int[] nums, int a, int b) =>
        (int)CountSubarraysWithEvenOddRatioIISolution.CountByFenwickPrefixSweep(nums, a, b);
}
