using DSAExperimentation.LeetCode.Catalog.Storage;
using DSAExperimentation.LeetCode.Catalog.Sync.Dataset;

namespace DSAExperimentation.LeetCode.Catalog.Sync;

// `dataset [source...]`: imports LeetCodeDataset's cases for every enrolled question, from the
// given JSONL paths or URLs, or from the dataset's own two files on Hugging Face. It reads each
// question's signature and constraints from the catalog, so `questions` must have run first. A
// question the dataset does not cover loses any cases file it had, so the folder always lists
// exactly the questions the last import covered. The dataset is a fixed release, not a feed, so
// this is run when it publishes a new one, not weekly.
internal static class DatasetCommand
{
    public const string Verb = "dataset";

    public static async Task<int> RunAsync(IReadOnlyList<string> sources, TextWriter output, CancellationToken cancellationToken)
    {
        var chosen = sources.Count > 0 ? sources : LeetCodeDataset.DefaultSources;
        var rows = await LeetCodeDataset.ReadRowsAsync(chosen, cancellationToken);
        var tally = new DatasetTally();

        foreach (var slug in QuestionFiles.Slugs())
        {
            var conversion = ConvertFor(slug, rows);
            CaseFiles.TryWrite(LeetCodeCaseSource.Dataset, slug, conversion.Cases);
            tally.Add(conversion);
        }

        tally.Report(output);
        return 0;
    }

    // The dataset covers function questions only; a design question's cases are call scripts.
    private static DatasetConversion ConvertFor(string slug, IReadOnlyDictionary<string, DatasetRowDto> rows)
    {
        if (!rows.TryGetValue(slug, out var row) || !QuestionFiles.TryRead(slug, out var question))
        {
            return DatasetConversion.None;
        }

        return question.Signature.IsDesign ? DatasetConversion.None : new DatasetCaseConverter(question).Convert(row.Cases);
    }
}
