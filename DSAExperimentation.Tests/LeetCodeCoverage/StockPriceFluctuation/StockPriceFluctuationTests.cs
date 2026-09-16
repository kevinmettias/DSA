using DSAExperimentation.LeetCode.StockPriceFluctuation;

namespace DSAExperimentation.Tests.LeetCodeCoverage.StockPriceFluctuation;

// Harness only. Both strategies are StockPriceFluctuationSolution's - this file
// replays LeetCode's published call sequence against each IStockPrice instance, so a
// failure still names the strategy that broke even though the "input" here is a
// sequence of Update/Current/Maximum/Minimum calls rather than a single argument
// tuple, the same shape DetectSquaresTests uses for its own instance-API problem. The
// full-scan baseline (previously untested scaffolding inlined in the benchmark) gets
// the same coverage as the two-heap strategy here for the first time.
public sealed class StockPriceFluctuationTests
{
    public static TheoryData<StockPriceOp[], int?[]> Examples =>
        new()
        {
            {
                // LeetCode's published example: the correction at timestamp 1 drops
                // that record from 10 to 3, which is what moves the maximum to 5.
                [
                    StockPriceOp.Update(1, 10), StockPriceOp.Update(2, 5),
                    StockPriceOp.Current(), StockPriceOp.Maximum(),
                    StockPriceOp.Update(1, 3), StockPriceOp.Maximum(),
                    StockPriceOp.Update(4, 2), StockPriceOp.Minimum(),
                    StockPriceOp.Update(4, 2), StockPriceOp.Minimum(),
                ],
                [null, null, 5, 10, null, 5, null, 2, null, 2]
            },
            {
                // A correction makes the standing maximum stale, and the same
                // correction simultaneously becomes the new minimum.
                [
                    StockPriceOp.Update(1, 100), StockPriceOp.Update(2, 20), StockPriceOp.Maximum(),
                    StockPriceOp.Update(1, 1), StockPriceOp.Maximum(), StockPriceOp.Minimum(),
                ],
                [null, null, 100, null, 20, 1]
            },
            {
                // One record: it is simultaneously the latest, the maximum and the
                // minimum.
                [
                    StockPriceOp.Update(3, 7), StockPriceOp.Current(),
                    StockPriceOp.Maximum(), StockPriceOp.Minimum(),
                ],
                [null, 7, 7, 7]
            },
            {
                // Correcting the LATEST timestamp changes what Current reports.
                [
                    StockPriceOp.Update(1, 10), StockPriceOp.Update(2, 5), StockPriceOp.Current(),
                    StockPriceOp.Update(2, 7), StockPriceOp.Current(),
                ],
                [null, null, 5, null, 7]
            },
            {
                // Updates arrive out of timestamp order, so Current follows the
                // largest timestamp rather than the most recent call.
                [StockPriceOp.Update(5, 10), StockPriceOp.Update(2, 20), StockPriceOp.Current()],
                [null, null, 10]
            },
            {
                // A correction that RAISES a price: the old minimum is stale and the
                // corrected record becomes the new maximum.
                [
                    StockPriceOp.Update(1, 5), StockPriceOp.Update(2, 9), StockPriceOp.Minimum(),
                    StockPriceOp.Update(1, 20), StockPriceOp.Minimum(), StockPriceOp.Maximum(),
                ],
                [null, null, 5, null, 9, 20]
            },
            {
                // Re-stating a price that is already current must not invalidate the
                // entries the heaps already hold for it.
                [
                    StockPriceOp.Update(1, 4), StockPriceOp.Update(1, 4), StockPriceOp.Maximum(),
                    StockPriceOp.Minimum(), StockPriceOp.Current(),
                ],
                [null, null, 4, 4, 4]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateByLazyDeletionTwoHeaps_LeetCodeExamples_TracksCurrentMaximumAndMinimum(
        StockPriceOp[] operations, int?[] expected) =>
        RunScript(StockPriceFluctuationSolution.CreateByLazyDeletionTwoHeaps(), operations, expected);

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateByFullScan_LeetCodeExamples_TracksCurrentMaximumAndMinimum(
        StockPriceOp[] operations, int?[] expected) =>
        RunScript(StockPriceFluctuationSolution.CreateByFullScan(), operations, expected);

    private static void RunScript(
        StockPriceFluctuationSolution.IStockPrice stockPrice, StockPriceOp[] operations, int?[] expected)
    {
        for (var i = 0; i < operations.Length; i++)
        {
            Assert.Equal(expected[i], operations[i].Apply(stockPrice));
        }
    }

    // One call in a StockPrice script: which operation to invoke and, for Update, the
    // record to store. Pure dispatch, built via the named factories below so a script
    // (like Examples above) reads like the LeetCode call sequence it replays. Update
    // returns null (no answer); the three queries return the actual answer - the same
    // null-means-no-return-value convention DetectSquaresOp.Apply uses for its own
    // mutator/query split. Nested here rather than left at file scope so the file
    // declares exactly one type.
    public readonly record struct StockPriceOp(StockPriceOp.StockPriceOpKind kind, int timestamp, int price)
    {
        public static StockPriceOp Update(int timestamp, int price) =>
            new(StockPriceOpKind.Update, timestamp, price);

        public static StockPriceOp Current() => new(StockPriceOpKind.Current, timestamp: 0, price: 0);

        public static StockPriceOp Maximum() => new(StockPriceOpKind.Maximum, timestamp: 0, price: 0);

        public static StockPriceOp Minimum() => new(StockPriceOpKind.Minimum, timestamp: 0, price: 0);

        internal int? Apply(StockPriceFluctuationSolution.IStockPrice stockPrice)
        {
            switch (kind)
            {
                case StockPriceOpKind.Current:
                    return stockPrice.Current();
                case StockPriceOpKind.Maximum:
                    return stockPrice.Maximum();
                case StockPriceOpKind.Minimum:
                    return stockPrice.Minimum();
                default:
                    stockPrice.Update(timestamp, price);
                    return null;
            }
        }

        public enum StockPriceOpKind
        {
            Update,
            Current,
            Maximum,
            Minimum,
        }
    }
}
