using DSAExperimentation.Algorithms.Backtracking;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DistributeRepeatingIntegers;

// LeetCode 1655. Distribute Repeating Integers: the same k-bucket assignment shape
// PartitionToKEqualSumSubsetsTests already uses for LeetCode 698, with orders
// (largest-first, for the same pruning reason) as the items and each distinct
// value's frequency in nums as a bucket capacity instead of a fixed target sum -
// this repo's own Backtrack.TrySearch assigns each order to a value whose
// remaining stock still covers it, letting several orders share one value.
public sealed partial class DistributeRepeatingIntegersTests
{
    [Fact]
    public void CanDistribute_SingleOrderFitsHighestStockValue_ReturnsTrue()
    {
        var canDistribute = CanDistribute([1, 2, 3, 3], [2]);

        Assert.True(canDistribute);
    }

    [Fact]
    public void CanDistribute_ExactStockSplitAcrossTwoOrders_ReturnsTrue()
    {
        var canDistribute = CanDistribute([1, 1, 2, 2], [2, 2]);

        Assert.True(canDistribute);
    }

    [Fact]
    public void CanDistribute_NoSingleValueHasEnoughStockForLargestOrder_ReturnsFalse()
    {
        var canDistribute = CanDistribute([1, 1, 2, 2], [3, 1]);

        Assert.False(canDistribute);
    }

    private static bool CanDistribute(int[] nums, int[] quantity)
    {
        var stock = nums
            .GroupBy(value => value)
            .Select(group => group.Count())
            .ToArray();

        var orders = (int[])quantity.Clone();
        Array.Sort(orders);
        Array.Reverse(orders);

        var state = new State(orders, stock);

        return Backtrack.TrySearch<State, int>(state, new BacktrackingSteps<State, int>(
            IsSolution: s => s.Index == orders.Length,
            Candidates: s => s.Index == orders.Length ? [] : Enumerable.Range(0, stock.Length).Where(s.CanPlace),
            Choose: (s, value) => s.Place(value),
            Unchoose: (s, value) => s.Remove(value),
            OnSolution: _ => true));
    }

    private sealed class State(int[] orders, int[] stock)
    {
        private readonly int[] _remaining = (int[])stock.Clone();

        public int Index { get; private set; }

        public bool CanPlace(int value) => _remaining[value] >= orders[Index];

        public void Place(int value)
        {
            _remaining[value] -= orders[Index];
            Index++;
        }

        public void Remove(int value)
        {
            Index--;
            _remaining[value] += orders[Index];
        }
    }
}
