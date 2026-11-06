namespace DSAExperimentation.LeetCode.Catalog.Storage;

// Corrections to the outputs a source records, kept by hand under Snapshot/Corrections/, one file per question,
// beside the files DSAExperimentation.LeetCode.Catalog.Sync writes and never touched by it. A source is wrong at
// times - the dataset's reference answered Maximum Product of the Length of Two Palindromic Substrings wrongly
// on three cases, and printed nothing for every node Lowest Common Ancestor of a Binary Tree answers with - and
// an example's output is at times stated only in prose, which the scrape cannot record. Each correction names
// the case by its inputs, says the output it should record (null where none can be recorded), and says why,
// with what was run to know it.
internal static class CorrectionFiles
{
    private const string CorrectionsFolderName = "Corrections";

    private static string Folder => Path.Combine(CatalogJson.SnapshotDirectory, CorrectionsFolderName);

    // The questions a correction file is kept for.
    public static IReadOnlyList<string> TitleSlugs() => CatalogJson.SlugsIn(Folder);

    public static IReadOnlyList<CaseCorrection> Read(string titleSlug)
    {
        var path = CatalogJson.PathOf(Folder, titleSlug);

        if (!CatalogJson.TryRead<List<CaseCorrection>>(path, out var corrections))
        {
            return [];
        }

        return corrections;
    }
}
