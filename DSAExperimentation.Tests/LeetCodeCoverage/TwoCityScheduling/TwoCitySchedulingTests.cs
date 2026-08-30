using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.TwoCityScheduling;

// LeetCode 1029. Two City Scheduling: the classic greedy - sort the 2n people by
// (aCost - bCost) ascending via this repo's own MergeSort.Sort<Element,TSequence>
// over an ArrayIndexedSequence, the same custom-comparer shape
// QueueReconstructionByHeightTests.cs already exercises - then send the cheapest-
// relative-to-A half to city A and the rest to city B.
public sealed class TwoCitySchedulingTests
{
    [Fact]
    public void TwoCityCostMinimum_LeetCodeExample_ReturnsMinimumCost()
    {
        int[][] costs = [[10, 20], [30, 200], [400, 50], [30, 20]];

        Assert.Equal(110, TwoCityCostMinimum(costs));
    }

    [Fact]
    public void TwoCityCostMinimum_AllEqualCosts_ReturnsCostTimesCount()
    {
        int[][] costs = [[1, 1], [1, 1]];

        Assert.Equal(2, TwoCityCostMinimum(costs));
    }

    private static int TwoCityCostMinimum(int[][] costs)
    {
        var people = costs.Select(c => (ACost: c[0], BCost: c[1])).ToArray();
        var byCostDifferenceAscending = Comparer<(int ACost, int BCost)>.Create(
            (a, b) => (a.ACost - a.BCost).CompareTo(b.ACost - b.BCost));

        MergeSort.Sort<(int ACost, int BCost), ArrayIndexedSequence<(int ACost, int BCost)>>(
            new ArrayIndexedSequence<(int ACost, int BCost)>(people), byCostDifferenceAscending);

        var toCityA = people.Length / 2;
        var total = 0;

        for (var i = 0; i < people.Length; i++)
        {
            total += i < toCityA ? people[i].ACost : people[i].BCost;
        }

        return total;
    }
}
