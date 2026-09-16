using DSAExperimentation.LeetCode.FindConsecutiveIntegersFromADataStream;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindConsecutiveIntegersFromADataStream;

// Harness only. Both strategies are FindConsecutiveIntegersFromADataStreamSolution's -
// the unbounded-history rescan that used to live only in the benchmark's baseline arm,
// and the Deque<int> window with an incremental match count. This file replays each
// example's arrivals one at a time through an IDataStreamStrategy and checks the
// per-call answers, so a failure still names the strategy that broke.
public sealed class FindConsecutiveIntegersFromADataStreamTests
{
    public static TheoryData<int, int, int[], bool[]> Examples =>
        new()
        {
            // LC example 1: the window fills with 4s on the third call, then the 3
            // evicts the oldest match and breaks the run.
            { 4, 3, [4, 4, 4, 3], [false, false, true, false] },

            // Fewer than windowSize arrivals can never be an answer, however many match.
            { 1, 5, [1, 1], [false, false] },

            // A mismatch mid-stream, then the window refills with matches and the
            // answer comes back.
            { 5, 3, [5, 5, 5, 1, 5, 5, 5], [false, false, true, false, false, false, true] },

            // windowSize = 1: every arrival is judged on its own.
            { 2, 1, [1, 2, 2, 3], [false, true, true, false] },

            // The value never arrives, so the window never qualifies.
            { 9, 2, [1, 2, 3], [false, false, false] },

            // A run longer than windowSize stays true as the window slides along it.
            { 7, 2, [7, 7, 7, 7], [false, true, true, true] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void DataStreamByHistoryRescan_LeetCodeExamples_ReportsWhetherTheLastKArrivalsAllMatch(
        int value, int windowSize, int[] arrivals, bool[] expected) =>
        AssertConsecResults(
            new FindConsecutiveIntegersFromADataStreamSolution.DataStreamByHistoryRescan(value, windowSize),
            arrivals,
            expected);

    [Theory]
    [MemberData(nameof(Examples))]
    public void DataStreamByFixedWindow_LeetCodeExamples_ReportsWhetherTheLastKArrivalsAllMatch(
        int value, int windowSize, int[] arrivals, bool[] expected) =>
        AssertConsecResults(
            new FindConsecutiveIntegersFromADataStreamSolution.DataStreamByFixedWindow(value, windowSize),
            arrivals,
            expected);

    private static void AssertConsecResults(
        FindConsecutiveIntegersFromADataStreamSolution.IDataStreamStrategy stream,
        int[] arrivals,
        bool[] expected)
    {
        for (var i = 0; i < arrivals.Length; i++)
        {
            Assert.Equal(expected[i], stream.Consec(arrivals[i]));
        }
    }
}
