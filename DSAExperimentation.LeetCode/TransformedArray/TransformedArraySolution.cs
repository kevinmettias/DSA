namespace DSAExperimentation.LeetCode.TransformedArray;

// LeetCode 3379. Transformed Array: replace each element with the value found by
// walking |nums[i]| steps right (nums[i] > 0) or left (nums[i] < 0) from its own
// index, wrapping circularly around the array; a zero stays put.
//
// Nothing here calls for one of this repo's data structures - it is pure
// circular-index arithmetic over the input array itself, so the two strategies
// differ only in how they compute the landing index, not in what they compose.
internal static class TransformedArraySolution
{
    // Baseline: walks one step at a time in the literal direction LeetCode
    // describes, rather than computing the landing index in one shot. O(sum
    // |nums[i]|) - the arm the modulo-formula strategy has to beat.
    public static int[] TransformByStepWalk(int[] nums)
    {
        var n = nums.Length;
        var result = new int[n];

        for (var i = 0; i < n; i++)
        {
            var index = i;
            var direction = Math.Sign(nums[i]);

            for (var taken = 0; taken < Math.Abs(nums[i]); taken++)
            {
                index = (index + direction + n) % n;
            }

            result[i] = nums[index];
        }

        return result;
    }

    // Composed: the landing index in one shot - "(i + shift) mod n", shifted back
    // into [0, n) when the sum went negative. O(n) overall instead of O(sum
    // |nums[i]|). A zero shift needs no special case: it lands back on i either
    // way.
    public static int[] TransformByModuloWalk(int[] nums)
    {
        var n = nums.Length;
        var result = new int[n];

        for (var i = 0; i < n; i++)
        {
            var index = ((i + nums[i]) % n + n) % n;
            result[i] = nums[index];
        }

        return result;
    }
}
