namespace DSAExperimentation.LeetCode.BurstBalloons;

// The interval DP both strategies below walk needs two sentinel balloons of value 1
// padded onto each end of the array, so a (left, right) range's endpoints are always
// valid "already burst" neighbors, even at the very edges. Wrapping the padded array
// in its own type - rather than handing back a bare int[] - is what keeps the
// LeetCode-shaped overload (raw nums) and the hoisted, already-padded overload from
// colliding on the same int[] signature. This padding scheme answers LC 312 alone,
// which is why it lives beside the solution rather than in Domain.
internal readonly record struct PaddedBalloons(int[] Values)
{
    public static PaddedBalloons FromNums(int[] nums)
    {
        var padded = new int[nums.Length + 2];
        padded[0] = 1;
        padded[^1] = 1;
        Array.Copy(nums, 0, padded, 1, nums.Length);
        return new PaddedBalloons(padded);
    }
}
