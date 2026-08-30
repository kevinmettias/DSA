using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.DynamicArray;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Elimination Game (LC 390): the O(n) DynamicArray<int> list-simulation baseline
// (rebuilds the surviving-numbers list every round, alternating direction) vs. the
// O(log n) closed-form head/step-tracking arithmetic it reduces to. No repo primitive
// applies to the closed-form side - the same "lighter repo-primitive fit" case
// already accepted for Pow(x, n)/Rectangle Area - but DynamicArray<int> stands in for
// the naive simulation's surviving-numbers list the way it already stands in for
// Count Primes' sieve array.
[MemoryDiagnoser]
public class EliminationGameBenchmarks
{
    [Params(10_000, 1_000_000)]
    public int N;

    [Benchmark(Baseline = true)]
    public int DynamicArraySimulation() => SimulateWithDynamicArray(N);

    [Benchmark]
    public int HeadStepArithmetic() => LastRemaining(N);

    private static int LastRemaining(int n)
    {
        var head = 1;
        var step = 1;
        var leftToRight = true;
        var remaining = n;

        while (remaining > 1)
        {
            if (leftToRight || remaining % 2 == 1)
            {
                head += step;
            }

            remaining /= 2;
            step *= 2;
            leftToRight = !leftToRight;
        }

        return head;
    }

    private static int SimulateWithDynamicArray(int n)
    {
        var current = new DynamicArray<int>();

        for (var i = 1; i <= n; i++)
        {
            current.Add(i);
        }

        var leftToRight = true;

        while (current.Count > 1)
        {
            if (!leftToRight)
            {
                current = Reverse(current);
            }

            current = KeepOddPositions(current);

            if (!leftToRight)
            {
                current = Reverse(current);
            }

            leftToRight = !leftToRight;
        }

        return current.Get(0);
    }

    private static DynamicArray<int> KeepOddPositions(DynamicArray<int> values)
    {
        var kept = new DynamicArray<int>();

        for (var i = 1; i < values.Count; i += 2)
        {
            kept.Add(values.Get(i));
        }

        return kept;
    }

    private static DynamicArray<int> Reverse(DynamicArray<int> values)
    {
        var reversed = new DynamicArray<int>();

        for (var i = values.Count - 1; i >= 0; i--)
        {
            reversed.Add(values.Get(i));
        }

        return reversed;
    }
}
