using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Set;
using RecipeFrontier = DSAExperimentation.DataStructures.Queue.Queue<int>;

namespace DSAExperimentation.LeetCode.FindAllPossibleRecipesFromGivenSupplies;

// LeetCode 2115. Find All Possible Recipes from Given Supplies: given recipes, the
// ingredient list each one needs, and an unlimited stock of supplies, report every
// recipe that can eventually be made. An ingredient is satisfied by a supply or by
// another recipe that is itself makeable, so this is a reachability question over
// the recipe-depends-on-recipe graph.
//
// Both strategies answer LeetCode's real shape - the list of makeable recipe names,
// not a count.
internal static class FindAllPossibleRecipesFromGivenSuppliesSolution
{
    // The textbook answer: hold everything currently on hand in a BCL HashSet and
    // rescan every still-unmade recipe until a whole pass makes no progress.
    // Deliberately written without this repo's primitives - it is the arm the Kahn's
    // pass below has to justify itself against. A pass costs O(n) and, when the
    // dependency order runs against the scan order, unlocks a single recipe, so the
    // sweep is O(n^2) where Kahn's algorithm is one O(n) pass.
    public static List<string> FindAllRecipesByFixedPointSweep(
        string[] recipes, string[][] ingredients, string[] supplies)
    {
        var available = new HashSet<string>(supplies);
        var made = new bool[recipes.Length];
        var result = new List<string>();
        var progress = true;

        while (progress)
        {
            progress = false;

            for (var i = 0; i < recipes.Length; i++)
            {
                if (!TryMakeRecipe(i, recipes, ingredients, available, made))
                {
                    continue;
                }

                result.Add(recipes[i]);
                progress = true;
            }
        }

        return result;
    }

    private static bool TryMakeRecipe(
        int i, string[] recipes, string[][] ingredients, HashSet<string> available, bool[] made)
    {
        if (made[i] || !AllIngredientsAvailable(ingredients[i], available))
        {
            return false;
        }

        made[i] = true;
        available.Add(recipes[i]);
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

    // Kahn's algorithm over the recipe-depends-on-recipe graph, built from this
    // repo's own HashMap (recipe name -> index), Set (supply membership) and Queue
    // (the frontier). A recipe naming an ingredient that is neither a supply nor
    // another recipe can never be unblocked by any topological order, so it is
    // simply never seeded into the frontier - the same "leftover in-degree never
    // reaches zero" signal TopologicalSort.cs uses for cycles, reused here for an
    // unsatisfiable raw ingredient.
    //
    // This is a hand-rolled Kahn's pass rather than Algorithms.TopologicalSort's
    // TrySort: that engine wants every vertex supplied upfront under one uniform
    // IGraphTopology edge relation, with no way to say "some prerequisites are free
    // (supplies) and some can never resolve (unknown ingredients)".
    public static List<string> FindAllRecipesByKahnsAlgorithm(
        string[] recipes, string[][] ingredients, string[] supplies)
    {
        var lookup = BuildSupplyLookup(recipes, supplies);
        var graph = CreateEmptyGraph(recipes.Length);
        PopulateDependencyGraph(recipes, ingredients, lookup, graph);

        var frontier = BuildInitialFrontier(graph);

        return DrainFrontier(recipes, frontier, graph);
    }

    private readonly record struct SupplyLookup(HashMap<string, int> RecipeIndex, Set<string> Available);

    private readonly record struct RecipeGraph(List<int>[] Dependents, int[] InDegree, bool[] Blocked);

    private static SupplyLookup BuildSupplyLookup(string[] recipes, string[] supplies)
    {
        var recipeIndex = new HashMap<string, int>();
        for (var i = 0; i < recipes.Length; i++)
        {
            recipeIndex.Set(recipes[i], i);
        }

        return new SupplyLookup(recipeIndex, new Set<string>(supplies));
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
            RecordIngredientDependencies(i, ingredients[i], lookup, graph);
        }
    }

    private static void RecordIngredientDependencies(
        int i, string[] ingredients, SupplyLookup lookup, RecipeGraph graph)
    {
        foreach (var ingredient in ingredients)
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

    private static List<string> DrainFrontier(string[] recipes, RecipeFrontier frontier, RecipeGraph graph)
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
