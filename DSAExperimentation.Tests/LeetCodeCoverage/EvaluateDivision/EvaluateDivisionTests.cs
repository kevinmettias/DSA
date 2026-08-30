using DSAExperimentation.DataStructures.HashMap;
using FrontierStack = DSAExperimentation.DataStructures.Stack.Stack<(string Node, double Product)>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.EvaluateDivision;

// LeetCode 399. Evaluate Division: each equation a/b = value wires two directed edges
// (a->b weighted value, b->a weighted 1/value) into a HashMap<string, HashMap<string,
// double>> adjacency map - this repo's own HashMap nested inside itself, the same
// "compose, don't invent" precedent InsertDeleteGetRandomO1DuplicatesAllowed already
// uses for a value-keyed collection of collections. A query is then an explicit-stack
// DFS over this repo's own Stack<T> (the DepthFirstSearch.Traverse precedent, extended
// to carry a running product alongside each frontier node instead of just visiting
// order) from the dividend, multiplying edge weights along the way until the divisor is
// reached or the reachable component is exhausted.
public sealed partial class EvaluateDivisionTests
{
    [Fact]
    public void Evaluate_ClassicTwoEquationChain_ComputesDirectAndInverseAndUnknownQueries()
    {
        var solution = new Solution(
            [("a", "b", 2.0), ("b", "c", 3.0)]);

        Assert.Equal(6.0, solution.Evaluate("a", "c"), 5);
        Assert.Equal(0.5, solution.Evaluate("b", "a"), 5);
        Assert.Equal(-1.0, solution.Evaluate("a", "e"), 5);
        Assert.Equal(1.0, solution.Evaluate("a", "a"), 5);
        Assert.Equal(-1.0, solution.Evaluate("x", "x"), 5);
    }

    [Fact]
    public void Evaluate_LongerChainThroughAnUnrelatedComponent_MultipliesAlongThePath()
    {
        var solution = new Solution(
            [("a", "b", 1.5), ("b", "c", 2.5), ("bc", "cd", 5.0)]);

        Assert.Equal(3.75, solution.Evaluate("a", "c"), 5);
        Assert.Equal(0.4, solution.Evaluate("c", "b"), 5);
        Assert.Equal(5.0, solution.Evaluate("bc", "cd"), 5);
        Assert.Equal(0.2, solution.Evaluate("cd", "bc"), 5);
        Assert.Equal(-1.0, solution.Evaluate("a", "cd"), 5);
    }

    private sealed class Solution
    {
        private readonly HashMap<string, HashMap<string, double>> _neighborsByNode = new();

        public Solution((string Dividend, string Divisor, double Value)[] equations)
        {
            foreach (var (dividend, divisor, value) in equations)
            {
                AddEdge(dividend, divisor, value);
                AddEdge(divisor, dividend, 1.0 / value);
            }
        }

        public double Evaluate(string dividend, string divisor)
        {
            if (!_neighborsByNode.HasKey(dividend) || !_neighborsByNode.HasKey(divisor))
            {
                return -1.0;
            }

            if (dividend == divisor)
            {
                return 1.0;
            }

            var visited = new HashSet<string> { dividend };
            var pending = new FrontierStack();
            pending.Push((dividend, 1.0));

            while (pending.TryPop(out var current))
            {
                if (current.Node == divisor)
                {
                    return current.Product;
                }

                _neighborsByNode.TryGetValue(current.Node, out var neighbors);

                foreach (var neighbor in neighbors.Keys)
                {
                    if (!visited.Add(neighbor))
                    {
                        continue;
                    }

                    neighbors.TryGetValue(neighbor, out var weight);
                    pending.Push((neighbor, current.Product * weight));
                }
            }

            return -1.0;
        }

        private void AddEdge(string from, string to, double weight)
        {
            if (!_neighborsByNode.TryGetValue(from, out var neighbors))
            {
                neighbors = new HashMap<string, double>();
                _neighborsByNode.Set(from, neighbors);
            }

            neighbors.Set(to, weight);
        }
    }
}
