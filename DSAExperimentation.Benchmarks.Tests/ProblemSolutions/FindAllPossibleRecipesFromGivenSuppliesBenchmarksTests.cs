using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindAllPossibleRecipesFromGivenSuppliesBenchmarks (ARCHITECTURE 17.9): its two
// arms are competing strategies for the same question - a fixed-point sweep that rescans every
// still-unmade recipe after each pass against Kahn's algorithm - so a harness whose arms disagree is
// resolving two different dependency graphs. Both arms build LeetCode's actual answer and hand back
// its count only, which is a proxy: the count equals the number of makeable recipes, so agreement
// witnesses that both resolved the same set SIZE, not that they named the same recipes. Setup builds
// a straight-line chain rooted at the single initial supply, so every recipe in it is makeable and
// the count is decisive; the same RecipeCount must rebuild the same chain.
public sealed partial class FindAllPossibleRecipesFromGivenSuppliesBenchmarksTests
{
    private const int SmallestRecipeCount = 200;

    // Setup's chain is rooted at the one initial supply and never breaks, so every recipe is makeable.
    private const int ExpectedMakeableRecipeCount = SmallestRecipeCount;

    [Fact]
    public void Setup_SameRecipeCount_RebuildsTheSameChain()
    {
        Assert.Equal(ExpectedMakeableRecipeCount, BuildHarness().FixedPointSweep());

        Assert.Equal(BuildHarness().FixedPointSweep(), BuildHarness().FixedPointSweep());
    }

    [Fact]
    public void FixedPointSweep_StraightLineDependencyChain_AgreesWithKahnsAlgorithm()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedMakeableRecipeCount, harness.FixedPointSweep());

        Assert.Equal(harness.KahnsAlgorithm(), harness.FixedPointSweep());
    }

    [Fact]
    public void KahnsAlgorithm_StraightLineDependencyChain_AgreesWithFixedPointSweep()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedMakeableRecipeCount, harness.KahnsAlgorithm());

        Assert.Equal(harness.FixedPointSweep(), harness.KahnsAlgorithm());
    }

    private static FindAllPossibleRecipesFromGivenSuppliesBenchmarks BuildHarness()
    {
        var harness = new FindAllPossibleRecipesFromGivenSuppliesBenchmarks { RecipeCount = SmallestRecipeCount };
        harness.Setup();

        return harness;
    }
}
