using DSAExperimentation.DataStructures.HashMap;
using FrontierStack = DSAExperimentation.DataStructures.Stack.Stack<(string Node, double Product)>;

namespace DSAExperimentation.LeetCode.EvaluateDivision;

// LeetCode 399. Evaluate Division: each equation a/b = value wires two directed edges
// (a->b weighted value, b->a weighted 1/value) into a weighted adjacency map. A query
// is then an explicit-stack DFS from the dividend, multiplying edge weights along the
// way until the divisor is reached or the reachable component is exhausted - -1.0 if
// either variable has never appeared in an equation.
internal static class EvaluateDivisionSolution
{
    private const double NotFound = -1.0;
    private const double SelfDivision = 1.0;

    // The textbook baseline: a plain Dictionary<string, Dictionary<string, double>>
    // adjacency map and a BCL Stack<(string,double)> DFS. Deliberately written
    // without this repo's primitives - it is the arm the composed strategy below has
    // to justify itself against.
    public static double EvaluateByDictionaryDfs(
        IEnumerable<(string Dividend, string Divisor, double Value)> equations, Dividend dividend, Divisor divisor)
    {
        var neighborsByNode = BuildDictionaryGraph(equations);

        if (!neighborsByNode.ContainsKey(dividend.Name) || !neighborsByNode.ContainsKey(divisor.Name))
        {
            return NotFound;
        }

        return DictionaryDfs(neighborsByNode, dividend, divisor);
    }

    private static Dictionary<string, Dictionary<string, double>> BuildDictionaryGraph(
        IEnumerable<(string Dividend, string Divisor, double Value)> equations)
    {
        var neighborsByNode = new Dictionary<string, Dictionary<string, double>>();

        void AddEdge(string from, string to, double weight)
        {
            if (!neighborsByNode.TryGetValue(from, out var neighbors))
            {
                neighbors = new Dictionary<string, double>();
                neighborsByNode[from] = neighbors;
            }

            neighbors[to] = weight;
        }

        foreach (var (dividend, divisor, value) in equations)
        {
            AddEdge(dividend, divisor, value);
            AddEdge(divisor, dividend, 1.0 / value);
        }

        return neighborsByNode;
    }

    private static double DictionaryDfs(
        Dictionary<string, Dictionary<string, double>> neighborsByNode, Dividend dividend, Divisor divisor)
    {
        var visited = new HashSet<string> { dividend.Name };
        var pending = new Stack<(string Node, double Product)>();
        pending.Push((dividend.Name, SelfDivision));

        while (pending.TryPop(out var current))
        {
            if (current.Node == divisor.Name)
            {
                return current.Product;
            }

            if (neighborsByNode.TryGetValue(current.Node, out var neighbors))
            {
                PushUnvisitedNeighbors(neighbors, current, visited, pending);
            }
        }

        return NotFound;
    }

    private static void PushUnvisitedNeighbors(
        Dictionary<string, double> neighbors,
        (string Node, double Product) current,
        HashSet<string> visited,
        Stack<(string Node, double Product)> pending)
    {
        foreach (var (neighbor, weight) in neighbors)
        {
            if (visited.Add(neighbor))
            {
                pending.Push((neighbor, current.Product * weight));
            }
        }
    }

    // This repo's own composition: a HashMap<string, HashMap<string, double>>
    // adjacency map - HashMap nested inside itself, the same "compose, don't invent"
    // precedent InsertDeleteGetRandomO1DuplicatesAllowed already uses for a
    // value-keyed collection of collections - walked by an explicit-stack DFS over
    // this repo's own Stack<T> (the DepthFirstSearch.Traverse precedent, extended to
    // carry a running product alongside each frontier node instead of just visiting
    // order).
    public static double EvaluateByHashMapStackDfs(
        IEnumerable<(string Dividend, string Divisor, double Value)> equations, Dividend dividend, Divisor divisor)
    {
        var neighborsByNode = BuildHashMapGraph(equations);

        if (!neighborsByNode.HasKey(dividend.Name) || !neighborsByNode.HasKey(divisor.Name))
        {
            return NotFound;
        }

        if (dividend.Name == divisor.Name)
        {
            return SelfDivision;
        }

        var frontier = CreateFrontier(dividend.Name);
        return Search(neighborsByNode, frontier, divisor.Name);
    }

    private static HashMap<string, HashMap<string, double>> BuildHashMapGraph(
        IEnumerable<(string Dividend, string Divisor, double Value)> equations)
    {
        var neighborsByNode = new HashMap<string, HashMap<string, double>>();

        void AddEdge(string from, string to, double weight)
        {
            if (!neighborsByNode.TryGetValue(from, out var neighbors))
            {
                neighbors = new HashMap<string, double>();
                neighborsByNode.Set(from, neighbors);
            }

            neighbors.Set(to, weight);
        }

        foreach (var (dividend, divisor, value) in equations)
        {
            AddEdge(dividend, divisor, value);
            AddEdge(divisor, dividend, 1.0 / value);
        }

        return neighborsByNode;
    }

    private static FrontierState CreateFrontier(string dividend)
    {
        var visited = new HashSet<string> { dividend };
        var pending = new FrontierStack();
        pending.Push((dividend, SelfDivision));
        return new FrontierState(visited, pending);
    }

    private static double Search(
        HashMap<string, HashMap<string, double>> neighborsByNode, FrontierState frontier, string divisor)
    {
        while (frontier.Pending.TryPop(out var current))
        {
            if (current.Node == divisor)
            {
                return current.Product;
            }

            ExpandNeighbors(neighborsByNode, current, frontier);
        }

        return NotFound;
    }

    private static void ExpandNeighbors(
        HashMap<string, HashMap<string, double>> neighborsByNode,
        (string Node, double Product) current,
        FrontierState frontier)
    {
        neighborsByNode.TryGetValue(current.Node, out var neighbors);

        foreach (var neighbor in neighbors.Keys)
        {
            if (!frontier.Visited.Add(neighbor))
            {
                continue;
            }

            neighbors.TryGetValue(neighbor, out var weight);
            frontier.Pending.Push((neighbor, current.Product * weight));
        }
    }

    internal readonly record struct FrontierState(HashSet<string> Visited, FrontierStack Pending);

    // LC 399's two query variables, named for the roles they play here rather than left
    // as two adjacent `string` positions a caller could hand over the wrong way round
    // with the compiler none the wiser. `dividend` is the variable the walk starts from
    // and `divisor` the one it is trying to reach - and the cost of (a, b) is the
    // reciprocal of the cost of (b, a), so a swap is a silently wrong value rather than
    // a different question.
    internal readonly record struct Dividend(string Name);

    internal readonly record struct Divisor(string Name);
}
