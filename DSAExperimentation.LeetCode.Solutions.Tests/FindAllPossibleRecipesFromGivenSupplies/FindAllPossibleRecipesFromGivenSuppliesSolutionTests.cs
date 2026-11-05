using DSAExperimentation.LeetCode.Solutions.FindAllPossibleRecipesFromGivenSupplies;

namespace DSAExperimentation.LeetCode.Solutions.Tests.FindAllPossibleRecipesFromGivenSupplies;

// Harness only: both strategies live in
// FindAllPossibleRecipesFromGivenSuppliesSolution and are asserted against the same
// cases - the unsatisfiable raw ingredient, the pure cycle, and a graph whose only
// makeable recipe is declared after the one that depends on it. LeetCode accepts the
// makeable recipes in any order; every case here is one both strategies emit in the
// same order, so the assertion can stay an exact sequence rather than a set
// comparison. LeetCode's published examples run in CatalogCaseTests.
public sealed partial class FindAllPossibleRecipesFromGivenSuppliesSolutionTests
{
    public static TheoryData<string[], string[][], string[], string[]> Cases =>
        new()
        {
            // flour is never supplied, so bread and everything downstream is unmakeable.
            {
                ["bread", "sandwich"],
                [["yeast", "flour"], ["bread", "meat"]],
                ["yeast"],
                []
            },

            // A two-recipe cycle: neither in-degree ever reaches zero.
            {
                ["a", "b"],
                [["b"], ["a"]],
                [],
                []
            },

            // Cake needs batter, which is declared after it, so the recipes can only
            // be made in the opposite order to their declaration.
            {
                ["cake", "batter"],
                [["batter"], ["flour"]],
                ["flour"],
                ["batter", "cake"]
            },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void FindAllRecipesByFixedPointSweep_Cases_ReturnsMakeableRecipes(
        string[] recipes, string[][] ingredients, string[] supplies, string[] expected)
    {
        var actual = FindAllPossibleRecipesFromGivenSuppliesSolution.FindAllRecipesByFixedPointSweep(
            recipes, ingredients, supplies);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void FindAllRecipesByKahnsAlgorithm_Cases_ReturnsMakeableRecipes(
        string[] recipes, string[][] ingredients, string[] supplies, string[] expected)
    {
        var actual = FindAllPossibleRecipesFromGivenSuppliesSolution.FindAllRecipesByKahnsAlgorithm(
            recipes, ingredients, supplies);
        Assert.Equal(expected, actual);
    }
}
