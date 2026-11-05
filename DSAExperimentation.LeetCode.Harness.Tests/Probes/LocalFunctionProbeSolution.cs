namespace DSAExperimentation.LeetCode.Harness.Tests.Probes;

// A solution-shaped class for CatalogStrategyTests, outside the solutions project and never enrolled: one
// strategy that calls a static local function, which compiles to an internal method named for the
// strategy around it and so carries its By.
internal static class LocalFunctionProbeSolution
{
    public static int CountByHelper(int[] values)
    {
        return Count(values);

        static int Count(int[] items) => items.Length;
    }
}
