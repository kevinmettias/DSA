using DSAExperimentation.LeetCode.Catalog.Storage;

namespace DSAExperimentation.LeetCode.Catalog.Tests.Storage;

// Against the checked-in snapshot's hand-kept corrections.
public sealed partial class CorrectionFilesTests
{
    private const string LowestCommonAncestor = "lowest-common-ancestor-of-a-binary-tree";

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

    private static IEnumerable<LeetCodeCase> SourceCases(string titleSlug)
    {
        var examples = LeetCodeCatalog.Question(titleSlug).Examples;
        var dataset = CaseFiles.Read(LeetCodeCaseSource.Dataset, titleSlug);
        var run = CaseFiles.Read(LeetCodeCaseSource.LeetCodeRun, titleSlug);

        return examples.Concat(dataset).Concat(run);
    }
}
