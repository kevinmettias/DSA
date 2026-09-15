using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.FindAllPossibleRecipesFromGivenSupplies;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindAllPossibleRecipesFromGivenSuppliesSolution's, the
// same methods FindAllPossibleRecipesFromGivenSuppliesTests proves correct. The
// workload is a straight-line dependency chain (recipe i needs recipe i-1), which is
// where the naive sweep's rescanning costs the most: it has to walk every
// still-unmade recipe again after each pass, while Kahn's algorithm touches each
// dependency edge once. Both arms now build LeetCode's actual answer - the list of
// makeable recipes - and the harness takes .Count, where the previous arms only
// counted.
[MemoryDiagnoser]
public class FindAllPossibleRecipesFromGivenSuppliesBenchmarks
{
    private const string InitialSupply = "s";

    private string[] _recipes = [];

    private string[][] _ingredients = [];
    private string[] _supplies = [];
    [Params(200, 2_000)]
    public int RecipeCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _recipes = Enumerable.Range(0, RecipeCount).Select(i => $"r{i}").ToArray();
        _ingredients = new string[RecipeCount][];
        _ingredients[0] = [InitialSupply];
        for (var i = 1; i < RecipeCount; i++)
        {
            _ingredients[i] = [$"r{i - 1}"];
        }

        _supplies = [InitialSupply];
    }

    [Benchmark(Baseline = true)]
    public int FixedPointSweep() =>
        FindAllPossibleRecipesFromGivenSuppliesSolution
            .FindAllRecipesByFixedPointSweep(_recipes, _ingredients, _supplies).Count;

    [Benchmark]
    public int KahnsAlgorithm() =>
        FindAllPossibleRecipesFromGivenSuppliesSolution
            .FindAllRecipesByKahnsAlgorithm(_recipes, _ingredients, _supplies).Count;
}
