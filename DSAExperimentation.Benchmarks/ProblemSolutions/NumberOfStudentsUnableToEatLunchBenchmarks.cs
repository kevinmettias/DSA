using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.NumberOfStudentsUnableToEatLunch;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are NumberOfStudentsUnableToEatLunchSolution's, the same
// methods NumberOfStudentsUnableToEatLunchTests proves correct. The workload is a
// seeded random line of binary preferences against an equally random pile, so many
// students cycle to the back before the line stalls - exactly the traffic that
// charges the List<int> baseline its O(n) RemoveAt(0) per round against the
// Deque-backed Queue<int>'s O(1). Generating both arrays is [GlobalSetup]'s job,
// so only the simulation is measured.
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
    public int ListSimulation() =>
        NumberOfStudentsUnableToEatLunchSolution.CountStudentsByListSimulation(_students, _sandwiches);

    [Benchmark]
    public int QueueStackSimulation() =>
        NumberOfStudentsUnableToEatLunchSolution.CountStudentsByQueueStackSimulation(_students, _sandwiches);
}
