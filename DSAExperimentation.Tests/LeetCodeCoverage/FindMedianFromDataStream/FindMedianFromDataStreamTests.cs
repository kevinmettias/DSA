using DSAExperimentation.LeetCode.FindMedianFromDataStream;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindMedianFromDataStream;

// Harness only: both strategies are FindMedianFromDataStreamSolution's. Each
// script is a sequence of AddNum values with the expected FindMedian result
// checked after every insert, so a failure still names both the strategy and
// the exact insert that produced the wrong median.
public sealed partial class FindMedianFromDataStreamTests
{
    public static TheoryData<int[], double[]> Examples =>
        new()
        {
            { [1, 2], [1.0, 1.5] },
            { [1, 2, 3], [1.0, 1.5, 2.0] },
            { [5, 4, 3, 2, 1], [5.0, 4.5, 4.0, 3.5, 3.0] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateByTwoHeaps_LeetCodeExamples_TracksRunningMedian(int[] stream, double[] expectedMedians) =>
        Assert.Equal(expectedMedians, RunScript(FindMedianFromDataStreamSolution.CreateByTwoHeaps(), stream));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateBySortOnEveryQuery_LeetCodeExamples_TracksRunningMedian(
        int[] stream, double[] expectedMedians) =>
        Assert.Equal(expectedMedians, RunScript(FindMedianFromDataStreamSolution.CreateBySortOnEveryQuery(), stream));

    private static double[] RunScript(IMedianFinder medianFinder, int[] stream)
    {
        var medians = new double[stream.Length];

        for (var i = 0; i < stream.Length; i++)
        {
            medianFinder.AddNum(stream[i]);
            medians[i] = medianFinder.FindMedian();
        }

        return medians;
    }
}
