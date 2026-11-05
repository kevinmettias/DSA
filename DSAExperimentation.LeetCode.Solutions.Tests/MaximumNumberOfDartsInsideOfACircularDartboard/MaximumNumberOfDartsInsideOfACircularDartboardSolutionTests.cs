using DSAExperimentation.LeetCode.Solutions.MaximumNumberOfDartsInsideOfACircularDartboard;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MaximumNumberOfDartsInsideOfACircularDartboard;

// LeetCode 1453. Maximum Number of Darts Inside of a Circular Dartboard. See
// MaximumNumberOfDartsInsideOfACircularDartboardSolution for the two strategies:
// the same candidate-center sweep buffering into a plain BCL List, and into this
// repo's own DynamicArray.
public sealed partial class MaximumNumberOfDartsInsideOfACircularDartboardSolutionTests
{
    public static TheoryData<int[][], int, int> Cases =>
        new()
        {
            // The pre-migration test's own variant of LeetCode's first example (darts
            // at (-2, 0), (2, 0), (0, 2) and (0, -2), radius 2), with the two
            // vertical darts pulled inside the boundary instead of onto it.
            { [[-2, 0], [2, 0], [0, 1], [0, -1]], 2, 4 },

            // Example 3 of an earlier version of the statement, which now publishes
            // only two: the first example's four darts with half the radius - every
            // pair is now more than a diameter apart.
            { [[-2, 0], [2, 0], [0, 2], [0, -2]], 1, 1 },

            // Example 4 of that earlier statement: a radius-2 circle catches four of
            // the six darts.
            { [[1, 2], [3, 5], [1, -1], [2, 3], [4, 1], [1, 3]], 2, 4 },

            // A lone dart: the answer is never below one.
            { [[3, 7]], 10, 1 },

            // Darts pairwise further apart than a diameter, so no candidate center
            // from a pair exists at all and every circle covers exactly one.
            { [[0, 0], [100, 100], [-100, -100]], 1, 1 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaxDartsByListCandidates_Cases_ReturnsMostDartsOneCircleCovers(
        int[][] darts, int radius, int expected)
    {
        var actual = MaximumNumberOfDartsInsideOfACircularDartboardSolution.MaxDartsByListCandidates(darts, radius);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaxDartsByDynamicArrayCandidates_Cases_ReturnsMostDartsOneCircleCovers(
        int[][] darts, int radius, int expected)
    {
        var actual = MaximumNumberOfDartsInsideOfACircularDartboardSolution.MaxDartsByDynamicArrayCandidates(
            darts, radius);

        Assert.Equal(expected, actual);
    }
}
