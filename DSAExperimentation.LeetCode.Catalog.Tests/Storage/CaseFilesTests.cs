using DSAExperimentation.LeetCode.Catalog.Storage;

namespace DSAExperimentation.LeetCode.Catalog.Tests.Storage;

// Against the checked-in snapshot; the write tests change nothing when they pass.
public sealed partial class CaseFilesTests
{
    private const string TwoSum = "two-sum";
    private const string NotEnrolled = "no-such-question";

    [Fact]
    public void Read_QuestionTheDatasetCovers_TagsEveryCaseWithTheDataset()
    {
        var cases = CaseFiles.Read(LeetCodeCaseSource.Dataset, TwoSum);

        Assert.NotEmpty(cases);
        Assert.All(cases, testCase => Assert.Equal(LeetCodeCaseSource.Dataset, testCase.Source));
    }

    [Fact]
    public void Read_QuestionWithNoFileForTheSource_ReadsNoCases()
    {
        var cases = CaseFiles.Read(LeetCodeCaseSource.LeetCodeRun, TwoSum);

        Assert.Empty(cases);
    }

    [Fact]
    public void TryWrite_CasesAsRead_ChangesNothing()
    {
        var cases = CaseFiles.Read(LeetCodeCaseSource.Dataset, TwoSum);

        var isWritten = CaseFiles.TryWrite(LeetCodeCaseSource.Dataset, TwoSum, cases);

        Assert.False(isWritten);
    }

    [Fact]
    public void TryWrite_NoCasesWhereNoFileExists_ChangesNothing()
    {
        var isWritten = CaseFiles.TryWrite(LeetCodeCaseSource.Dataset, NotEnrolled, []);

        Assert.False(isWritten);
    }
}
