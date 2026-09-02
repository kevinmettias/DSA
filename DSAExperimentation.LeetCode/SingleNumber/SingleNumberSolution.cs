namespace DSAExperimentation.LeetCode.SingleNumber;

// LeetCode 136. Single Number: every element in the array appears exactly twice
// except for one, which appears exactly once - find it in linear time using only
// constant extra space.
//
// XOR is commutative and its own inverse, so folding it across every element
// cancels every pair (x ^ x = 0) and leaves only the element with no partner.
internal static class SingleNumberSolution
{
    public static int FindUniqueByXorFold(int[] nums) => nums.Aggregate(0, (acc, n) => acc ^ n);
}
