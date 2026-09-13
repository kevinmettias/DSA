using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.RemoveSubFoldersFromTheFilesystem;

// LeetCode 1233. Remove Sub-Folders from the Filesystem: keep only the folders that
// are not contained in some other folder of the list. LeetCode accepts the answer in
// any order, which is what lets the two strategies below differ in the order they
// emit - the pairwise check keeps the caller's order, the sort-then-scan returns
// ordinal-sorted order.
//
// Both strategies hinge on the same containment rule: "/a/b/ca" is not a subfolder of
// "/a/b/c" even though it shares that raw string prefix, so every prefix test is
// against "<folder>/" rather than "<folder>".
internal static class RemoveSubFoldersFromTheFilesystemSolution
{
    private const string PathSeparator = "/";

    // The textbook answer: for every folder, scan every other folder and ask whether
    // it is an ancestor - O(n^2) prefix comparisons, no ordering assumed and nothing
    // from this repo involved. This is the arm the sort-then-scan below has to
    // justify itself against.
    public static List<string> RemoveSubfoldersByPairwisePrefixCheck(string[] folders)
    {
        var kept = new List<string>();

        for (var i = 0; i < folders.Length; i++)
        {
            if (!IsSubfolderOfAny(folders, i))
            {
                kept.Add(folders[i]);
            }
        }

        return kept;
    }

    private static bool IsSubfolderOfAny(string[] folders, int index)
    {
        for (var other = 0; other < folders.Length; other++)
        {
            if (other != index &&
                folders[index].StartsWith(folders[other] + PathSeparator, StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }

    // Sorting ordinally puts every folder immediately after its own ancestors, so a
    // single linear scan only ever has to compare against the last folder it kept.
    // The sort is this repo's own MergeSort over an ArrayIndexedSequence<string>,
    // ordinal-compared so the result never depends on the current culture.
    //
    // The input array is copied first: MergeSort sorts in place, and a strategy that
    // reordered the caller's array would make the two arms non-interchangeable on the
    // same input (the ArrayPartitionBenchmarks precedent, moved inside the strategy
    // where it belongs).
    public static List<string> RemoveSubfoldersByMergeSortThenScan(string[] folders)
    {
        var sorted = folders.ToArray();
        var sequence = new ArrayIndexedSequence<string>(sorted);
        MergeSort.Sort<string, ArrayIndexedSequence<string>>(sequence, StringComparer.Ordinal);

        var kept = new List<string>();
        string? lastKept = null;

        foreach (var folder in sorted)
        {
            if (lastKept is null ||
                !folder.StartsWith(lastKept + PathSeparator, StringComparison.Ordinal))
            {
                kept.Add(folder);
                lastKept = folder;
            }
        }

        return kept;
    }
}
