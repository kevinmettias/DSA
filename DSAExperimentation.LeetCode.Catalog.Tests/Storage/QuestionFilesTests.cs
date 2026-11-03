using DSAExperimentation.LeetCode.Catalog.Questions;
using DSAExperimentation.LeetCode.Catalog.Storage;

namespace DSAExperimentation.LeetCode.Catalog.Tests.Storage;

// Against the checked-in snapshot. The write test writes back exactly what it read, which must
// change nothing: that is the property that keeps a refresh which finds nothing new from touching
// a single file.
public sealed partial class QuestionFilesTests
{
    private const string TwoSum = "two-sum";

    [Fact]
    public void Slugs_Snapshot_IncludesAnEnrolledQuestion()
        => Assert.Contains(TwoSum, QuestionFiles.Slugs());

    [Fact]
    public void TryRead_EnrolledSlug_ReadsThatQuestion()
    {
        var isRead = QuestionFiles.TryRead(TwoSum, out var question);

        Assert.True(isRead);
        Assert.NotNull(question);
        Assert.Equal((1, "Two Sum"), (question.FrontendId, question.Title));
    }

    [Fact]
    public void TryRead_SlugNotEnrolled_ReportsNothingRead()
    {
        var isRead = QuestionFiles.TryRead("no-such-question", out _);

        Assert.False(isRead);
    }

    [Fact]
    public void TryWrite_QuestionAsRead_ChangesNothing()
    {
        QuestionFiles.TryRead(TwoSum, out LeetCodeQuestion? question);
        Assert.NotNull(question);

        var isWritten = QuestionFiles.TryWrite(question);

        Assert.False(isWritten);
    }
}
