using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.FenwickTree;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountNumberOfTeams;

// LeetCode 1395. Count Number of Teams: for every soldier j as the middle of a
// team, the team count is leftLess[j]*rightGreater[j] + leftGreater[j]*rightLess[j].
// Each of those four counts comes from a coordinate-compressed sweep - rank via
// BinarySearch.LowerBound/UpperBound over the sorted distinct ratings, tallied in
// a FenwickTree<int, SumOperation<int>> (this repo's own Binary Indexed Tree) built
// once left-to-right and once right-to-left - the same rank-then-Fenwick-sweep
// shape CountOfSmallerNumbersAfterSelfTests already uses, run twice.
public sealed partial class CountNumberOfTeamsTests
{
    [Fact]
    public void NumTeams_ClassicExample_ReturnsMixedAscendingAndDescendingTeams()
    {
        int[] rating = [2, 5, 3, 4, 1];

        var teams = NumTeams(rating);

        Assert.Equal(3, teams);
    }

    [Fact]
    public void NumTeams_NeitherMonotonicDirection_ReturnsZero()
    {
        int[] rating = [2, 1, 3];

        var teams = NumTeams(rating);

        Assert.Equal(0, teams);
    }

    [Fact]
    public void NumTeams_StrictlyIncreasing_ReturnsEveryTriple()
    {
        int[] rating = [1, 2, 3, 4];

        var teams = NumTeams(rating);

        Assert.Equal(4, teams);
    }

    private static int NumTeams(int[] rating)
    {
        var n = rating.Length;
        if (n < 3)
        {
            return 0;
        }

        var sortedDistinct = rating.Distinct().OrderBy(value => value).ToArray();
        var sequence = new ArraySequence<int>(sortedDistinct);

        var (leftLess, leftGreater) = SweepCounts(rating, sequence, sortedDistinct.Length, forward: true);
        var (rightLess, rightGreater) = SweepCounts(rating, sequence, sortedDistinct.Length, forward: false);

        var teams = 0;
        for (var j = 0; j < n; j++)
        {
            teams += (leftLess[j] * rightGreater[j]) + (leftGreater[j] * rightLess[j]);
        }

        return teams;
    }

    // One directional sweep shared by both passes: forward builds leftLess/leftGreater
    // (how many earlier soldiers rank below/above j), forward:false walks right-to-left
    // over the same indices to build rightLess/rightGreater instead.
    private static (int[] Less, int[] Greater) SweepCounts(
        int[] rating, ArraySequence<int> sequence, int distinctCount, bool forward)
    {
        var n = rating.Length;
        var less = new int[n];
        var greater = new int[n];
        var tree = new FenwickTree<int, SumOperation<int>>(distinctCount);
        var seenSoFar = 0;

        for (var step = 0; step < n; step++)
        {
            var j = forward ? step : n - 1 - step;
            var lower = BinarySearch.LowerBound(sequence, rating[j]);
            var upper = BinarySearch.UpperBound(sequence, rating[j]);

            less[j] = lower == 0 ? 0 : tree.PrefixQuery(lower - 1);
            var lessOrEqual = upper == 0 ? 0 : tree.PrefixQuery(upper - 1);
            greater[j] = seenSoFar - lessOrEqual;

            tree.Add(lower, 1);
            seenSoFar++;
        }

        return (less, greater);
    }
}
