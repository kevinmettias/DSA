using DSAExperimentation.LeetCode.FindMedianFromDataStream;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindMedianFromDataStream;

// Harness only: both strategies are FindMedianFromDataStreamSolution's. Each
// script is a sequence of AddNum values with the expected FindMedian result
// checked after every insert, so a failure still names both the strategy and
// the exact insert that produced the wrong median.
public sealed class FindMedianFromDataStreamTests
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
        RunScript(FindMedianFromDataStreamSolution.CreateByTwoHeaps(), stream, expectedMedians);

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateBySortOnEveryQuery_LeetCodeExamples_TracksRunningMedian(
        int[] stream, double[] expectedMedians) =>
        RunScript(FindMedianFromDataStreamSolution.CreateBySortOnEveryQuery(), stream, expectedMedians);

    private static void RunScript(IMedianFinder medianFinder, int[] stream, double[] expectedMedians)
    {
        for (var i = 0; i < stream.Length; i++)
        {
            medianFinder.AddNum(stream[i]);
            Assert.Equal(expectedMedians[i], medianFinder.FindMedian());
        }
    }
}
