using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.CountWaysToChooseCoprimeIntegersFromRows;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CountWaysToChooseCoprimeIntegersFromRowsSolution's,
// the same methods CountWaysToChooseCoprimeIntegersFromRowsSolutionTests proves correct.
//
// Sizes are per arm, each a square matrix. The brute-force DFS enumerates size^size
// leaf combinations and would not finish past 7 x 7, so it stops there; the
// GCD-counting DP stays polynomial in size and runs on to LC 3725's own 150 x 150
// bound, and the two are compared at the sizes both run.
public class CountWaysToChooseCoprimeIntegersFromRowsBenchmarks
{
    private const int RandomSeed = 3725; // LC problem number
    private const int MaxValueInclusive = 150;

    private Dictionary<int, int[][]> _matBySize = [];

    public static IEnumerable<int> BruteForceSizes => [5, 7];

    public static IEnumerable<int> GcdCountingDpSizes => [.. BruteForceSizes, 40, 150];

    // Every size any arm runs is drawn here, outside the timed region, each from its own
    // generator on the same seed; an arm looks its own up.
    [GlobalSetup]
    public void Setup() => _matBySize = GcdCountingDpSizes.ToDictionary(size => size, BuildMatrix);

    private static int[][] BuildMatrix(int size)
    {
        var random = new Random(RandomSeed);
        var mat = new int[size][];

        for (var row = 0; row < size; row++)
        {
            mat[row] = SeededDraws.Values(size, 1, MaxValueInclusive + 1, random);
        }

        return mat;
    }

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(BruteForceSizes))]
    public long BruteForce(int size) =>
        CountWaysToChooseCoprimeIntegersFromRowsSolution.CountWaysByBruteForce(_matBySize[size]);

    [Benchmark]
    [ArgumentsSource(nameof(GcdCountingDpSizes))]
    public long GcdCountingDp(int size) =>
        CountWaysToChooseCoprimeIntegersFromRowsSolution.CountWaysByGcdCountingDp(_matBySize[size]);
}
