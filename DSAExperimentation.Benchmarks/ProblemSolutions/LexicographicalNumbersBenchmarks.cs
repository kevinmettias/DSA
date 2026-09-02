using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Traversal.DepthFirst;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Lexicographical Numbers (LC 386): sorting 1..n by their string representation
// (O(n log n), a string allocation plus an ordinal comparison per element) vs. this
// repo's own successor-function DepthFirstSearch.Traverse walking the implicit
// 10-ary "next digit" tree in O(n), one root 1-9 at a time - the exact "implicit
// graph, no Representation axis" shape ARCHITECTURE.md's Traversal section
// documents.
[MemoryDiagnoser]
public class LexicographicalNumbersBenchmarks
{
    private const int MaxDigit = 9;
    private const int DecimalBase = 10;

    [Params(1_000, 500_000)]
    public int N;

    [Benchmark(Baseline = true)]
    public List<int> SortByStringRepresentation()
        => Enumerable.Range(1, N)
            .OrderBy(value => value.ToString(), StringComparer.Ordinal)
            .ToList();

    [Benchmark]
    public List<int> DepthFirstOverImplicitDigitTree()
    {
        var order = new List<int>(N);

        for (var root = 1; root <= MaxDigit && root <= N; root++)
        {
            var traversal = DepthFirstSearch.Traverse(root, current => Successors(current, N));
            order.AddRange(traversal);
        }

        return order;
    }

    private static IEnumerable<int> Successors(int current, int n)
    {
        for (var digit = 0; digit <= MaxDigit; digit++)
        {
            var next = (current * DecimalBase) + digit;
            if (next > n)
            {
                yield break;
            }

            yield return next;
        }
    }
}
