using DSAExperimentation.LeetCode.StatisticsFromALargeSample;

namespace DSAExperimentation.Tests.LeetCodeCoverage.StatisticsFromALargeSample;

// Harness only. Both strategies are StatisticsFromALargeSampleSolution's; this file
// pins them to LeetCode's published examples plus the odd/even-total pair that
// exercises both halves of the median rule.
public sealed partial class StatisticsFromALargeSampleTests
{
    // LeetCode fixes the sample's value range at [0, 255], so every count array is
    // this long however few buckets an example actually fills.
    private const int BucketCount = 256;

    // The mean is a ratio, so expectations like 24/11 are only equal to within a
    // few decimal places.
    private const int StatisticPrecision = 5;

    public static TheoryData<long[], double[]> Examples =>
        new()
        {
            // LeetCode example 1: sample [1,2,2,2,3,3,3,3].
            { Counts((1, 1), (2, 3), (3, 4)), [1.0, 3.0, 19.0 / 8.0, 2.5, 3.0] },

            // LeetCode example 2: sample [1,1,1,1,2,2,2,3,3,4,4].
            { Counts((1, 4), (2, 3), (3, 2), (4, 2)), [1.0, 4.0, 24.0 / 11.0, 2.0, 1.0] },

            // Odd total: the median is the single element at position total/2 + 1.
            { Counts((1, 1), (2, 3), (3, 1)), [1.0, 3.0, 2.0, 2.0, 2.0] },

            // Even total: the median averages the two middle positions, and lands
            // between two distinct values rather than on one of them.
            { Counts((1, 2), (2, 1), (3, 1)), [1.0, 3.0, 1.75, 1.5, 1.0] },

            // A single distinct value: minimum, maximum, mean, median and mode all
            // collapse onto it.
            { Counts((7, 5)), [7.0, 7.0, 7.0, 7.0, 7.0] },

            // Non-adjacent buckets, so the empty buckets between them must neither
            // shift the median nor become the minimum.
            { Counts((0, 3), (200, 3)), [0.0, 200.0, 100.0, 100.0, 0.0] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void ComputeStatisticsBySampleExpansion_LeetCodeExamples_ReturnsMinMaxMeanMedianMode(
        long[] count, double[] expected) =>
        AssertStatistics(expected, StatisticsFromALargeSampleSolution.ComputeStatisticsBySampleExpansion(count));

    [Theory]
    [MemberData(nameof(Examples))]
    public void ComputeStatisticsByCumulativeBinarySearch_LeetCodeExamples_ReturnsMinMaxMeanMedianMode(
        long[] count, double[] expected) =>
        AssertStatistics(expected, StatisticsFromALargeSampleSolution.ComputeStatisticsByCumulativeBinarySearch(count));

    private static void AssertStatistics(double[] expected, double[] actual)
    {
        Assert.Equal(expected.Length, actual.Length);

        for (var i = 0; i < expected.Length; i++)
        {
            Assert.Equal(expected[i], actual[i], StatisticPrecision);
        }
    }

    // States an example as just its non-empty buckets, spread into the full
    // 256-slot count array LeetCode actually passes.
    private static long[] Counts(params (int Value, long Occurrences)[] buckets)
    {
        var count = new long[BucketCount];

        foreach (var (value, occurrences) in buckets)
        {
            count[value] = occurrences;
        }

        return count;
    }
}
