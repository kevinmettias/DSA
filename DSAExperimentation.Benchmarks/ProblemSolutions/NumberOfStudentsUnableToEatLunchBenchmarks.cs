using BenchmarkDotNet.Attributes;
using RepoQueue = DSAExperimentation.DataStructures.Queue.Queue<int>;
using RepoStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Number of Students Unable to Eat Lunch (LC 1700): a plain List<int>
// simulation of the textbook circular-queue process (List.RemoveAt(0) is
// O(n), so every front-of-line removal shifts the rest of the list - O(n^2)
// overall once many students cycle to the back) vs. this repo's own
// Queue<int> (students, Deque-backed so front removal is O(1)) and
// Stack<int> (sandwiches, top = index 0) running the identical simulation -
// the same Queue+Stack composition NumberOfStudentsUnableToEatLunchTests
// itself makes - for O(n) overall.
[MemoryDiagnoser]
public class NumberOfStudentsUnableToEatLunchBenchmarks
{
    // LC problem number, reused as the deterministic benchmark seed.
    private const int RandomSeed = 1700;

    private const int PreferenceUpperBound = 2;

    [Params(200, 5_000)]
    public int Length;

    private int[] _students = null!;
    private int[] _sandwiches = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _students = Enumerable.Range(0, Length).Select(_ => random.Next(PreferenceUpperBound)).ToArray();
        _sandwiches = Enumerable.Range(0, Length).Select(_ => random.Next(PreferenceUpperBound)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int ListSimulation()
    {
        var queue = new List<int>(_students);
        var sandwichIndex = 0;
        var consecutiveSkips = 0;

        while (queue.Count > 0 && consecutiveSkips < queue.Count)
        {
            var preference = queue[0];
            queue.RemoveAt(0);

            if (preference == _sandwiches[sandwichIndex])
            {
                sandwichIndex++;
                consecutiveSkips = 0;
            }
            else
            {
                queue.Add(preference);
                consecutiveSkips++;
            }
        }

        return queue.Count;
    }

    [Benchmark]
    public int QueueStackSimulation()
    {
        var queue = BuildQueue(_students);
        var stack = BuildStack(_sandwiches);

        return SimulateLunchLine(queue, stack);
    }

    private static RepoQueue BuildQueue(int[] students)
    {
        var queue = new RepoQueue();

        foreach (var student in students)
        {
            queue.Enqueue(student);
        }

        return queue;
    }

    private static RepoStack BuildStack(int[] sandwiches)
    {
        var stack = new RepoStack();

        for (var i = sandwiches.Length - 1; i >= 0; i--)
        {
            stack.Push(sandwiches[i]);
        }

        return stack;
    }

    private static int SimulateLunchLine(RepoQueue queue, RepoStack stack)
    {
        var consecutiveSkips = 0;

        while (queue.Count > 0 && consecutiveSkips < queue.Count)
        {
            queue.TryDequeue(out var preference);
            stack.TryPeek(out var top);

            if (preference == top)
            {
                stack.TryPop(out _);
                consecutiveSkips = 0;
            }
            else
            {
                queue.Enqueue(preference);
                consecutiveSkips++;
            }
        }

        return queue.Count;
    }
}
