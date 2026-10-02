namespace DSAExperimentation.LeetCode.SingleNumber;

// LeetCode 136. Single Number: every element in the array appears exactly twice
// except for one, which appears exactly once - find it in linear time.
//
// Two strategies: an XOR fold cancels every pair in O(1) space, or a set toggles
// each value in and out as it is met a second time, leaving the survivor - O(n)
// space, but the cancelling is explicit rather than relying on XOR's algebra.
internal static class SingleNumberSolution
{
    // Toggle membership: add a value the first time it is seen, remove it the
    // second. Every pair cancels itself, so the one element never removed is the
    // answer. It costs a HashSet and one add/remove per element against the XOR
    // fold's constant space, but it makes the cancelling explicit rather than
    // relying on x ^ x = 0.
    public static int FindUniqueBySetToggling(int[] nums)
    {
        var unpaired = new HashSet<int>();

        foreach (var n in nums)
        {
            if (!unpaired.Add(n))
            {
                unpaired.Remove(n);
            }
        }

        return unpaired.Single();
    }

    // XOR is commutative and its own inverse, so folding it across every element
    // cancels every pair (x ^ x = 0) and leaves only the element with no partner.
    public static int FindUniqueByXorFold(int[] nums) => nums.Aggregate(0, (acc, n) => acc ^ n);
}
