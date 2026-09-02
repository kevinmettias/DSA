using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.EditDistance;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are EditDistanceSolution's, the same methods
// EditDistanceTests proves correct. The two strings are already LeetCode's own
// input shape, so [GlobalSetup] only sizes the workload - there is no separate
// prepared-input overload to hoist into.
[MemoryDiagnoser]
public class EditDistanceBenchmarks
{
    private const string DifferingSuffix = "b";

    private string _first = null!;
    private string _second = null!;

    [Params(20, 80)]
    public int Length;

    [GlobalSetup]
    public void Setup()
    {
        _first = new string('a', Length);
        _second = new string('a', Length - 1) + DifferingSuffix;
    }

    [Benchmark(Baseline = true)]
    public int Tabulation() => EditDistanceSolution.MinDistanceByTabulation(_first, _second);

    [Benchmark]
    public int MemoizedRecurrence() => EditDistanceSolution.MinDistanceByMemoizedRecurrence(_first, _second);
}
