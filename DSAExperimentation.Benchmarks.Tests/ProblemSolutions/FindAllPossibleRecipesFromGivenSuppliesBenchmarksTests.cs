using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindAllPossibleRecipesFromGivenSuppliesBenchmarks (ARCHITECTURE 17.9), for what
// BenchmarkArmsTests cannot pin: which recipes are makeable, known from Setup's construction rather than from either
// arm. Setup builds a straight-line chain "ra", "rb", ... - "r" and each index spelled by LowercaseNames.Of - rooted at
// the single initial supply, so every recipe in it is makeable. LC 2115 leaves the answer's order free, so the recipes
// are compared in sorted order.
public sealed partial class FindAllPossibleRecipesFromGivenSuppliesBenchmarksTests
{
    private const int SmallestRecipeCount = 10;
    private const string RecipePrefix = "r";

    [Fact]
    public void FixedPointSweep_StraightLineDependencyChain_MakesEveryRecipe() =>
        Assert.Equal(EveryRecipe(), BuildHarness().FixedPointSweep().Order(StringComparer.Ordinal));

    [Fact]
    public void KahnsAlgorithm_StraightLineDependencyChain_MakesEveryRecipe() =>
        Assert.Equal(EveryRecipe(), BuildHarness().KahnsAlgorithm().Order(StringComparer.Ordinal));

    // Setup's chain is rooted at the one initial supply and never breaks, so every recipe is makeable.
    private static IEnumerable<string> EveryRecipe() =>
        Enumerable.Range(0, SmallestRecipeCount).Select(index => RecipePrefix + LowercaseNames.Of(index)).Order(StringComparer.Ordinal);

    private static FindAllPossibleRecipesFromGivenSuppliesBenchmarks BuildHarness()
    {
        var harness = new FindAllPossibleRecipesFromGivenSuppliesBenchmarks { RecipeCount = SmallestRecipeCount };
        harness.Setup();

        return harness;
    }
}
