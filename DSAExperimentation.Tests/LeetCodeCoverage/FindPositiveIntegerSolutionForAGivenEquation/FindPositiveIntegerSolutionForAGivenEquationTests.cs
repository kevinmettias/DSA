using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindPositiveIntegerSolutionForAGivenEquation;

// LeetCode 1237. Find Positive Integer Solution for a Given Equation: the hidden
// CustomFunction is strictly increasing in both x and y, so for any fixed x the row
// f(x, 1..1000) is itself sorted ascending - the same "binary search over a
// computed sequence" shape KokoEatingBananasTests/CapacityToShipPackagesWithinDDaysTests
// already use, just applied per row (BinarySearch.Find for an exact z, not
// LowerBound for a feasibility boundary) instead of over a single monotone
// predicate.
public sealed partial class FindPositiveIntegerSolutionForAGivenEquationTests
{
    private const int Bound = 1000;

    [Fact]
    public void FindSolution_AdditionFunction_ReturnsEveryPairSummingToZ()
    {
        var solutions = FindSolution((x, y) => x + y, z: 5);

        Assert.Equal([(1, 4), (2, 3), (3, 2), (4, 1)], solutions);
    }

    [Fact]
    public void FindSolution_MultiplicationFunction_ReturnsEveryPairMultiplyingToZ()
    {
        var solutions = FindSolution((x, y) => x * y, z: 5);

        Assert.Equal([(1, 5), (5, 1)], solutions);
    }

    [Fact]
    public void FindSolution_NoPairSatisfiesTheEquation_ReturnsEmpty()
    {
        var solutions = FindSolution((x, y) => x + y, z: 2 * Bound + 1);

        Assert.Empty(solutions);
    }

    private static List<(int X, int Y)> FindSolution(Func<int, int, int> function, int z)
    {
        var results = new List<(int X, int Y)>();

        for (var x = 1; x <= Bound; x++)
        {
            var row = new FunctionRowSequence(function, x);
            var index = BinarySearch.Find(row, z);

            if (index is not null)
            {
                results.Add((x, index.Value + 1));
            }
        }

        return results;
    }

    private readonly struct FunctionRowSequence(Func<int, int, int> function, int x) : IRandomAccessSequence<int>
    {
        public int Length => Bound;

        public int Get(int index) => function(x, index + 1);
    }
}
