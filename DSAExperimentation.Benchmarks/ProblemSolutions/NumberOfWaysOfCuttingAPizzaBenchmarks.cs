using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.NumberOfWaysOfCuttingAPizza;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are NumberOfWaysOfCuttingAPizzaSolution's, the same
// methods NumberOfWaysOfCuttingAPizzaTests proves correct. Both are handed the
// prepared AppleGrid their hoisted overload takes, so building the suffix-sum table
// is charged to [GlobalSetup] rather than to the cut counting being measured. The
// pizza is all apples, so nothing short-circuits the naive baseline's full
// branching early; Size and the piece count are kept modest for exactly that
// reason.
[MemoryDiagnoser]
public class NumberOfWaysOfCuttingAPizzaBenchmarks
{
    // Four cuts, the workload the pre-migration benchmark measured, stated here as
    // the piece count LeetCode's own signature takes.
    private const int Pieces = 5;

    [Params(6, 8)]
    public int Size;

    private AppleGrid _apples;

    [GlobalSetup]
    public void Setup()
    {
        var pizza = new string[Size];
        Array.Fill(pizza, new string('A', Size));
        _apples = new AppleGrid(pizza);
    }

    [Benchmark(Baseline = true)]
    public int UnmemoizedRecursion() =>
        NumberOfWaysOfCuttingAPizzaSolution.CountWaysByUnmemoizedRecursion(_apples, Pieces);

    [Benchmark]
    public int MemoizedRecursion() =>
        NumberOfWaysOfCuttingAPizzaSolution.CountWaysByMemoizedRecursion(_apples, Pieces);
}
