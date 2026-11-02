using FolderRead = DSAExperimentation.Tests.Architecture.FolderGraph.FolderRead;

namespace DSAExperimentation.Tests.Architecture;

// The cycles FolderGraph's reads form across tiers. DataStructures, Algorithms and Domain may read
// one another (ARCHITECTURE.md section 17.2); what keeps them peers rather than one tangle is that
// no folder reads itself back through another tier. A cycle inside one tier is not this rule's
// business: Algorithms' walks, traversals and reductions form one today.
internal static class FolderCycles
{
    // Every read that crosses a tier and closes a cycle, each stated as that cycle, from the
    // reading folder around to it again. Any cycle across tiers holds at least one such read, so a
    // graph where none closes one has no cycle across tiers.
    public static IEnumerable<string> AcrossTiers(IReadOnlyCollection<FolderRead> reads)
    {
        var readsOf = reads.ToLookup(read => read.Reader, read => read.Read);

        return reads
            .Distinct()
            .Where(read => TierOf(read.Reader) != TierOf(read.Read))
            .Select(read => CycleThrough(readsOf, read))
            .OfType<List<string>>()
            .Select(cycle => string.Join(" -> ", cycle));
    }

    private static string TierOf(string folder) => folder.Split('/')[0];

    // The shortest cycle through one read: its reader, the folder it reads, and the shortest chain
    // of reads from there back to the reader. Null when the read folder never reaches the reader.
    private static List<string>? CycleThrough(ILookup<string, string> readsOf, FolderRead read)
    {
        var cameFrom = new Dictionary<string, string> { [read.Read] = read.Read };
        var frontier = new Queue<string>([read.Read]);

        while (frontier.TryDequeue(out var folder))
        {
            if (folder == read.Reader)
            {
                return ChainEndingAt(cameFrom, folder).Prepend(read.Reader).ToList();
            }

            foreach (var next in readsOf[folder])
            {
                if (cameFrom.TryAdd(next, folder))
                {
                    frontier.Enqueue(next);
                }
            }
        }

        return null;
    }

    // The chain the search walked to reach a folder, from where it started.
    private static IEnumerable<string> ChainEndingAt(Dictionary<string, string> cameFrom, string end)
    {
        var chain = new List<string> { end };

        while (cameFrom[chain[^1]] != chain[^1])
        {
            chain.Add(cameFrom[chain[^1]]);
        }

        chain.Reverse();

        return chain;
    }
}
