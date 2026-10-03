using DSAExperimentation.LeetCode.Subsets;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SubsetsSolution's, the same methods SubsetsSolutionTests proves correct.
// Both arms materialize all 2^n subsets - the parameter counts exponents, not raw input size - and
// ElementCount stops at LC 78's 10 elements, 1,024 lists. Values are 1..ElementCount, distinct and
// inside its [-10, 10], so every subset is distinct, which keeps the two arms' outputs comparable
// element for element.
public class SubsetsBenchmarks
{
    private const int RandomSeed = 78; // LC problem number

    private int[] _values = [];

    [Params(8, 10)]
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
