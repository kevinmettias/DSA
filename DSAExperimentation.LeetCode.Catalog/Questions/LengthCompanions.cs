namespace DSAExperimentation.LeetCode.Catalog.Questions;

// C's lengths of an array input, which LeetCode's metaData lists beside the array for its C signature and its
// call scripts never pass: an integer named for an array listed before it, as numsSize follows nums and
// matrixRowSize and matrixColSize follow matrix. CatalogBenchmarkGenerator's DesignShape drops them by the
// same rule.
internal static class LengthCompanions
{
    private const string ArraySuffix = "[]";
    private const string Integer = "integer";

    private static readonly string[] Suffixes = ["Size", "RowSize", "ColSize"];

    public static IReadOnlyList<LeetCodeParameter> Without(IReadOnlyList<LeetCodeParameter> parameters)
    {
        var arrays = parameters.Where(parameter => parameter.Type.EndsWith(ArraySuffix, StringComparison.Ordinal)).Select(parameter => parameter.Name).ToList();

        return [.. parameters.Where(parameter => !IsCompanionOf(arrays, parameter))];
    }

    private static bool IsCompanionOf(List<string> arrays, LeetCodeParameter parameter)
        => parameter.Type == Integer && arrays.Any(array => Suffixes.Any(suffix => parameter.Name == array + suffix));
}
