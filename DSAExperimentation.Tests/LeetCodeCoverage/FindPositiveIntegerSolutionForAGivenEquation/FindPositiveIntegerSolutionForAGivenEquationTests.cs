using DSAExperimentation.LeetCode.FindPositiveIntegerSolutionForAGivenEquation;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindPositiveIntegerSolutionForAGivenEquation;

// Harness only: all three strategies live in
// FindPositiveIntegerSolutionForAGivenEquationSolution. Each example supplies a
// stand-in for LeetCode's hidden CustomFunction - strictly increasing in both x and y,
// which is all any strategy is allowed to assume - and every strategy emits its pairs
// with x ascending, so the expectations are stated that way once for all three.
public sealed class FindPositiveIntegerSolutionForAGivenEquationTests
{
    private const int Bound = FindPositiveIntegerSolutionForAGivenEquationSolution.Bound;

    public static TheoryData<Func<int, int, int>, int, (int X, int Y)[]> Examples =>
        new()
        {
            // LeetCode example 1: function_id = 1 (x + y), z = 5.
            { (x, y) => x + y, 5, [(1, 4), (2, 3), (3, 2), (4, 1)] },
            // LeetCode example 2: function_id = 2 (x * y), z = 5.
            { (x, y) => x * y, 5, [(1, 5), (5, 1)] },
            // No reachable pair: the largest sum in range is Bound + Bound.
            { (x, y) => x + y, (2 * Bound) + 1, [] },
            // The extremes of the search space: the single largest sum, and the
            // single smallest product.
            { (x, y) => x + y, 2 * Bound, [(Bound, Bound)] },
            { (x, y) => x * y, 1, [(1, 1)] },
            // A function that is neither of LeetCode's first two, to keep the
            // strategies honest about only using monotonicity.
            { (x, y) => (2 * x) + (3 * y), 20, [(1, 6), (4, 4), (7, 2)] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindSolutionsByBruteForce_LeetCodeExamples_ReturnsEveryPairSatisfyingTheEquation(
        Func<int, int, int> function, int z, (int X, int Y)[] expected) =>
        Assert.Equal(
            expected,
            FindPositiveIntegerSolutionForAGivenEquationSolution.FindSolutionsByBruteForce(function, z));

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindSolutionsByTwoPointer_LeetCodeExamples_ReturnsEveryPairSatisfyingTheEquation(
        Func<int, int, int> function, int z, (int X, int Y)[] expected) =>
        Assert.Equal(
            expected,
            FindPositiveIntegerSolutionForAGivenEquationSolution.FindSolutionsByTwoPointer(function, z));

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindSolutionsByBinarySearchPerRow_LeetCodeExamples_ReturnsEveryPairSatisfyingTheEquation(
        Func<int, int, int> function, int z, (int X, int Y)[] expected) =>
        Assert.Equal(
            expected,
            FindPositiveIntegerSolutionForAGivenEquationSolution
                .FindSolutionsByBinarySearchPerRow(function, z));
}
