using DSAExperimentation.LeetCode.FindingMKAverage;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindingMKAverage;

// Harness only. Both strategies are FindingMKAverageSolution's - this file replays
// LeetCode's published call script against each IMKAverage instance, so a failure
// still names the strategy that broke even though the "input" here is a sequence of
// addElement/calculateMKAverage calls rather than a single argument tuple, the same
// shape AllOneDataStructureTests uses for its own instance-API problem. The
// pre-migration test only proved the Fenwick composition; the sorting baseline
// (previously untested scaffolding inlined in the benchmark) gets that same coverage
// here for the first time. MKAverageOp.Apply is pure dispatch, no averaging logic of
// its own.
public sealed partial class FindingMKAverageTests
{
    public static TheoryData<int, int, MKAverageOp[], int?[]> Examples =>
        new()
        {
            {
                // LeetCode's own example: -1 until the window of 3 fills, then the
                // single middle element of each window.
                3, 1,
                [
                    MKAverageOp.AddElement(3), MKAverageOp.AddElement(1), MKAverageOp.Calculate(),
                    MKAverageOp.AddElement(10), MKAverageOp.Calculate(),
                    MKAverageOp.AddElement(5), MKAverageOp.AddElement(5), MKAverageOp.Calculate(),
                ],
                [null, null, -1, null, 3, null, null, 5]
            },
            {
                // One element into a window of 99 is nowhere near full.
                99, 33,
                [MKAverageOp.AddElement(42), MKAverageOp.Calculate()],
                [null, -1]
            },
            {
                // Every element identical: the trimmed mean is that value, and the
                // count tree's LowerBound lands inside a single 6-deep run.
                6, 1,
                [
                    MKAverageOp.AddElement(7), MKAverageOp.AddElement(7), MKAverageOp.AddElement(7),
                    MKAverageOp.AddElement(7), MKAverageOp.AddElement(7), MKAverageOp.AddElement(7),
                    MKAverageOp.Calculate(),
                ],
                [null, null, null, null, null, null, 7]
            },
            {
                // Eviction is FIFO, not smallest-first: adding 100 drops the 1, so
                // the middle element becomes 3 rather than staying 2.
                3, 1,
                [
                    MKAverageOp.AddElement(1), MKAverageOp.AddElement(2), MKAverageOp.AddElement(3),
                    MKAverageOp.AddElement(100), MKAverageOp.Calculate(),
                ],
                [null, null, null, null, 3]
            },
            {
                // A duplicated value straddling the trim boundary, and a mean that
                // truncates: [1,10,10,100] keeps 10+10 -> 10, then evicting the first
                // 10 leaves [10,1,100,1], which keeps 1+10 -> 5, not 5.5.
                4, 1,
                [
                    MKAverageOp.AddElement(10), MKAverageOp.AddElement(10), MKAverageOp.AddElement(1),
                    MKAverageOp.AddElement(100), MKAverageOp.Calculate(),
                    MKAverageOp.AddElement(1), MKAverageOp.Calculate(),
                ],
                [null, null, null, null, 10, null, 5]
            },
            {
                // Arrival order is unsorted, so the trim has to be by value: the two
                // smallest and two largest of [5,1,4,2,3] leave only 3.
                5, 2,
                [
                    MKAverageOp.AddElement(5), MKAverageOp.AddElement(1), MKAverageOp.AddElement(4),
                    MKAverageOp.AddElement(2), MKAverageOp.AddElement(3), MKAverageOp.Calculate(),
                ],
                [null, null, null, null, null, 3]
            },
            {
                // Repeated queries as the window keeps sliding, so a strategy that
                // only happens to be right on the first full window is caught.
                3, 1,
                [
                    MKAverageOp.AddElement(5), MKAverageOp.AddElement(5), MKAverageOp.AddElement(5),
                    MKAverageOp.Calculate(),
                    MKAverageOp.AddElement(1), MKAverageOp.Calculate(),
                    MKAverageOp.AddElement(2), MKAverageOp.Calculate(),
                ],
                [null, null, null, 5, null, 5, null, 2]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateBySortingSlidingWindow_LeetCodeExamples_ReturnsTrimmedWindowMean(
        int windowSize, int trimCount, MKAverageOp[] operations, int?[] expected) =>
        Assert.Equal(
            expected,
            RunScript(FindingMKAverageSolution.CreateBySortingSlidingWindow(windowSize, trimCount), operations));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateByFenwickOrderStatistics_LeetCodeExamples_ReturnsTrimmedWindowMean(
        int windowSize, int trimCount, MKAverageOp[] operations, int?[] expected) =>
        Assert.Equal(
            expected,
            RunScript(FindingMKAverageSolution.CreateByFenwickOrderStatistics(windowSize, trimCount), operations));

    private static int?[] RunScript(
        FindingMKAverageSolution.IMKAverage mkAverage, MKAverageOp[] operations) =>
        [.. operations.Select(operation => operation.Apply(mkAverage))];

    // One call in an MKAverage script: either an addElement with its value or a
    // calculateMKAverage. Pure dispatch, built via the named factories below so a script
    // reads like the LeetCode call sequence it replays. addElement returns null (no
    // answer to compare); calculateMKAverage returns the actual answer, including -1 for
    // a window that has not filled - the same null-means-"no return value" convention
    // AllOneOp.Apply uses. Nested here rather than left at file scope so the file
    // declares exactly one type.
    public readonly record struct MKAverageOp(int value, bool isCalculate)
    {
        public static MKAverageOp AddElement(int num) => new(num, false);

        public static MKAverageOp Calculate() => new(0, true);

        internal int? Apply(FindingMKAverageSolution.IMKAverage mkAverage)
        {
            if (isCalculate)
            {
                return mkAverage.CalculateMKAverage();
            }

            mkAverage.AddElement(value);
            return null;
        }
    }
}
