using DSAExperimentation.LeetCode.HappyNumber;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are HappyNumberSolution's, the same methods
// HappyNumberSolutionTests proves correct. The pre-migration arms here ("Baseline",
// "PrimitiveComposed") were compile-smoke placeholders that returned a constant
// and never invoked any algorithm; these replace them with the problem's actual
// two textbook approaches, both walking the canonical non-happy cycle so
// neither strategy gets to exit early. No [Params] axis: the input is one int,
// and its digit-square chain drops below 1,000 after a single step (ten digits
// of at most 81 each), so the walk is bounded by a small constant and there is no
// size to scale.
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
