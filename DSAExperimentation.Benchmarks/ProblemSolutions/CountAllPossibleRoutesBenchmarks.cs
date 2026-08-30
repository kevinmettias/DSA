using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Count All Possible Routes (LC 1575): the naive un-memoized recursion over
// (city, remaining fuel) - re-exploring the same state on every path that reaches it,
// the same "no cache" shape FibonacciNumberBenchmarks' own baseline uses - vs. this
// repo's own Memoizer keyed on the (City, Fuel) tuple state
// (CountAllPossibleRoutesTests/CoinChangeIIBenchmarks precedent). Fuel stays small
// enough that the naive side's branching (up to Locations.Length - 1 per step) still
// finishes in reasonable time while remaining clearly exponential next to the
// memoized O(Locations.Length^2 * Fuel) side.
[MemoryDiagnoser]
public class CountAllPossibleRoutesBenchmarks
{
    private const int Mod = 1_000_000_007;
    private static readonly int[] Locations = [0, 1, 2, 3];
    private const int Start = 0;
    private const int Finish = 3;

    [Params(8, 12)]
    public int Fuel;

    [Benchmark(Baseline = true)]
    public int NaiveRecursion() => CountRoutesNaive(Start, Fuel);

    private static int CountRoutesNaive(int city, int remaining)
    {
        var total = city == Finish ? 1 : 0;

        for (var next = 0; next < Locations.Length; next++)
        {
            if (next == city)
            {
                continue;
            }

            var cost = Math.Abs(Locations[city] - Locations[next]);
            if (cost <= remaining)
            {
                total = (total + CountRoutesNaive(next, remaining - cost)) % Mod;
            }
        }

        return total;
    }

    [Benchmark]
    public int MemoizedTopDown()
    {
        return Memoizer.Memoize<(int City, int Fuel), int>((Start, Fuel), WaysFrom);

        int WaysFrom((int City, int Fuel) state, Func<(int City, int Fuel), int> ways)
        {
            var (city, remaining) = state;
            var total = city == Finish ? 1 : 0;

            for (var next = 0; next < Locations.Length; next++)
            {
                if (next == city)
                {
                    continue;
                }

                var cost = Math.Abs(Locations[city] - Locations[next]);
                if (cost <= remaining)
                {
                    total = (total + ways((next, remaining - cost))) % Mod;
                }
            }

            return total;
        }
    }
}
