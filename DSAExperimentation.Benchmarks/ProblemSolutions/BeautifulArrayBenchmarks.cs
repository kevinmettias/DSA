using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Beautiful Array (LC 932): PrunedBacktrackingSearch tries candidate values
// left-to-right and backtracks the instant a partial prefix violates the
// no-arithmetic-mean rule - exponential in the worst case, the naive way most
// solvers reach for first. MemoizedDivideAndConquer instead builds the array
// top-down (odds = 2x-1 over Build((n+1)/2), evens = 2x over Build(n/2), which
// stays beautiful because the two halves land on disjoint parities) using this
// repo's own HashMap<int,int[]> to memoize the O(n) distinct subproblem sizes
// the recursion actually revisits - O(n log n) total, the same top-down
// -memoization role HashMap already plays elsewhere in this repo's DP coverage.
[MemoryDiagnoser]
public class BeautifulArrayBenchmarks
{
    [Params(6, 8)]
    public int Length;

    [Benchmark(Baseline = true)]
    public int[] PrunedBacktrackingSearch()
    {
        var used = new bool[Length + 1];
        var result = new int[Length];
        Search(0, used, result);
        return result;
    }

    private bool Search(int position, bool[] used, int[] result)
    {
        if (position == Length)
        {
            return true;
        }

        for (var candidate = 1; candidate <= Length; candidate++)
        {
            if (used[candidate])
            {
                continue;
            }

            result[position] = candidate;

            if (IsValidPrefix(result, position))
            {
                used[candidate] = true;

                if (Search(position + 1, used, result))
                {
                    return true;
                }

                used[candidate] = false;
            }
        }

        return false;
    }

    private static bool IsValidPrefix(int[] result, int lastIndex)
    {
        for (var i = 0; i < lastIndex; i++)
        {
            for (var k = i + 1; k < lastIndex; k++)
            {
                if ((2 * result[k]) == result[i] + result[lastIndex])
                {
                    return false;
                }
            }
        }

        return true;
    }

    [Benchmark]
    public int[] MemoizedDivideAndConquer() => Build(Length, new HashMap<int, int[]>());

    private static int[] Build(int n, HashMap<int, int[]> memo)
    {
        if (n == 1)
        {
            return [1];
        }

        if (memo.TryGetValue(n, out var cached))
        {
            return cached;
        }

        var odds = Build((n + 1) / 2, memo).Select(x => (2 * x) - 1);
        var evens = Build(n / 2, memo).Select(x => 2 * x);
        var result = odds.Concat(evens).ToArray();

        memo.Set(n, result);
        return result;
    }
}
