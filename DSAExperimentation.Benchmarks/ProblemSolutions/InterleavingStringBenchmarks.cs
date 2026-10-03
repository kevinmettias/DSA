using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.InterleavingString;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are InterleavingStringSolution's, the same methods
// InterleavingStringSolutionTests proves correct. The previous class was a
// compile-smoke placeholder (`=> 1` on both arms) that measured nothing; this
// measures the memoized recursion against the roll-forward row over strings from
// InterleavingStringWorkloads: two seeded two-letter sources and a seeded
// interleaving of them, so the answer is true and the narrow alphabet keeps the
// recursion choosing between sources. SourceLength stops at LC 97's 100-letter cap.
public class InterleavingStringBenchmarks
{
    private const int RandomSeed = 97; // LC problem number

    private string _first = "";
    private string _second = "";
    private InterleavingStringSolution.TargetText _target;

    [Params(10, 100)]
    public int SourceLength { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var (first, second, target, _) = InterleavingStringWorkloads.Build(SourceLength, new Random(RandomSeed));
        _first = first;
        _second = second;
        _target = new InterleavingStringSolution.TargetText(target);
    }

    [Benchmark(Baseline = true)]
    public bool IsInterleaveByMemoizedRecursion() =>
        InterleavingStringSolution.IsInterleaveByMemoizedRecursion(_first, _second, _target);

    [Benchmark]
    public bool IterativeTable() =>
        InterleavingStringSolution.IsInterleaveByIterativeTable(_first, _second, _target);
}
