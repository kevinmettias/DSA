using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Set;
using RecipeFrontier = DSAExperimentation.DataStructures.Queue.Queue<int>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindAllPossibleRecipesFromGivenSupplies;

// LeetCode 2115. Find All Possible Recipes from Given Supplies: Kahn's algorithm
// over the recipe-depends-on-recipe graph, built from this repo's own HashMap
// (recipe name -> index), Set (supply membership), and Queue (the BFS frontier). A
// recipe naming an ingredient that is neither a supply nor another recipe can never
// be unblocked by any topological order, so it is simply never seeded into the
// initial frontier - the same "leftover in-degree never reaches zero" signal
// TopologicalSort.cs already uses for cycles, reused here for an unsatisfiable raw
// ingredient instead.
//
// This is a hand-rolled Kahn's pass rather than the generic
// Algorithms.TopologicalSort.TrySort: that engine requires every vertex to be
// supplied upfront under one uniform IGraphTopology edge relation, with no way to
// express "some prerequisites are free (supplies) and some can never resolve
// (unknown ingredients)" - a mixed-source rule outside its closed contract, the
// same "plain runtime object, not a witness" territory ARCHITECTURE.md's
// DepthFirstSearch example already carves out for an open-ended successor
// relation.
public sealed partial class FindAllPossibleRecipesFromGivenSuppliesTests
{
    [Fact]
    public void FindAllRecipes_ChainedDependencies_ReturnsMakeableRecipesInOrder()
    {
        string[] recipes = ["bread", "sandwich", "burger"];
        string[][] ingredients =
        [
            ["yeast", "flour"],
            ["bread", "meat"],
            ["sandwich", "meat", "bread"],
        ];
        string[] supplies = ["yeast", "flour", "meat"];

        var result = FindAllRecipes(recipes, ingredients, supplies);

        Assert.Equal(["bread", "sandwich", "burger"], result);
    }

    [Fact]
    public void FindAllRecipes_MissingRawIngredient_ExcludesRecipeAndItsDependents()
    {
        string[] recipes = ["bread", "sandwich"];
        string[][] ingredients =
        [
            ["yeast", "flour"],
            ["bread", "meat"],
        ];
        string[] supplies = ["yeast"]; // flour is missing, so bread (and sandwich) can never be made

        var result = FindAllRecipes(recipes, ingredients, supplies);

        Assert.Empty(result);
    }

    private readonly record struct SupplyLookup(HashMap<string, int> RecipeIndex, Set<string> Available);

    private readonly record struct RecipeGraph(List<int>[] Dependents, int[] InDegree, bool[] Blocked);

    private static List<string> FindAllRecipes(string[] recipes, string[][] ingredients, string[] supplies)
    {
        var lookup = BuildSupplyLookup(recipes, supplies);
        var graph = CreateEmptyGraph(recipes.Length);
        PopulateDependencyGraph(recipes, ingredients, lookup, graph);

        var frontier = BuildInitialFrontier(graph);

        return RunKahnsAlgorithm(recipes, frontier, graph);
    }

    private static SupplyLookup BuildSupplyLookup(string[] recipes, string[] supplies)
    {
        var recipeIndex = new HashMap<string, int>();
        for (var i = 0; i < recipes.Length; i++)
        {
            recipeIndex.Set(recipes[i], i);
        }

        var available = new Set<string>();
        foreach (var supply in supplies)
        {
            available.TryAdd(supply);
        }

        return new SupplyLookup(recipeIndex, available);
    }

    private static RecipeGraph CreateEmptyGraph(int recipeCount)
    {
        var dependents = new List<int>[recipeCount];
        for (var i = 0; i < recipeCount; i++)
        {
            dependents[i] = [];
        }

        return new RecipeGraph(dependents, new int[recipeCount], new bool[recipeCount]);
    }

    private static void PopulateDependencyGraph(
        string[] recipes, string[][] ingredients, SupplyLookup lookup, RecipeGraph graph)
    {
        for (var i = 0; i < recipes.Length; i++)
        {
            foreach (var ingredient in ingredients[i])
            {
                if (lookup.Available.Has(ingredient))
                {
                    continue;
                }

                if (lookup.RecipeIndex.TryGetValue(ingredient, out var prerequisite))
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
    }

    private static RecipeFrontier BuildInitialFrontier(RecipeGraph graph)
    {
        var frontier = new RecipeFrontier();
        for (var i = 0; i < graph.InDegree.Length; i++)
        {
            if (graph.InDegree[i] == 0 && !graph.Blocked[i])
            {
                frontier.Enqueue(i);
            }
        }

        return frontier;
    }

    private static List<string> RunKahnsAlgorithm(string[] recipes, RecipeFrontier frontier, RecipeGraph graph)
    {
        var result = new List<string>();
        while (frontier.TryDequeue(out var current))
        {
            result.Add(recipes[current]);

            foreach (var next in graph.Dependents[current])
            {
                graph.InDegree[next]--;
                if (graph.InDegree[next] == 0 && !graph.Blocked[next])
                {
                    frontier.Enqueue(next);
                }
            }
        }

        return result;
    }
}
