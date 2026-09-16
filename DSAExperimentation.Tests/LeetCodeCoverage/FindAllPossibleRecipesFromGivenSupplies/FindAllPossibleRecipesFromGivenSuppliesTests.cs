using DSAExperimentation.LeetCode.FindAllPossibleRecipesFromGivenSupplies;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindAllPossibleRecipesFromGivenSupplies;

// Harness only: both strategies live in
// FindAllPossibleRecipesFromGivenSuppliesSolution and are asserted against the same
// examples - LeetCode's three published ones plus the unsatisfiable raw ingredient,
// the pure cycle, and a graph whose only makeable recipe is declared after the one
// that depends on it. LeetCode accepts the makeable recipes in any order; every case
// here is one both strategies emit in the same order, so the assertion can stay an
// exact sequence rather than a set comparison.
public sealed partial class FindAllPossibleRecipesFromGivenSuppliesTests
{
    public static TheoryData<string[], string[][], string[], string[]> Examples =>
        new()
        {
            // LeetCode example 1: one recipe, all its ingredients supplied.
            {
                ["bread"],
                [["yeast", "flour"]],
                ["yeast", "flour", "corn"],
                ["bread"]
            },

            // LeetCode example 2: sandwich depends on bread.
            {
                ["bread", "sandwich"],
                [["yeast", "flour"], ["bread", "meat"]],
                ["yeast", "flour", "meat"],
                ["bread", "sandwich"]
            },

            // LeetCode example 3: burger depends on sandwich, which depends on bread.
            {
                ["bread", "sandwich", "burger"],
                [["yeast", "flour"], ["bread", "meat"], ["sandwich", "meat", "bread"]],
                ["yeast", "flour", "meat"],
                ["bread", "sandwich", "burger"]
            },

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

            // The only makeable recipe is declared second, so the answer is not in
            // declaration order.
            {
                ["cake", "batter"],
                [["batter"], ["flour"]],
                ["flour"],
                ["batter", "cake"]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindAllRecipesByFixedPointSweep_LeetCodeExamples_ReturnsMakeableRecipes(
        string[] recipes, string[][] ingredients, string[] supplies, string[] expected)
    {
        var actual = FindAllPossibleRecipesFromGivenSuppliesSolution.FindAllRecipesByFixedPointSweep(
            recipes, ingredients, supplies);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindAllRecipesByKahnsAlgorithm_LeetCodeExamples_ReturnsMakeableRecipes(
        string[] recipes, string[][] ingredients, string[] supplies, string[] expected)
    {
        var actual = FindAllPossibleRecipesFromGivenSuppliesSolution.FindAllRecipesByKahnsAlgorithm(
            recipes, ingredients, supplies);
        Assert.Equal(expected, actual);
    }
}
