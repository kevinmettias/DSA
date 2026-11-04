using System.Numerics;

namespace DSAExperimentation.LeetCode.Solutions.ProductOfArrayExceptSelf;

// LC 238's promise about its input: every product of the other numbers fits in 32 bits. LeetCode's
// judge never poses an input that breaks it. A product past that wraps around in the int the
// strategy answers. Generated cases do not always keep the promise, so a harness that runs them
// asks this first.
internal static class ProductOfArrayExceptSelfPromise
{
    public static bool IsKeptBy(int[] nums)
    {
        var products = new BigInteger[nums.Length];
        BigInteger before = 1;

        for (var index = 0; index < nums.Length; index++)
        {
            products[index] = before;
            before *= nums[index];
        }

        BigInteger after = 1;

        for (var index = nums.Length - 1; index >= 0; index--)
        {
            products[index] *= after;
            after *= nums[index];
        }

        return products.All(product => product >= int.MinValue && product <= int.MaxValue);
    }
}
