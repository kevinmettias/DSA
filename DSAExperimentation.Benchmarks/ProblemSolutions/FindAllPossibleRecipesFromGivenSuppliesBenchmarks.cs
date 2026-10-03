using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.FindAllPossibleRecipesFromGivenSupplies;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindAllPossibleRecipesFromGivenSuppliesSolution's, the
// same methods FindAllPossibleRecipesFromGivenSuppliesSolutionTests proves correct. The
// workload is a straight-line dependency chain (recipe i needs recipe i-1), which is
// where the naive sweep's rescanning costs the most: it has to walk every
// still-unmade recipe again after each pass, while Kahn's algorithm touches each
// dependency edge once. Both arms now build and return LeetCode's actual answer -
// the list of makeable recipes - where the previous arms only counted. LC 2115 caps
// the recipes at 100 and spells every name in lowercase letters, so the larger
// RecipeCount is that cap and recipe i is "r" followed by LowercaseNames.Of(i).
public class FindAllPossibleRecipesFromGivenSuppliesBenchmarks
{
    private const string InitialSupply = "s";

    private const string RecipePrefix = "r";

    private string[] _recipes = [];

    private string[][] _ingredients = [];
    private string[] _supplies = [];
    [Params(10, 100)]
    public int RecipeCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _recipes = Enumerable.Range(0, RecipeCount).Select(RecipeName).ToArray();
        _ingredients = new string[RecipeCount][];
        _ingredients[0] = [InitialSupply];
        for (var i = 1; i < RecipeCount; i++)
        {
            _ingredients[i] = [_recipes[i - 1]];
        }

        _supplies = [InitialSupply];
    }

    private static string RecipeName(int index) => RecipePrefix + LowercaseNames.Of(index);

    [Benchmark(Baseline = true)]
    public List<string> FixedPointSweep() =>
        FindAllPossibleRecipesFromGivenSuppliesSolution
            .FindAllRecipesByFixedPointSweep(_recipes, _ingredients, _supplies);

    [Benchmark]
    public List<string> KahnsAlgorithm() =>
        FindAllPossibleRecipesFromGivenSuppliesSolution
            .FindAllRecipesByKahnsAlgorithm(_recipes, _ingredients, _supplies);
}
