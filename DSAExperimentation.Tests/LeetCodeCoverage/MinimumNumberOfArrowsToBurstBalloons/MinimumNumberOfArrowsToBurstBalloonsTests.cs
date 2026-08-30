using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumNumberOfArrowsToBurstBalloons;

// LeetCode 452. Minimum Number of Arrows to Burst Balloons: sort balloons by end
// coordinate with this repo's own MergeSort.Sort<Element,TSequence> over an
// ArrayIndexedSequence - the same custom-comparer shape QueueReconstructionByHeightTests
// exercises - then a single greedy pass. An arrow placed at the end of the
// earliest-ending unburst balloon always bursts the largest possible set of
// remaining balloons, so sorting once is enough (this is NOT IntervalSet.Count:
// transitively-merged overlap groups can still need more than one stabbing point,
// e.g. [1,2],[2,3],[3,4] merge into one interval but need two arrows).
public sealed partial class MinimumNumberOfArrowsToBurstBalloonsTests
{
    [Fact]
    public void FindMinArrowShots_LeetCodeExample_ReturnsTwo()
    {
        int[][] points = [[10, 16], [2, 8], [1, 6], [7, 12]];

        Assert.Equal(2, FindMinArrowShots(points));
    }

    [Fact]
    public void FindMinArrowShots_NoOverlaps_ReturnsOnePerBalloon()
    {
        int[][] points = [[1, 2], [3, 4], [5, 6], [7, 8]];

        Assert.Equal(4, FindMinArrowShots(points));
    }

    [Fact]
    public void FindMinArrowShots_ChainedTouchingIntervals_ReturnsTwoNotOne()
    {
        // [1,2] and [2,3] touch, [2,3] and [3,4] touch, but no single point
        // lies in all three - proves this can't be solved by transitively
        // merging overlaps (IntervalSet.Count would wrongly return 1 here).
        int[][] points = [[1, 2], [2, 3], [3, 4]];

        Assert.Equal(2, FindMinArrowShots(points));
    }

    private static int FindMinArrowShots(int[][] points)
    {
        var items = points.Select(p => (Start: p[0], End: p[1])).ToArray();

        MergeSort.Sort<(int Start, int End), ArrayIndexedSequence<(int Start, int End)>>(
            new ArrayIndexedSequence<(int Start, int End)>(items),
            Comparer<(int Start, int End)>.Create((a, b) => a.End.CompareTo(b.End)));

        var arrows = 1;
        var arrowPosition = items[0].End;

        for (var i = 1; i < items.Length; i++)
        {
            if (items[i].Start > arrowPosition)
            {
                arrows++;
                arrowPosition = items[i].End;
            }
        }

        return arrows;
    }
}
