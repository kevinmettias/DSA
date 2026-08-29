namespace DSAExperimentation.Tests.LeetCodeCoverage.RemoveDuplicatesFromSortedArrayII;

public sealed class RemoveDuplicatesFromSortedArrayIITests
{
    [Fact]
    public void RemoveDuplicates_AllowsAtMostTwoOccurrences_CompactsPrefix()
    {
        int[] nums = [0,0,1,1,1,1,2,3,3];
        var length = RemoveDuplicates(nums);
        Assert.Equal(7, length);
        Assert.Equal([0,0,1,1,2,3,3], nums[..length]);
    }

    private static int RemoveDuplicates(int[] nums)
    {
        var write = 0;
        foreach (var num in nums) if (write < 2 || num != nums[write - 2]) nums[write++] = num;
        return write;
    }
}
