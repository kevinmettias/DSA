using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.RemoveSubFoldersFromTheFilesystem;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are RemoveSubFoldersFromTheFilesystemSolution's, the same
// methods RemoveSubFoldersFromTheFilesystemTests proves correct - the O(n^2) pairwise
// "does any other folder prefix me" check against the O(n log n) MergeSort-then-scan.
// [GlobalSetup] builds the random folder tree; each arm takes .Count so the two
// return the same comparable measurement while still producing LeetCode's real answer
// (the pre-migration arms only ever counted, and never built, the surviving list).
[MemoryDiagnoser]
public class RemoveSubFoldersFromTheFilesystemBenchmarks
{
    private const int RandomSeed = 1233; // LC problem number
    private const int MinBreadth = 2;
    private const int BreadthDivisor = 10;
    private const int MaxDepthExclusive = 5;
    private const string FolderNamePrefix = "f";
    private const string PathSeparator = "/";

    private string[] _folders = [];

    [Params(200, 3_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var breadth = Math.Max(MinBreadth, Length / BreadthDivisor);
        _folders = new string[Length];

        for (var i = 0; i < Length; i++)
        {
            var depth = random.Next(1, MaxDepthExclusive);
            var segments = new string[depth];

            for (var d = 0; d < depth; d++)
            {
                segments[d] = FolderNamePrefix + random.Next(0, breadth);
            }

            _folders[i] = PathSeparator + string.Join('/', segments);
        }
    }

    [Benchmark(Baseline = true)]
    public int BruteForcePairwisePrefixCheck() =>
        RemoveSubFoldersFromTheFilesystemSolution
            .RemoveSubfoldersByPairwisePrefixCheck(_folders).Count;

    [Benchmark]
    public int MergeSortThenScan() =>
        RemoveSubFoldersFromTheFilesystemSolution
            .RemoveSubfoldersByMergeSortThenScan(_folders).Count;
}
