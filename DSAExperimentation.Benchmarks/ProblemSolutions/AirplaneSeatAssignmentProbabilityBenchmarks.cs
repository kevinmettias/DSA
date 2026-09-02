using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Airplane Seat Assignment Probability (LC 1227): the O(n^2) memoized probability
// recursion summing over every shorter seat count - via this repo's own Memoizer, no
// hand-rolled cache, the same pairing DivisorGameBenchmarks/NimGameBenchmarks already
// use - vs. the closed-form O(1) "1 if n == 1 else 0.5" formula the recursion itself
// reduces to.
[MemoryDiagnoser]
public class AirplaneSeatAssignmentProbabilityBenchmarks
{
    // First seat index after the base case (seat 1) that the recursion sums over.
    private const int FirstAlternativeSeat = 2;

    // Closed-form probability for every seat past the first: the recursion itself
    // reduces to this constant.
    private const double NonFirstSeatProbability = 0.5;

    [Params(100, 1_000)]
    public int N;

    [Benchmark(Baseline = true)]
    public double MemoizedRecursion()
        => Memoizer.Memoize<int, double>(N, (current, probability) =>
        {
            if (current == 1)
            {
                return 1.0;
            }

            var sum = 1.0;

            for (var j = FirstAlternativeSeat; j < current; j++)
            {
                sum += probability(j);
            }

            return sum / current;
        });

    [Benchmark]
    public double ClosedForm() => N == 1 ? 1.0 : NonFirstSeatProbability;
}
