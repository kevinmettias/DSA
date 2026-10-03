using DSAExperimentation.LeetCode.Subsets;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SubsetsSolution's, the same methods SubsetsTests proves correct.
// Element counts stay small because both arms materialize all 2^n subsets - the parameter counts
// exponents, not raw input size, and 2^14 already returns 16384 lists. Values are distinct so
// every subset is distinct, which keeps the two arms' outputs comparable element for element.
public class SubsetsBenchmarks
{
    private const int RandomSeed = 78; // LC problem number

    private int[] _values = [];

    [Params(8, 14)]
    public int ElementCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);

        _values = Enumerable.Range(1, ElementCount)
            .OrderBy(_ => random.Next())
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public List<List<int>> BitmaskEnumeration() =>
        SubsetsSolution.FindAllSubsetsByBitmask(_values);

    [Benchmark]
    public List<List<int>> BacktrackSearch() =>
        SubsetsSolution.FindAllSubsetsByBacktrack(_values);
}
