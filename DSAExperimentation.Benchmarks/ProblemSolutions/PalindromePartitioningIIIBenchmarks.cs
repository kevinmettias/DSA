using DSAExperimentation.LeetCode.PalindromePartitioningIII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are PalindromePartitioningIIISolution's, the same methods
// PalindromePartitioningIIISolutionTests proves correct. The workload is a random string
// over a small alphabet (so palindrome repairs are neither free nor uniformly
// expensive) split into half as many pieces as it has characters, which is where the
// two arms' shared decision tree is widest.
//
// Sizes are per arm. The naive recursion re-solves every (position, pieces left)
// state once per path, so it stops at 18 characters; the memoized arm solves each
// state once and runs on to LC 1278's own bound of 100. The two are compared at the
// sizes both run.
public class PalindromePartitioningIIIBenchmarks
{
    private const int RandomSeed = 1278; // LC problem number
    private const int AlphabetSize = 4;
    private const int PartitionDivisor = 2;

    private Dictionary<int, (string Text, int PartitionCount)> _workloadBySize = [];

    public static IEnumerable<int> BaselineSizes => [12, 18];

    public static IEnumerable<int> MemoizedSizes => [.. BaselineSizes, 50, 100];

    // Every size any arm runs is built here, outside the timed region; an arm looks its own up.
    [GlobalSetup]
    public void Setup() => _workloadBySize = MemoizedSizes.ToDictionary(length => length, BuildWorkload);

    private static (string Text, int PartitionCount) BuildWorkload(int length)
    {
        var random = new Random(RandomSeed);
        var text = new string(Enumerable.Range(0, length).Select(_ => (char)('a' + random.Next(AlphabetSize))).ToArray());

        return (text, length / PartitionDivisor);
    }

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(BaselineSizes))]
    public int NaiveRecursion(int length)
    {
        var (text, partitionCount) = _workloadBySize[length];

        return PalindromePartitioningIIISolution.MinChangesByNaiveRecursion(text, partitionCount);
    }

    [Benchmark]
    [ArgumentsSource(nameof(MemoizedSizes))]
    public int MemoizedTopDown(int length)
    {
        var (text, partitionCount) = _workloadBySize[length];

        return PalindromePartitioningIIISolution.MinChangesByMemoizedRecurrence(text, partitionCount);
    }
}
