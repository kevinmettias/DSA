using DSAExperimentation.LeetCode.DataStreamAsDisjointIntervals;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DataStreamAsDisjointIntervals;

// Harness only. Both strategies are DataStreamAsDisjointIntervalsSolution's; this file just
// replays LeetCode's published addNum() call script against each and asserts the disjoint
// intervals reported after every single call, not just the last one - LC 352's own examples are
// stated as a running summary, so Examples carries the expected snapshot after each addNum
// alongside the values, and AssertSequence drives both strategies through the same script.
public sealed class DataStreamAsDisjointIntervalsTests
{
    public static TheoryData<int[], (int Start, int End)[][]> Examples =>
        new()
        {
            {
                [1, 3, 7, 2, 6],
                [
                    [(1, 1)],
                    [(1, 1), (3, 3)],
                    [(1, 1), (3, 3), (7, 7)],
                    [(1, 3), (7, 7)],
                    [(1, 3), (6, 7)],
                ]
            },
            {
                [5, 5],
                [
                    [(5, 5)],
                    [(5, 5)],
                ]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateByIntervalSetMerge_LeetCodeExamples_SummarizesAsDisjointIntervals(
        int[] values, (int Start, int End)[][] expectedAfterEachAdd) =>
        AssertSequence(
            DataStreamAsDisjointIntervalsSolution.CreateByIntervalSetMerge(), values, expectedAfterEachAdd);

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateByFullRebuildEachCall_LeetCodeExamples_SummarizesAsDisjointIntervals(
        int[] values, (int Start, int End)[][] expectedAfterEachAdd) =>
        AssertSequence(
            DataStreamAsDisjointIntervalsSolution.CreateByFullRebuildEachCall(), values, expectedAfterEachAdd);

    private static void AssertSequence(
        DataStreamAsDisjointIntervalsSolution.ISummaryRanges stream,
        int[] values,
        (int Start, int End)[][] expectedAfterEachAdd)
    {
        for (var i = 0; i < values.Length; i++)
        {
            stream.AddNum(values[i]);
            Assert.Equal(expectedAfterEachAdd[i], stream.GetIntervals());
        }
    }
}
