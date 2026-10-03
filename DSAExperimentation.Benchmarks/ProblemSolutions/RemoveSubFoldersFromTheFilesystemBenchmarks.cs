using DSAExperimentation.LeetCode.RemoveSubFoldersFromTheFilesystem;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are RemoveSubFoldersFromTheFilesystemSolution's, the same
// methods RemoveSubFoldersFromTheFilesystemSolutionTests proves correct - the O(n^2) pairwise
// "does any other folder prefix me" check against the O(n log n) MergeSort-then-scan.
// [GlobalSetup] builds the random folder tree; each arm returns LeetCode's real
// answer, the surviving folders (the pre-migration arms only ever counted, and never
// built, the surviving list).
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
        var drawn = new HashSet<string>();
        _folders = new string[Length];

        // LC 1233 guarantees every folder in the list is unique, so a path drawn a second
        // time is drawn again rather than kept.
        while (drawn.Count < Length)
        {
            var depth = random.Next(1, MaxDepthExclusive);
            var segments = new string[depth];

            for (var d = 0; d < depth; d++)
            {
                segments[d] = FolderNamePrefix + random.Next(0, breadth);
            }

            var folder = PathSeparator + string.Join('/', segments);

            if (drawn.Add(folder))
            {
                _folders[drawn.Count - 1] = folder;
            }
        }
    }

    [Benchmark(Baseline = true)]
    public List<string> BruteForcePairwisePrefixCheck() =>
        RemoveSubFoldersFromTheFilesystemSolution
            .RemoveSubfoldersByPairwisePrefixCheck(_folders);

    [Benchmark]
    public List<string> MergeSortThenScan() =>
        RemoveSubFoldersFromTheFilesystemSolution
            .RemoveSubfoldersByMergeSortThenScan(_folders);
}
