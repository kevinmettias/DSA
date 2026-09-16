using DSAExperimentation.LeetCode.MostFrequentIds;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MostFrequentIds;

// Harness only. Both strategies are MostFrequentIdsSolution's - this file just
// pins them to LeetCode's published examples.
public sealed class MostFrequentIdsTests
{
    public static TheoryData<int[], int[], long[]> Examples =>
        new()
        {
            { [2, 3, 2, 1], [3, 2, -3, 1], [3, 3, 2, 2] },
            { [5, 5, 3], [2, -2, 1], [2, 0, 1] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MostFrequentCountsByBruteForce_LeetCodeExamples_ReturnsMaxCountAfterEachStep(
        int[] nums, int[] freq, long[] expected)
    {
        var actual = MostFrequentIdsSolution.MostFrequentCountsByBruteForce(nums, freq);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MostFrequentCountsByLazyHeap_LeetCodeExamples_ReturnsMaxCountAfterEachStep(
        int[] nums, int[] freq, long[] expected)
    {
        var actual = MostFrequentIdsSolution.MostFrequentCountsByLazyHeap(nums, freq);

        Assert.Equal(expected, actual);
    }
}
