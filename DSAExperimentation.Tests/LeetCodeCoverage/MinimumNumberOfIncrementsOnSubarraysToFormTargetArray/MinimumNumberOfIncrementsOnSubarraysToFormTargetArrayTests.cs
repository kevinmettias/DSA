namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumNumberOfIncrementsOnSubarraysToFormTargetArray;

// LeetCode 1526. Minimum Number of Increments on Subarrays to Form a Target Array: each
// operation increments one contiguous subarray by 1, starting from an all-zero array. The
// minimum operation count is exactly the sum of positive rises between consecutive elements
// (target[0] counts as a rise from an implicit leading 0) - a single running-previous-value
// scan with no stronger data structure to reach for, the same "no repo primitive applies"
// category MaximumSubarrayBenchmarks/JumpGame already established for this kind of greedy
// single-pass array problem.
public sealed partial class MinimumNumberOfIncrementsOnSubarraysToFormTargetArrayTests
{
    [Fact]
    public void MinNumberOperations_SinglePeak_ReturnsThree()
    {
        int[] target = [1, 2, 3, 2, 1];

        var operations = MinNumberOperations(target);

        Assert.Equal(3, operations);
    }

    [Fact]
    public void MinNumberOperations_DipThenRise_ReturnsFour()
    {
        int[] target = [3, 1, 1, 2];

        var operations = MinNumberOperations(target);

        Assert.Equal(4, operations);
    }

    [Fact]
    public void MinNumberOperations_MultipleRisesAndFalls_ReturnsSeven()
    {
        int[] target = [3, 1, 5, 4, 2];

        var operations = MinNumberOperations(target);

        Assert.Equal(7, operations);
    }

    private static int MinNumberOperations(int[] target)
    {
        var operations = 0;
        var previous = 0;

        foreach (var level in target)
        {
            if (level > previous)
            {
                operations += level - previous;
            }

            previous = level;
        }

        return operations;
    }
}
