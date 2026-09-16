using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.HappyNumber;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are HappyNumberSolution's, the same methods
// HappyNumberTests proves correct. The pre-migration arms here ("Baseline",
// "PrimitiveComposed") were compile-smoke placeholders that returned a constant
// and never invoked any algorithm; these replace them with the problem's actual
// two textbook approaches, both walking the canonical non-happy cycle so
// neither strategy gets to exit early.
[MemoryDiagnoser]
public class HappyNumberBenchmarks
{
    // A member of the canonical non-happy cycle (4 -> 16 -> ... -> 4), forcing
    // the walk all the way around the cycle before either strategy detects it.
    private const int UnhappyCycleMember = 4;

    [Benchmark(Baseline = true)]
    public bool IsHappyByVisitedSet() => HappyNumberSolution.IsHappyByVisitedSet(UnhappyCycleMember);

    [Benchmark]
    public bool IsHappyByFloydCycleDetection() => HappyNumberSolution.IsHappyByFloydCycleDetection(UnhappyCycleMember);
}
