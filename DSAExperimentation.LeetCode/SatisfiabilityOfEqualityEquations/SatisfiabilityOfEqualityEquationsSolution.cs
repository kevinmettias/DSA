using DSAExperimentation.DataStructures.DisjointSet;

namespace DSAExperimentation.LeetCode.SatisfiabilityOfEqualityEquations;

// LeetCode 990. Satisfiability of Equality Equations: given equations of the form
// "a==b" / "a!=b" over single lowercase variables, decide whether some assignment
// satisfies all of them at once.
//
// Equality is an equivalence relation, so the "==" equations partition the 26
// variables into components and the answer is "no two sides of a '!=' equation land
// in the same component". The textbook baseline builds an adjacency list from the
// equalities and re-runs a fresh BFS reachability query per inequality; the composed
// strategy replaces both halves with this repo's own DisjointSet over the fixed
// 26-slot alphabet - O(a(26)) Union per equality, O(a(26)) IsConnected per
// inequality, and no per-query allocation at all.
internal static class SatisfiabilityOfEqualityEquationsSolution
{
    // The problem fixes single lowercase variable names, so an equation is always
    // exactly four characters: variable, operator, operator, variable.
    private const int AlphabetSize = 26;
    private const int SecondVariableIndex = 3;

    // The naive baseline: a Dictionary-of-Lists adjacency graph over the equality
    // equations, then one freshly allocated HashSet+Queue BFS per inequality asking
    // whether its two variables are connected. Deliberately written with BCL
    // primitives only - it is the arm the composed solution has to justify itself
    // against.
    public static bool EquationsPossibleByAdjacencyBfs(string[] equations)
    {
        var (adjacency, inequalities) = BuildAdjacencyGraph(equations);

        return !HasContradiction(adjacency, inequalities);
    }

    private static (Dictionary<char, List<char>> Adjacency, List<(char First, char Second)> Inequalities)
        BuildAdjacencyGraph(string[] equations)
    {
        var adjacency = new Dictionary<char, List<char>>();
        var inequalities = new List<(char First, char Second)>();

        foreach (var equation in equations)
        {
            var first = equation[0];
            var second = equation[SecondVariableIndex];

            if (equation[1] == '=')
            {
                AddAdjacencyEdge(adjacency, first, second);
            }
            else
            {
                inequalities.Add((first, second));
            }
        }

        return (adjacency, inequalities);
    }

    private static void AddAdjacencyEdge(Dictionary<char, List<char>> adjacency, char first, char second)
    {
        if (!adjacency.TryGetValue(first, out var firstNeighbors))
        {
            firstNeighbors = [];
            adjacency[first] = firstNeighbors;
        }

        if (!adjacency.TryGetValue(second, out var secondNeighbors))
        {
            secondNeighbors = [];
            adjacency[second] = secondNeighbors;
        }

        firstNeighbors.Add(second);
        secondNeighbors.Add(first);
    }

    private static bool HasContradiction(
        Dictionary<char, List<char>> adjacency, List<(char First, char Second)> inequalities)
    {
        foreach (var (first, second) in inequalities)
        {
            if (IsReachable(adjacency, first, second))
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsReachable(Dictionary<char, List<char>> adjacency, char start, char target)
    {
        if (start == target)
        {
            return true;
        }

        var frontier = CreateFrontier(start);

        while (frontier.Queue.Count > 0)
        {
            var current = frontier.Queue.Dequeue();

            if (TryExpandFrontier(adjacency, current, target, frontier))
            {
                return true;
            }
        }

        return false;
    }

    private static SearchFrontier CreateFrontier(char start)
    {
        var visited = new HashSet<char> { start };
        var queue = new Queue<char>();
        queue.Enqueue(start);

        return new SearchFrontier(visited, queue);
    }

    private static bool TryExpandFrontier(
        Dictionary<char, List<char>> adjacency, char current, char target, SearchFrontier frontier)
    {
        if (!adjacency.TryGetValue(current, out var neighbors))
        {
            return false;
        }

        foreach (var next in neighbors)
        {
            if (next == target)
            {
                return true;
            }

            if (frontier.Visited.Add(next))
            {
                frontier.Queue.Enqueue(next);
            }
        }

        return false;
    }

    // The visited set and FIFO queue one reachability query expands into - bundled
    // so TryExpandFrontier stays within the parameter-count limit.
    private readonly record struct SearchFrontier(HashSet<char> Visited, Queue<char> Queue);

    // This repo's own DisjointSet over the 26 variable ids: every "==" equation is
    // one Union, then a single scan of the "!=" equations asks IsConnected. The
    // same union-then-scan shape AccountsMerge uses, applied to a fixed alphabet
    // instead of account indices.
    public static bool EquationsPossibleByDisjointSet(string[] equations)
    {
        var components = new DisjointSet(AlphabetSize);

        foreach (var equation in equations)
        {
            if (equation[1] == '=')
            {
                components.Union(VariableId(equation[0]), VariableId(equation[SecondVariableIndex]));
            }
        }

        return !HasContradictingInequality(equations, components);
    }

    private static bool HasContradictingInequality(string[] equations, DisjointSet components)
    {
        foreach (var equation in equations)
        {
            if (equation[1] == '!' &&
                components.IsConnected(VariableId(equation[0]), VariableId(equation[SecondVariableIndex])))
            {
                return true;
            }
        }

        return false;
    }

    private static int VariableId(char variable) => variable - 'a';
}
