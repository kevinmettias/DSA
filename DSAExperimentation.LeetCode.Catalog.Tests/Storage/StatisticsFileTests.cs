using DSAExperimentation.LeetCode.Catalog.Storage;

namespace DSAExperimentation.LeetCode.Catalog.Tests.Storage;

// Against the checked-in snapshot; the write test changes nothing when it passes.
public sealed partial class StatisticsFileTests
{
    [Fact]
    public void Read_Snapshot_HoldsADatedFigureForAnEnrolledQuestion()
    {
        var file = StatisticsFile.Read();

        Assert.NotEqual(DateOnly.MinValue, file.AsOf);
        Assert.InRange(file.Questions["two-sum"].AcceptanceRatePercent, 0, 100);
    }

    [Fact]
    public void TryWrite_FileAsRead_ChangesNothing()
    {
        var isWritten = StatisticsFile.Read().TryWrite();

        Assert.False(isWritten);
    }
}
