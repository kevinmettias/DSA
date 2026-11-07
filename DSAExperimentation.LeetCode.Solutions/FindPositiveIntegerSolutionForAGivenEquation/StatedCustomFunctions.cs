namespace DSAExperimentation.LeetCode.Solutions.FindPositiveIntegerSolutionForAGivenEquation;

// LC 1237's judge holds nine hidden CustomFunction implementations and takes a
// function_id naming the one to hand findSolution. The statement states the formula of
// only the two its examples name - "The hidden formula for function_id = 1 is f(x, y) =
// x + y" and "for function_id = 2 is f(x, y) = x * y" - and of the other seven says
// nothing beyond their monotonicity, so these two are every CustomFunction a harness can
// build as LeetCode's judge does. Each strategy's LeetCode-shaped overload takes the
// function from here.
internal static class StatedCustomFunctions
{
    // Example 1's function_id.
    public const int SumId = 1;

    // Example 2's function_id.
    public const int ProductId = 2;

    private const string UnstatedFunction =
        "LC 1237's statement states the formula of function_id 1 (x + y) and 2 (x * y) only.";

    private static readonly ICustomFunction Sum = new SumFunction();

    private static readonly ICustomFunction Product = new ProductFunction();

    // Whether the statement states the formula of the function functionId names.
    public static bool IsStated(int functionId) => functionId is SumId or ProductId;

    // The CustomFunction LeetCode's judge hands findSolution for functionId.
    public static ICustomFunction Of(int functionId) => functionId switch
    {
        SumId => Sum,
        ProductId => Product,
        _ => throw new ArgumentOutOfRangeException(nameof(functionId), functionId, UnstatedFunction),
    };

    // f(x, y) = x + y, function_id 1.
    private sealed class SumFunction : ICustomFunction
    {
        public int Evaluate(int xValue, int yValue) => xValue + yValue;
    }

    // f(x, y) = x * y, function_id 2.
    private sealed class ProductFunction : ICustomFunction
    {
        public int Evaluate(int xValue, int yValue) => xValue * yValue;
    }
}
