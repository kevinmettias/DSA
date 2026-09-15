using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.DirectionAssignmentsWithExactlyKVisiblePeople;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are
// DirectionAssignmentsWithExactlyKVisiblePeopleSolution's, the same methods
// the Tests project proves correct.
[MemoryDiagnoser]
public class DirectionAssignmentsWithExactlyKVisiblePeopleBenchmarks
{
    private const int Seed = 3881; private int _pos;

    private int _k;
    // LC problem number

    [Params(200, 5_000)]
    public int PersonCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _pos = random.Next(0, PersonCount);
        _k = random.Next(0, PersonCount);
    }

    [Benchmark(Baseline = true)]
    public int PascalConvolution() =>
        DirectionAssignmentsWithExactlyKVisiblePeopleSolution.CountAssignmentsByPascalConvolution(PersonCount, _pos, _k);

    [Benchmark]
    public int VandermondeIdentity() =>
        DirectionAssignmentsWithExactlyKVisiblePeopleSolution.CountAssignmentsByVandermondeIdentity(PersonCount, _pos, _k);
}
