using DSAExperimentation.LeetCode.Catalog.Storage;

namespace DSAExperimentation.LeetCode.Catalog.Tests.Storage;

// Against the checked-in snapshot; the write test changes nothing when it passes.
public sealed partial class ContestRatingsFileTests
{
    [Fact]
    public void Read_Snapshot_NamesItsSourceAndHoldsRatings()
    {
        var file = ContestRatingsFile.Read();

        Assert.Equal("https://github.com/zerotrac/leetcode_problem_rating", file.Source);
        Assert.NotEmpty(file.Questions);
    }

    [Fact]
    public void TryWrite_FileAsRead_ChangesNothing()
    {
        var isWritten = ContestRatingsFile.Read().TryWrite();

        Assert.False(isWritten);
    }
}
