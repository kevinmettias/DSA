using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Traversal.DepthFirst;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// K-th Smallest in Lexicographical Order (LC 440): generating every number 1..n as a
// string and sorting it (O(n log n) - ordinal string comparison matches the same
// digit-by-digit order the implicit 10-ary tree walks) vs. this repo's own
// successor-function DepthFirstSearch.Traverse walking that tree directly in
// lexicographical order (O(n), no sort needed) - the same primitive
// LexicographicalNumbersTests (LC 386) already uses, just indexed to the k-th
// element instead of returning the whole order. Traverse's per-node Stack/HashSet
// bookkeeping carries a real constant-factor cost - a dry run showed it losing to
// the sort-based baseline by 2-5x at N=2_000/20_000 - but O(n) overtakes O(n log n)
// as n grows: roughly even by N=200_000, clearly ahead (~2x) by N=2_000_000. Large
// [Params] values, closer to this problem's LeetCode input range (n up to 10^9),
// are what it takes to actually see that crossover.
[MemoryDiagnoser]
public class KthSmallestInLexicographicalOrderBenchmarks
{
    private const int HalfDivisor = 2;
    private const int MaxDigit = 9;
    private const int DecimalBase = 10;

    [Params(200_000, 2_000_000)]
    public int N;

    private int _k;

    [GlobalSetup]
    public void Setup() => _k = N / HalfDivisor;

    [Benchmark(Baseline = true)]
    public int GenerateAndSortStrings()
    {
        var values = new string[N];
        for (var i = 1; i <= N; i++)
        {
            values[i - 1] = i.ToString();
        }

        Array.Sort(values, StringComparer.Ordinal);

        return int.Parse(values[_k - 1]);
    }

    [Benchmark]
    public int DepthFirstTraversalOrder()
    {
        var order = new List<int>();
        for (var root = 1; root <= MaxDigit && root <= N; root++)
        {
            var traversal = DepthFirstSearch.Traverse(root, current => Successors(current, N));
            order.AddRange(traversal);
        }

        return order[_k - 1];
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
