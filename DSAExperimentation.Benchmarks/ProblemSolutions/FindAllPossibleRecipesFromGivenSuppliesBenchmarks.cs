using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Set;
using RecipeFrontier = DSAExperimentation.DataStructures.Queue.Queue<int>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Find All Possible Recipes From Given Supplies (LC 2115): a naive fixed-point
// sweep (repeatedly rescanning every still-unmade recipe until a full pass makes no
// progress) vs Kahn's algorithm's single pass, using this repo's own
// HashMap/Set/Queue. The fixture is a straight-line dependency chain (recipe i
// needs recipe i-1), the naive sweep's worst case: it unlocks exactly one recipe
// per O(n) pass, O(n^2) total, while Kahn's algorithm still does one O(n) pass.
[MemoryDiagnoser]
public class FindAllPossibleRecipesFromGivenSuppliesBenchmarks
{
    private const string InitialSupply = "s";

    [Params(200, 2_000)]
    public int RecipeCount;

    private string[] _recipes = null!;
    private string[][] _ingredients = null!;
    private string[] _supplies = null!;

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
    public int NaiveFixedPointSweep()
    {
        var available = new HashSet<string>(_supplies);
        var made = new bool[_recipes.Length];
        var madeCount = 0;
        var progress = true;

        while (progress)
        {
            progress = false;

            for (var i = 0; i < _recipes.Length; i++)
            {
                if (!TryMakeRecipe(i, available, made))
                {
                    continue;
                }

                madeCount++;
                progress = true;
            }
        }

        return madeCount;
    }

    private bool TryMakeRecipe(int i, HashSet<string> available, bool[] made)
    {
        if (made[i])
        {
            return false;
        }

        if (!AllIngredientsAvailable(_ingredients[i], available))
        {
            return false;
        }

        made[i] = true;
        available.Add(_recipes[i]);
        return true;
    }

    private static bool AllIngredientsAvailable(string[] ingredients, HashSet<string> available)
    {
        foreach (var ingredient in ingredients)
        {
            if (!available.Contains(ingredient))
            {
                return false;
            }
        }

        return true;
    }

    [Benchmark]
    public int KahnsAlgorithm()
    {
        var recipeIndex = BuildRecipeIndex(_recipes);
        var available = BuildAvailableSet(_supplies);
        var graph = BuildDependencyGraph(recipeIndex, available);
        var frontier = SeedFrontier(graph);

        return ProcessFrontier(frontier, graph);
    }

    private static HashMap<string, int> BuildRecipeIndex(string[] recipes)
    {
        var recipeIndex = new HashMap<string, int>();
        for (var i = 0; i < recipes.Length; i++)
        {
            recipeIndex.Set(recipes[i], i);
        }

        return recipeIndex;
    }

    private static Set<string> BuildAvailableSet(string[] supplies)
    {
        var available = new Set<string>();
        foreach (var supply in supplies)
        {
            available.TryAdd(supply);
        }

        return available;
    }

    private RecipeGraph BuildDependencyGraph(HashMap<string, int> recipeIndex, Set<string> available)
    {
        var graph = new RecipeGraph(_recipes.Length);

        for (var i = 0; i < _recipes.Length; i++)
        {
            RecordIngredientDependencies(i, recipeIndex, available, graph);
        }

        return graph;
    }

    private void RecordIngredientDependencies(
        int i, HashMap<string, int> recipeIndex, Set<string> available, RecipeGraph graph)
    {
        foreach (var ingredient in _ingredients[i])
        {
            if (available.Has(ingredient))
            {
                continue;
            }

            if (recipeIndex.TryGetValue(ingredient, out var prerequisite))
            {
                graph.Dependents[prerequisite].Add(i);
                graph.InDegree[i]++;
            }
            else
            {
                graph.Blocked[i] = true;
            }
        }
    }

    private RecipeFrontier SeedFrontier(RecipeGraph graph)
    {
        var frontier = new RecipeFrontier();
        for (var i = 0; i < _recipes.Length; i++)
        {
            if (graph.InDegree[i] == 0 && !graph.Blocked[i])
            {
                frontier.Enqueue(i);
            }
        }

        return frontier;
    }

    private static int ProcessFrontier(RecipeFrontier frontier, RecipeGraph graph)
    {
        var made = 0;
        while (frontier.TryDequeue(out var current))
        {
            made++;

            foreach (var next in graph.Dependents[current])
            {
                graph.InDegree[next]--;
                if (graph.InDegree[next] == 0 && !graph.Blocked[next])
                {
                    frontier.Enqueue(next);
                }
            }
        }

        return made;
    }

    private sealed class RecipeGraph
    {
        public List<int>[] Dependents { get; }

        public int[] InDegree { get; }

        public bool[] Blocked { get; }

        public RecipeGraph(int count)
        {
            Dependents = new List<int>[count];
            for (var i = 0; i < count; i++)
            {
                Dependents[i] = [];
            }

            InDegree = new int[count];
            Blocked = new bool[count];
        }
    }
}
