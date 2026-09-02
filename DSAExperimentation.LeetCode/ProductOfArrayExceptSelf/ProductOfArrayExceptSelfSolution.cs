namespace DSAExperimentation.LeetCode.ProductOfArrayExceptSelf;

// LeetCode 238. Product of Array Except Self: for every index, the product of
// every other element - in O(n) and without division, since LC's constraints
// allow zero operands, which a division-based approach cannot handle.
//
// One left-to-right pass fills each slot with the running product of every
// element before it; one right-to-left pass then multiplies in the running
// product of every element after it. Every index is touched twice, no
// division, and the result buffer doubles as the only extra memory used.
internal static class ProductOfArrayExceptSelfSolution
{
    public static int[] ProductExceptSelfByPrefixSuffixPass(int[] nums)
    {
        var result = new int[nums.Length];
        var prefix = 1;

        for (var i = 0; i < nums.Length; i++)
        {
            result[i] = prefix;
            prefix *= nums[i];
        }

        var suffix = 1;

        for (var i = nums.Length - 1; i >= 0; i--)
        {
            result[i] *= suffix;
            suffix *= nums[i];
        }

        return result;
    }
}
