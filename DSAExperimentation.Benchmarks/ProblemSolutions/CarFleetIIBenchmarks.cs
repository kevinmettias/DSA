using BenchmarkDotNet.Attributes;
using RepoIndexStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Car Fleet II (LC 1776): both variants compute the exact same right-to-left
// recurrence (CarFleetIITests precedent) - a candidate car j ahead is only ever
// usable if car i is faster and would reach j before j's own already-known
// collision. BruteForcePerCar re-derives that from scratch for every car via an
// independent forward scan with no memory of previous cars' discarded
// candidates, O(n^2) worst case. MonotonicStackSweep instead composes this
// repo's own Stack<int> to permanently discard a candidate the moment it's
// proven irrelevant, so it is examined by later (further-behind) cars at all -
// O(n) amortized, one push and at most one pop per car. Speeds are strictly
// increasing front-to-back (car i is always slower than every car ahead of it),
// so no car ever catches up - the classic "never breaks early" adversarial input
// that forces BruteForcePerCar's inner scan all the way to the end every time.
[MemoryDiagnoser]
public class CarFleetIIBenchmarks
{
    // Gap between adjacent cars' starting positions in the generated fleet.
    private const int PositionSpacing = 10;

    [Params(200, 5_000)]
    public int Length;

    private int[][] _cars = null!;

    [GlobalSetup]
    public void Setup()
    {
        _cars = new int[Length][];

        for (var i = 0; i < Length; i++)
        {
            _cars[i] = [i * PositionSpacing, i + 1];
        }
    }

    [Benchmark(Baseline = true)]
    public double[] BruteForcePerCar()
    {
        var cars = _cars;
        var answer = new double[cars.Length];
        ComputeCollisionTimesBruteForce(cars, answer);
        return answer;
    }

    [Benchmark]
    public double[] MonotonicStackSweep()
    {
        var cars = _cars;
        var answer = new double[cars.Length];
        var candidatesAhead = new RepoIndexStack();

        for (var i = cars.Length - 1; i >= 0; i--)
        {
            answer[i] = -1.0;
            ResolveCollisionForCar(cars, answer, candidatesAhead, i);
            candidatesAhead.Push(i);
        }

        return answer;
    }

    private static void ComputeCollisionTimesBruteForce(int[][] cars, double[] answer)
    {
        for (var i = cars.Length - 1; i >= 0; i--)
        {
            answer[i] = -1.0;

            for (var j = i + 1; j < cars.Length; j++)
            {
                if (cars[i][1] <= cars[j][1])
                {
                    continue;
                }

                var collisionTime = (double)(cars[j][0] - cars[i][0]) / (cars[i][1] - cars[j][1]);

                if (answer[j] < 0 || collisionTime <= answer[j])
                {
                    answer[i] = collisionTime;
                    break;
                }
            }
        }
    }

    private static void ResolveCollisionForCar(int[][] cars, double[] answer, RepoIndexStack candidatesAhead, int i)
    {
        while (candidatesAhead.TryPeek(out var j))
        {
            if (cars[i][1] <= cars[j][1])
            {
                candidatesAhead.TryPop(out _);
                continue;
            }

            var collisionTime = (double)(cars[j][0] - cars[i][0]) / (cars[i][1] - cars[j][1]);

            if (answer[j] < 0 || collisionTime <= answer[j])
            {
                answer[i] = collisionTime;
                break;
            }

            candidatesAhead.TryPop(out _);
        }
    }
}
