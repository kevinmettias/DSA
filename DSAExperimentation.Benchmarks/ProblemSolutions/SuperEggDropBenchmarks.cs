using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.SuperEggDrop;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SuperEggDropSolution's, the same methods
// SuperEggDropTests proves correct. The comparison is how the next trial floor is
// chosen at each (eggs, floors) state - an exhaustive scan of every candidate,
// O(eggs * floors^2), against a bisection on the monotonic worst-case curve,
// O(eggs * floors * log(floors)).
[MemoryDiagnoser]
public class SuperEggDropBenchmarks
{
    private const int Eggs = 2;

    [Params(50, 400)]
    public int Floors;

    [Benchmark(Baseline = true)]
    public int LinearScanDp() => SuperEggDropSolution.MinMovesByLinearScan(Eggs, Floors);

    [Benchmark]
    public int BinarySearchDp() => SuperEggDropSolution.MinMovesByBinarySearch(Eggs, Floors);
}
