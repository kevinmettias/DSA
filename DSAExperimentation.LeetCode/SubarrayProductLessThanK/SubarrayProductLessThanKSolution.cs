namespace DSAExperimentation.LeetCode.SubarrayProductLessThanK;

// LeetCode 713. Subarray Product Less Than K: count contiguous subarrays whose
// product of elements is strictly less than the product limit. Every element is at
// least 1, so extending a subarray never lowers its product and shrinking it never
// raises it - which is what lets one window slide across the whole array.
//
// The window keeps its product exactly. A sum of logarithms would avoid large
// products, but it is not exact: a subarray whose product equals the limit can sum
// to a hair under log(limit) and be counted, as [5, 6] with limit 30 was.
internal static class SubarrayProductLessThanKSolution
{
    // The textbook O(n^2): for every start index, extend the running product
    // right until it stops being < productLimit. Deliberately written without this repo's
    // primitives - it is the arm the composed solution below has to justify
    // itself against.
    public static int CountSubarraysWithProductLessThanKByBruteForce(int[] nums, int productLimit)
    {
        if (productLimit <= 1)
        {
            return 0;
        }

        var count = 0;

        for (var start = 0; start < nums.Length; start++)
        {
            count += CountEndsFrom(nums, start, productLimit);
        }

        return count;
    }

    // Extends the running product right from `start` until it stops being < productLimit;
    // every end index before that is one valid subarray.
    private static int CountEndsFrom(int[] nums, int start, int productLimit)
    {
        var count = 0;
        long product = 1;

        for (var end = start; end < nums.Length; end++)
        {
            product *= nums[end];

            if (product >= productLimit)
            {
                break;
            }

            count++;
        }

        return count;
    }

    // The O(n) window: every subarray ending at `right` that starts inside the window
    // qualifies, so summing the window's length at each right edge counts them all.
    public static int CountSubarraysWithProductLessThanKBySlidingWindow(int[] nums, int productLimit)
    {
        if (productLimit <= 1)
        {
            return 0;
        }

        var window = new ProductWindow(nums, productLimit);
        var count = 0;

        for (var right = 0; right < nums.Length; right++)
        {
            count += window.Advance(right);
        }

        return count;
    }

    // The window's exact product and its left edge. Before a drop the product is at most
    // (productLimit - 1) * 1000, which LC 713's limit of 10^6 keeps far inside a long.
    private sealed class ProductWindow(int[] nums, int productLimit)
    {
        private long _product = 1;
        private int _left;

        // Admits index right, drops elements from the left until the product is under the
        // limit again, and reports the window's length.
        public int Advance(int right)
        {
            _product *= nums[right];

            while (_product >= productLimit)
            {
                _product /= nums[_left];
                _left++;
            }

            return right - _left + 1;
        }
    }
}
