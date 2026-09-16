using DSAExperimentation.LeetCode.MaximumNumberOfDartsInsideOfACircularDartboard;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumNumberOfDartsInsideOfACircularDartboard;

// LeetCode 1453. Maximum Number of Darts Inside of a Circular Dartboard. See
// MaximumNumberOfDartsInsideOfACircularDartboardSolution for the two strategies:
// the same candidate-center sweep buffering into a plain BCL List, and into this
// repo's own DynamicArray.
public sealed partial class MaximumNumberOfDartsInsideOfACircularDartboardTests
{
    public static TheoryData<int[][], int, int> Examples =>
        new()
        {
            // LeetCode example 1: a radius-2 circle centred at the origin has all four
            // darts exactly on its boundary, so containment has to be inclusive.
            { [[-2, 0], [2, 0], [0, 2], [0, -2]], 2, 4 },

            // The pre-migration test's own variant of that example, with the two
            // vertical darts pulled inside the boundary instead of onto it.
            { [[-2, 0], [2, 0], [0, 1], [0, -1]], 2, 4 },

            // LeetCode example 2: the optimal radius-5 circle is centred on neither
            // input dart, which is why dart-only centers are not a correct shortcut.
            { [[-3, 0], [3, 0], [2, 6], [5, 4], [0, 9], [7, 8]], 5, 5 },

            // LeetCode example 3: the same four darts with half the radius - every
            // pair is now more than a diameter apart.
            { [[-2, 0], [2, 0], [0, 2], [0, -2]], 1, 1 },

            // LeetCode example 4: a radius-2 circle catches four of the six darts.
            { [[1, 2], [3, 5], [1, -1], [2, 3], [4, 1], [1, 3]], 2, 4 },

            // A lone dart: the answer is never below one.
            { [[3, 7]], 10, 1 },

            // Darts pairwise further apart than a diameter, so no candidate center
            // from a pair exists at all and every circle covers exactly one.
            { [[0, 0], [100, 100], [-100, -100]], 1, 1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxDartsByListCandidates_LeetCodeExamples_ReturnsMostDartsOneCircleCovers(
        int[][] darts, int radius, int expected)
    {
        var actual = MaximumNumberOfDartsInsideOfACircularDartboardSolution.MaxDartsByListCandidates(darts, radius);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxDartsByDynamicArrayCandidates_LeetCodeExamples_ReturnsMostDartsOneCircleCovers(
        int[][] darts, int radius, int expected)
    {
        var actual = MaximumNumberOfDartsInsideOfACircularDartboardSolution.MaxDartsByDynamicArrayCandidates(
            darts, radius);

        Assert.Equal(expected, actual);
    }
}
