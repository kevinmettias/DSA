using DSAExperimentation.LeetCode.Catalog.Questions;

namespace DSAExperimentation.LeetCode.Catalog.Storage;

// Corrections to what a source records, kept by hand under Snapshot/Corrections/, one file per question.
//
// A case correction fixes an output. A source is wrong at times - the dataset's reference answered Maximum
// Product of the Length of Two Palindromic Substrings wrongly on three cases, and printed nothing for every node
// Lowest Common Ancestor of a Binary Tree answers with - and an example's output is at times stated only in
// prose, which the scrape cannot record. Each names the case by its inputs, says the output it should record
// (null where none can be recorded), and says why, with what was run to know it. LeetCodeCatalog.Cases applies
// them, and DSAExperimentation.LeetCode.Catalog.Sync never writes them.
//
// A signature correction, under Signatures/, fixes LeetCode's metaData where it misdescribes its own judge:
// Peeking Iterator's lists a function taking nums and commands, where its judge replays a call script on a
// class, as its examples show. The sync writes the corrected signature into the question's file in place of
// the metaData's, so every reader of the snapshot - the harness and the benchmark generator alike - sees it.
internal static class CorrectionFiles
{
    private const string CorrectionsFolderName = "Corrections";
    private const string SignaturesFolderName = "Signatures";

    private static string Folder => Path.Combine(CatalogJson.SnapshotDirectory, CorrectionsFolderName);

    private static string SignaturesFolder => Path.Combine(Folder, SignaturesFolderName);

    // The questions a case correction file is kept for.
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

    // The questions a signature correction is kept for.
    public static IReadOnlyList<string> SignatureTitleSlugs() => CatalogJson.SlugsIn(SignaturesFolder);

    public static SignatureCorrection? SignatureFor(string titleSlug)
    {
        var path = CatalogJson.PathOf(SignaturesFolder, titleSlug);

        if (!CatalogJson.TryRead<SignatureCorrection>(path, out var correction))
        {
            return null;
        }

        return correction;
    }

    // The question as the snapshot records it: with its signature corrected where a correction is kept.
    public static LeetCodeQuestion WithCorrectedSignature(LeetCodeQuestion question)
    {
        if (SignatureFor(question.TitleSlug) is not { } correction)
        {
            return question;
        }

        return question with { Signature = correction.Signature };
    }
}
