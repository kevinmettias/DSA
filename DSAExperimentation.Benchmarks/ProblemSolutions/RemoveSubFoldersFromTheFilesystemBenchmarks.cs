using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Remove Sub-Folders from the Filesystem (LC 1233): the O(n^2) pairwise "does any
// other folder prefix me" brute force vs. this repo's own MergeSort over an
// ArrayIndexedSequence<string> followed by a single O(n) sort-then-scan pass - the
// classic O(n log n) solution (ArrayPartitionBenchmarks precedent for cloning the
// backing array per call so MergeSort's in-place sort never contaminates the next
// invocation with already-sorted input).
[MemoryDiagnoser]
public class RemoveSubFoldersFromTheFilesystemBenchmarks
{
    private const int RandomSeed = 1233; // LC problem number
    private const int MinBreadth = 2;
    private const int BreadthDivisor = 10;
    private const int MaxDepthExclusive = 5;
    private const string FolderNamePrefix = "f";
    private const string PathSeparator = "/";

    [Params(200, 3_000)]
    public int Length;

    private string[] _folders = null!;

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
    public int BruteForcePairwisePrefixCheck()
    {
        var count = 0;

        for (var i = 0; i < _folders.Length; i++)
        {
            var isSubfolder = false;

            for (var j = 0; j < _folders.Length; j++)
            {
                if (i != j && _folders[i].StartsWith(_folders[j] + PathSeparator, StringComparison.Ordinal))
                {
                    isSubfolder = true;
                    break;
                }
            }

            if (!isSubfolder)
            {
                count++;
            }
        }

        return count;
    }

    [Benchmark]
    public int MergeSortThenScan()
    {
        var folders = _folders.ToArray();
        var sequence = new ArrayIndexedSequence<string>(folders);
        MergeSort.Sort<string, ArrayIndexedSequence<string>>(sequence, StringComparer.Ordinal);

        var count = 0;
        string? lastKept = null;

        foreach (var folder in folders)
        {
            if (lastKept is null || !folder.StartsWith(lastKept + PathSeparator, StringComparison.Ordinal))
            {
                count++;
                lastKept = folder;
            }
        }

        return count;
    }
}
