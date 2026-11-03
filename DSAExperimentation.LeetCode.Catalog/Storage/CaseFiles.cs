namespace DSAExperimentation.LeetCode.Catalog.Storage;

// Snapshot/Cases/<source>/: the cases one source supplies, one file per question. A case's source is
// the folder it was read from, so the files do not repeat it on every case.
internal static class CaseFiles
{
    private const string CasesFolderName = "Cases";

    public static IReadOnlyList<LeetCodeCase> Read(LeetCodeCaseSource source, string titleSlug)
    {
        var path = PathOf(source, titleSlug);

        if (!CatalogJson.TryRead<List<LeetCodeCase>>(path, out var cases))
        {
            return [];
        }

        return cases.Select(testCase => testCase with { Source = source }).ToList();
    }

    // An empty list deletes the file rather than writing "[]", so a question with no cases from a
    // source has no file for it, and the folder lists exactly the questions the source covers.
    public static bool TryWrite(LeetCodeCaseSource source, string titleSlug, IReadOnlyList<LeetCodeCase> cases)
    {
        var path = PathOf(source, titleSlug);

        if (cases.Count > 0)
        {
            return CatalogJson.TryWrite(path, cases);
        }

        var existed = File.Exists(path);
        File.Delete(path);
        return existed;
    }

    private static string PathOf(LeetCodeCaseSource source, string titleSlug)
    {
        var folder = Path.Combine(CatalogJson.SnapshotDirectory, CasesFolderName, source.ToString());
        return CatalogJson.PathOf(folder, titleSlug);
    }
}
