using System.Diagnostics.CodeAnalysis;
using DSAExperimentation.LeetCode.Catalog.Questions;

namespace DSAExperimentation.LeetCode.Catalog.Storage;

// Snapshot/Questions/: one LeetCodeQuestion per file, named by its slug. A question's file existing
// is what enrols it: the sync tool refreshes exactly the questions that have one, so the set cannot
// drift from a list kept somewhere else.
internal static class QuestionFiles
{
    private const string QuestionsFolderName = "Questions";

    private static string Folder => Path.Combine(CatalogJson.SnapshotDirectory, QuestionsFolderName);

    public static IReadOnlyList<string> Slugs() => CatalogJson.SlugsIn(Folder);

    public static bool TryRead(string titleSlug, [MaybeNullWhen(false)] out LeetCodeQuestion question)
    {
        var path = CatalogJson.PathOf(Folder, titleSlug);
        return CatalogJson.TryRead(path, out question);
    }

    public static bool TryWrite(LeetCodeQuestion question)
    {
        var path = CatalogJson.PathOf(Folder, question.TitleSlug);
        return CatalogJson.TryWrite(path, question);
    }
}
