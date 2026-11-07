using DSAExperimentation.LeetCode.Solutions.FindPositiveIntegerSolutionForAGivenEquation;

namespace DSAExperimentation.LeetCode.Solutions.Tests.FindPositiveIntegerSolutionForAGivenEquation;

// Harness only: all three strategies live in
// FindPositiveIntegerSolutionForAGivenEquationSolution. Each example supplies a
// stand-in for LeetCode's hidden CustomFunction - strictly increasing in both x and y,
// which is all any strategy is allowed to assume - and every strategy emits its pairs
// with x ascending, so the expectations are stated that way once for all three.
// LeetCode's two stated functions come from StatedCustomFunctions, as each strategy's
// LeetCode-shaped overload takes them, and each overload is held to the examples by
// function_id.
//
// The stand-ins are named types rather than lambdas, because a formula carried as a
// bare callable can say nothing about what it computes; each one's name and its
// Evaluate body are where that now lives. ICustomFunction is internal, so no public
// signature here may name it: not the TheoryData<...> member (CS0053), and not a
// theory's own parameter (CS0051). The first column is therefore typed <object> and
// held as object, and each theory casts it back - the cast being the one place the
// internal type is allowed to appear, and a row's stand-in always being one.
public sealed partial class FindPositiveIntegerSolutionForAGivenEquationSolutionTests
{
    private const int Bound = SearchRange.Bound;

    private static readonly ICustomFunction Sum = StatedCustomFunctions.Of(StatedCustomFunctions.SumId);

    private static readonly ICustomFunction Product = StatedCustomFunctions.Of(StatedCustomFunctions.ProductId);

    // LeetCode's inputs, function_id and z, and the pairs the judge's answer key holds.
    public static TheoryData<int, int, (int X, int Y)[]> LeetCodeInputs =>
        new()
        {
            // LeetCode example 1.
            { StatedCustomFunctions.SumId, 5, [(1, 4), (2, 3), (3, 2), (4, 1)] },
            // LeetCode example 2.
            { StatedCustomFunctions.ProductId, 5, [(1, 5), (5, 1)] },
            // z at its smallest, 1: no two positive integers sum to it.
            { StatedCustomFunctions.SumId, 1, [] },
            // z at its largest, 100, as a product.
            { StatedCustomFunctions.ProductId, 100, [(1, 100), (2, 50), (4, 25), (5, 20), (10, 10), (20, 5), (25, 4), (50, 2), (100, 1)] },
        };

    public static TheoryData<object, int, (int X, int Y)[]> Examples =>
        new()
        {
            // LeetCode example 1: function_id = 1 (x + y), z = 5.
            { Sum, 5, [(1, 4), (2, 3), (3, 2), (4, 1)] },
            // LeetCode example 2: function_id = 2 (x * y), z = 5.
            { Product, 5, [(1, 5), (5, 1)] },
            // No reachable pair: the largest sum in range is Bound + Bound.
            { Sum, (2 * Bound) + 1, [] },
            // The extremes of the search space: the single largest sum, and the
            // single smallest product.
            { Sum, 2 * Bound, [(Bound, Bound)] },
            { Product, 1, [(1, 1)] },
            // A function that is neither of LeetCode's first two, to keep the
            // strategies honest about only using monotonicity.
            { new LinearCombinationFunction(), 20, [(1, 6), (4, 4), (7, 2)] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindSolutionsByBruteForce_LeetCodeExamples_ReturnsEveryPairSatisfyingTheEquation(
        object function, int targetValue, (int X, int Y)[] expected)
    {
        var actual = FindPositiveIntegerSolutionForAGivenEquationSolution
            .FindSolutionsByBruteForce((ICustomFunction)function, targetValue);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindSolutionsByTwoPointer_LeetCodeExamples_ReturnsEveryPairSatisfyingTheEquation(
        object function, int targetValue, (int X, int Y)[] expected)
    {
        var actual = FindPositiveIntegerSolutionForAGivenEquationSolution
            .FindSolutionsByTwoPointer((ICustomFunction)function, targetValue);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindSolutionsByBinarySearchPerRow_LeetCodeExamples_ReturnsEveryPairSatisfyingTheEquation(
        object function, int targetValue, (int X, int Y)[] expected)
    {
        var actual = FindPositiveIntegerSolutionForAGivenEquationSolution
            .FindSolutionsByBinarySearchPerRow((ICustomFunction)function, targetValue);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(LeetCodeInputs))]
    public void FindSolutionsByBruteForce_LeetCodeShape_ReturnsThePairsTheFunctionIdSolves(
        int functionId, int targetValue, (int X, int Y)[] expected)
        => Assert.Equal(expected, FindPositiveIntegerSolutionForAGivenEquationSolution.FindSolutionsByBruteForce(functionId, targetValue));

    [Theory]
    [MemberData(nameof(LeetCodeInputs))]
    public void FindSolutionsByTwoPointer_LeetCodeShape_ReturnsThePairsTheFunctionIdSolves(
        int functionId, int targetValue, (int X, int Y)[] expected)
        => Assert.Equal(expected, FindPositiveIntegerSolutionForAGivenEquationSolution.FindSolutionsByTwoPointer(functionId, targetValue));

    [Theory]
    [MemberData(nameof(LeetCodeInputs))]
    public void FindSolutionsByBinarySearchPerRow_LeetCodeShape_ReturnsThePairsTheFunctionIdSolves(
        int functionId, int targetValue, (int X, int Y)[] expected)
        => Assert.Equal(expected, FindPositiveIntegerSolutionForAGivenEquationSolution.FindSolutionsByBinarySearchPerRow(functionId, targetValue));

    // f(x, y) = 2x + 3y, so a strategy that leaned on anything beyond
    // monotonicity would be caught.
    private sealed class LinearCombinationFunction : ICustomFunction
    {
        public int Evaluate(int xValue, int yValue) => (2 * xValue) + (3 * yValue);
    }
}
