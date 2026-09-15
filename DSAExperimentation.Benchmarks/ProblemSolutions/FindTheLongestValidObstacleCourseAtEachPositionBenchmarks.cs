using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.FindTheLongestValidObstacleCourseAtEachPosition;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are
// FindTheLongestValidObstacleCourseAtEachPositionSolution's - the textbook O(n^2) DP
// against patience sorting over this repo's own BinarySearch.UpperBound. The workload is
// a fixed-seed random height sequence, so runs never fall into an all-increasing shape
// that would let the DP's inner loop stay cheap.
[MemoryDiagnoser]
public class FindTheLongestValidObstacleCourseAtEachPositionBenchmarks
{
    private const int RandomSeed = 1964; private int[] _obstacles = [];

    // LC problem number

    [Params(2_000, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _obstacles = Enumerable.Range(0, Length).Select(_ => random.Next(0, Length)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int[] DynamicProgramming() =>
        FindTheLongestValidObstacleCourseAtEachPositionSolution.LongestObstacleCourseByDynamicProgramming(_obstacles);

    [Benchmark]
    public int[] PatienceSortingBinarySearch() =>
        FindTheLongestValidObstacleCourseAtEachPositionSolution.LongestObstacleCourseByPatienceSorting(_obstacles);
}
