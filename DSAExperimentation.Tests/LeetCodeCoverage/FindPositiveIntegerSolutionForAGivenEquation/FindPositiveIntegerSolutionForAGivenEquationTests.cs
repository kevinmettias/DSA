using DSAExperimentation.LeetCode.FindPositiveIntegerSolutionForAGivenEquation;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindPositiveIntegerSolutionForAGivenEquation;

// Harness only: all three strategies live in
// FindPositiveIntegerSolutionForAGivenEquationSolution. Each example supplies a
// stand-in for LeetCode's hidden CustomFunction - strictly increasing in both x and y,
// which is all any strategy is allowed to assume - and every strategy emits its pairs
// with x ascending, so the expectations are stated that way once for all three.
//
// The stand-ins are named types rather than lambdas, because a formula carried as a
// bare callable can say nothing about what it computes; each one's name and its
// Evaluate body are where that now lives. ICustomFunction is internal, so no public
// signature here may name it: not the TheoryData<...> member (CS0053), and not a
// theory's own parameter (CS0051). The first column is therefore typed <object> and
// held as object, and each theory casts it back - the cast being the one place the
// internal type is allowed to appear, and a row's stand-in always being one.
public sealed class FindPositiveIntegerSolutionForAGivenEquationTests
{
    private const int Bound = SearchRange.Bound;

    public static TheoryData<object, int, (int X, int Y)[]> Examples =>
        new()
        {
            // LeetCode example 1: function_id = 1 (x + y), z = 5.
            { new SumFunction(), 5, [(1, 4), (2, 3), (3, 2), (4, 1)] },
            // LeetCode example 2: function_id = 2 (x * y), z = 5.
            { new ProductFunction(), 5, [(1, 5), (5, 1)] },
            // No reachable pair: the largest sum in range is Bound + Bound.
            { new SumFunction(), (2 * Bound) + 1, [] },
            // The extremes of the search space: the single largest sum, and the
            // single smallest product.
            { new SumFunction(), 2 * Bound, [(Bound, Bound)] },
            { new ProductFunction(), 1, [(1, 1)] },
            // A function that is neither of LeetCode's first two, to keep the
            // strategies honest about only using monotonicity.
            { new LinearCombinationFunction(), 20, [(1, 6), (4, 4), (7, 2)] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindSolutionsByBruteForce_LeetCodeExamples_ReturnsEveryPairSatisfyingTheEquation(
        object function, int z, (int X, int Y)[] expected)
    {
        var actual = FindPositiveIntegerSolutionForAGivenEquationSolution
            .FindSolutionsByBruteForce((ICustomFunction)function, z);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindSolutionsByTwoPointer_LeetCodeExamples_ReturnsEveryPairSatisfyingTheEquation(
        object function, int z, (int X, int Y)[] expected)
    {
        var actual = FindPositiveIntegerSolutionForAGivenEquationSolution
            .FindSolutionsByTwoPointer((ICustomFunction)function, z);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindSolutionsByBinarySearchPerRow_LeetCodeExamples_ReturnsEveryPairSatisfyingTheEquation(
        object function, int z, (int X, int Y)[] expected)
    {
        var actual = FindPositiveIntegerSolutionForAGivenEquationSolution
            .FindSolutionsByBinarySearchPerRow((ICustomFunction)function, z);

        Assert.Equal(expected, actual);
    }

    // f(x, y) = x + y, LeetCode's function_id 1.
    private sealed class SumFunction : ICustomFunction
    {
        public int Evaluate(int x, int y) => x + y;
    }

    // f(x, y) = x * y, LeetCode's function_id 2.
    private sealed class ProductFunction : ICustomFunction
    {
        public int Evaluate(int x, int y) => x * y;
    }

    // f(x, y) = 2x + 3y, so a strategy that leaned on anything beyond
    // monotonicity would be caught.
    private sealed class LinearCombinationFunction : ICustomFunction
    {
        public int Evaluate(int x, int y) => (2 * x) + (3 * y);
    }
}
