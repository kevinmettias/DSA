using System.Text.Json;
using DSAExperimentation.LeetCode.Catalog.Questions;
using DSAExperimentation.LeetCode.Catalog.Storage;

namespace DSAExperimentation.LeetCode.Catalog.Tests.Storage;

// Against the checked-in snapshot's hand-kept corrections.
public sealed partial class CorrectionFilesTests
{
    private const string LowestCommonAncestor = "lowest-common-ancestor-of-a-binary-tree";
    private const string PeekingIterator = "peeking-iterator";

    [Fact]
    public void Read_CorrectedQuestion_SaysWhyOfEachCorrection()
    {
        var corrections = CorrectionFiles.Read(LowestCommonAncestor);

        Assert.NotEmpty(corrections);
        Assert.All(corrections, correction => Assert.False(string.IsNullOrWhiteSpace(correction.Reason)));
    }

    [Fact]
    public void Read_UncorrectedQuestion_IsEmpty() => Assert.Empty(CorrectionFiles.Read("two-sum"));

    // A correction whose case its sources no longer hold corrects nothing, and would sit in the file unseen.
    [Fact]
    public void TitleSlugs_EveryCorrection_NamesACaseItsSourcesHold()
    {
        var strays = CorrectionFiles.TitleSlugs()
            .SelectMany(titleSlug => CorrectionFiles.Read(titleSlug)
                .Where(correction => !SourceCases(titleSlug).Any(testCase => testCase.Inputs.SequenceEqual(correction.Inputs)))
                .Select(correction => $"{titleSlug}: {string.Join(", ", correction.Inputs)}"))
            .ToList();

        Assert.Empty(strays);
    }

    [Fact]
    public void SignatureFor_CorrectedQuestion_SaysWhy()
    {
        var correction = CorrectionFiles.SignatureFor(PeekingIterator);

        Assert.NotNull(correction);
        Assert.False(string.IsNullOrWhiteSpace(correction.Reason));
    }

    [Fact]
    public void SignatureFor_UncorrectedQuestion_IsNull() => Assert.Null(CorrectionFiles.SignatureFor("two-sum"));

    // The sync writes a corrected signature into the question's file, so the snapshot already holds it, and a
    // sync that skipped the correction would show here as a file no longer agreeing with it.
    [Fact]
    public void SignatureTitleSlugs_EveryCorrection_IsTheSnapshotQuestionsSignature()
    {
        var strays = CorrectionFiles.SignatureTitleSlugs()
            .Where(titleSlug => !IsSnapshotSignatureCorrected(titleSlug))
            .ToList();

        Assert.Empty(strays);
    }

    [Fact]
    public void WithCorrectedSignature_SnapshotQuestion_ChangesNoFile()
    {
        QuestionFiles.TryRead(PeekingIterator, out LeetCodeQuestion? question);
        Assert.NotNull(question);

        var corrected = CorrectionFiles.WithCorrectedSignature(question);

        Assert.False(QuestionFiles.TryWrite(corrected));
    }

    private static bool IsSnapshotSignatureCorrected(string titleSlug)
    {
        var correction = CorrectionFiles.SignatureFor(titleSlug) ?? throw new InvalidOperationException($"{titleSlug} lists no correction.");
        var recorded = JsonSerializer.Serialize(LeetCodeCatalog.Question(titleSlug).Signature);

        return recorded == JsonSerializer.Serialize(correction.Signature);
    }

    private static IEnumerable<LeetCodeCase> SourceCases(string titleSlug)
    {
        var examples = LeetCodeCatalog.Question(titleSlug).Examples;
        var dataset = CaseFiles.Read(LeetCodeCaseSource.Dataset, titleSlug);
        var run = CaseFiles.Read(LeetCodeCaseSource.LeetCodeRun, titleSlug);

        return examples.Concat(dataset).Concat(run);
    }
}
