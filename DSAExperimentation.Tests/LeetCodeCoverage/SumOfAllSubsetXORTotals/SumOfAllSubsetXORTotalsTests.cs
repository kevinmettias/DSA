using DSAExperimentation.Algorithms.Backtracking;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SumOfAllSubsetXORTotals;

// LeetCode 1863. Sum of All Subset XOR Totals: every node in the choose/explore/
// unchoose recursion tree (including the empty selection) IS a valid subset, so
// IsSolution is unconditionally true and OnSolution adds the running XOR to the total
// - the same exhaustive-enumeration shape SubsetsTests.cs already exercises for LC 78,
// just accumulating a running XOR (toggled on Choose, untoggled on Unchoose) instead
// of collecting snapshots.
public sealed class SumOfAllSubsetXORTotalsTests
{
    [Fact]
    public void SubsetXORSum_TwoElementExample_ReturnsSumOfAllSubsetTotals()
    {
        int[] nums = [1, 3];

        var total = SubsetXORSum(nums);

        Assert.Equal(6, total);
    }

    [Fact]
    public void SubsetXORSum_ThreeElementExample_ReturnsSumOfAllSubsetTotals()
    {
        int[] nums = [5, 1, 6];

        var total = SubsetXORSum(nums);

        Assert.Equal(28, total);
    }

    [Fact]
    public void SubsetXORSum_SixElementExample_ReturnsSumOfAllSubsetTotals()
    {
        int[] nums = [3, 4, 5, 6, 7, 8];

        var total = SubsetXORSum(nums);

        Assert.Equal(480, total);
    }

    private static int SubsetXORSum(int[] nums)
    {
        var total = 0;
        var state = new XorState();

        Backtrack.Search<XorState, int>(
            state,
            isSolution: static _ => true,
            candidates: s => Enumerable.Range(s.NextIndex, nums.Length - s.NextIndex),
            choose: (s, index) =>
            {
                s.RunningXor ^= nums[index];
                s.NextIndex = index + 1;
            },
            unchoose: (s, index) => s.RunningXor ^= nums[index],
            onSolution: s => total += s.RunningXor);

        return total;
    }

    private sealed class XorState
    {
        public int RunningXor { get; set; }

        public int NextIndex { get; set; }
    }
}
