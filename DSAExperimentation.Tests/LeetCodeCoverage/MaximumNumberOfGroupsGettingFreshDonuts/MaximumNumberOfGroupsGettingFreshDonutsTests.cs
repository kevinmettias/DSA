using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumNumberOfGroupsGettingFreshDonuts;

// LeetCode 1815. Maximum Number of Groups Getting Fresh Donuts: a group whose size
// is already a multiple of batchSize can always be served first (adding a multiple
// of batchSize to a running total that starts at 0 keeps that running total at
// residue 0), so every such group is nice for free. The rest only matter through
// their size mod batchSize, and serving ORDER decides which of them land on a
// residue-0 running total - the same "many different serving orders reach the same
// (running residue, remaining remainder counts) state" shape this repo's own
// Memoizer (Algorithms/DynamicProgramming/Memoizer.cs) already drives for
// CountAllPossibleRoutesTests, just over a different state shape here. State is
// encoded as a string ("residue|c1,c2,...") since a bare int[] has no structural
// equality of its own to hand Memoizer's dictionary cache directly.
public sealed partial class MaximumNumberOfGroupsGettingFreshDonutsTests
{
    [Theory]
    // batchSize=3, groups=[1,2,3,4,5,6]: remainders are {0,0,1,1,2,2}; the two
    // remainder-0 groups are free, and exhaustively checking every ordering of the
    // remaining four (two 1s, two 2s) tops out at 2 more nice groups (e.g. serve
    // 1,3,2 -> 1+3 completes a batch, so the following 2 is nice too) - 2+2=4.
    [InlineData(3, new[] { 1, 2, 3, 4, 5, 6 }, 4)]
    // batchSize=4, groups=[1,3,4,6]: remainders are {1,3,0,2}; the remainder-0
    // group is free, and exhaustively checking all 6 orderings of the remaining
    // three (remainders 1, 2, 3) tops out at 2 more nice groups (e.g. 1,3,2: the
    // first is nice by starting fresh, 1+3 completes a batch so 2 is nice too,
    // but no ordering reaches a third) - 1+2=3.
    [InlineData(4, new[] { 1, 3, 4, 6 }, 3)]
    public void MaxHappyGroups_ExhaustivelyVerifiedExamples_ReturnsExpectedCount(int batchSize, int[] groups, int expected)
    {
        var actual = MaxHappyGroups(batchSize, groups);
        Assert.Equal(expected, actual);
    }

    private static int MaxHappyGroups(int batchSize, int[] groups)
    {
        var counts = new int[batchSize];
        foreach (var g in groups)
        {
            counts[g % batchSize]++;
        }

        var niceFromZeroRemainder = counts[0];
        var remainderCounts = counts.Skip(1).ToArray();

        if (remainderCounts.Length == 0 || remainderCounts.All(c => c == 0))
        {
            return niceFromZeroRemainder;
        }

        var initialState = EncodeState(0, remainderCounts);
        var extraNice = Memoizer.Memoize<string, int>(initialState, (state, best) => Solve(state, best, batchSize));

        return niceFromZeroRemainder + extraNice;
    }

    private static int Solve(string state, Func<string, int> best, int batchSize)
    {
        var (residue, counts) = DecodeState(state);

        if (Array.TrueForAll(counts, c => c == 0))
        {
            return 0;
        }

        var context = new DonutServingContext(residue, counts, batchSize);
        var result = 0;

        for (var i = 0; i < counts.Length; i++)
        {
            if (counts[i] == 0)
            {
                continue;
            }

            var candidate = BestAfterServing(context, i, best);
            result = Math.Max(result, candidate);
        }

        return result;
    }

    private static int BestAfterServing(DonutServingContext context, int remainderIndex, Func<string, int> best)
    {
        var remainder = remainderIndex + 1;
        var nice = context.Residue == 0 ? 1 : 0;
        var nextCounts = (int[])context.Counts.Clone();
        nextCounts[remainderIndex]--;
        var nextResidue = (context.Residue + remainder) % context.BatchSize;

        var nextState = EncodeState(nextResidue, nextCounts);
        return nice + best(nextState);
    }

    private readonly record struct DonutServingContext(int Residue, int[] Counts, int BatchSize);

    private static string EncodeState(int residue, int[] counts) => residue + "|" + string.Join(',', counts);

    private static (int Residue, int[] Counts) DecodeState(string state)
    {
        var parts = state.Split('|');
        var counts = Array.ConvertAll(parts[1].Split(','), int.Parse);
        return (int.Parse(parts[0]), counts);
    }
}
