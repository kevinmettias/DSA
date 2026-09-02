using BenchmarkDotNet.Attributes;
using RepoStack = DSAExperimentation.DataStructures.Stack.Stack<(int Value, int Step)>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Steps to Make Array Non-decreasing (LC 2289): the direct brute-force baseline -
// literally simulate each removal round over the array (O(n) per round, up to O(n)
// rounds) - vs. a single O(n) monotonic-decreasing sweep through this repo's own
// Stack<(int,int)> tracking each pending value's own removal step
// (DailyTemperaturesBenchmarks' MonotonicStackSweep precedent, generalized from
// "wait days" to "removal step"). Values are a random sequence with repeats so
// removal rounds actually chain instead of finishing after one pass.
[MemoryDiagnoser]
public class StepsToMakeArrayNonDecreasingBenchmarks
{
    private const int RandomSeed = 2289; // LC problem number

    [Params(200, 5_000)]
    public int Length;

    private int[] _nums = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(0, Length)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int SimulateRounds()
    {
        var current = _nums;
        var steps = 0;

        while (true)
        {
            var (next, anyRemoved) = RunOneRound(current);

            if (!anyRemoved)
            {
                return steps;
            }

            current = next;
            steps++;
        }
    }

    [Benchmark]
    public int MonotonicStackSweep()
    {
        var stack = new RepoStack();
        var maxSteps = 0;

        foreach (var value in _nums)
        {
            var step = 0;

            while (stack.TryPeek(out var top) && top.Value <= value)
            {
                step = Math.Max(step, top.Step);
                stack.TryPop(out _);
            }

            step = stack.Count == 0 ? 0 : step + 1;
            maxSteps = Math.Max(maxSteps, step);
            stack.Push((value, step));
        }

        return maxSteps;
    }

    // One simultaneous removal round: every index whose left neighbor (in the
    // pre-round array) is strictly greater gets dropped, in one linear pass.
    private static (int[] Next, bool AnyRemoved) RunOneRound(int[] current)
    {
        var next = new List<int>(current.Length) { current[0] };
        var anyRemoved = false;

        for (var i = 1; i < current.Length; i++)
        {
            if (current[i - 1] > current[i])
            {
                anyRemoved = true;
            }
            else
            {
                next.Add(current[i]);
            }
        }

        return (next.ToArray(), anyRemoved);
    }
}
