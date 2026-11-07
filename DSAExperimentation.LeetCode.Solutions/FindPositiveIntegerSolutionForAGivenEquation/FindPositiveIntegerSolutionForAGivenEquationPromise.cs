namespace DSAExperimentation.LeetCode.Solutions.FindPositiveIntegerSolutionForAGivenEquation;

// LC 1237's function_id names one of its judge's nine hidden CustomFunction
// implementations, and the statement defines only the two its examples name: function_id
// 1 is f(x, y) = x + y and 2 is f(x, y) = x * y (StatedCustomFunctions). It defines no
// others, so a case naming function_id 3 to 9 names a function no harness can build as
// LeetCode does, and judging a strategy on it would judge it against a formula of the
// harness's own guessing. A harness that runs the catalog's cases asks this first; the
// target z plays no part.
internal static class FindPositiveIntegerSolutionForAGivenEquationPromise
{
    public static bool IsKeptBy(int functionId, int targetValue) => StatedCustomFunctions.IsStated(functionId);
}
