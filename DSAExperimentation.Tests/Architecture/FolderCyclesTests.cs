using static DSAExperimentation.Tests.Architecture.FolderCycles;

namespace DSAExperimentation.Tests.Architecture;

// What FolderCycles reads as a cycle across tiers, stated as cases. The layering test passes the
// peer tiers only when this finds none in the framework project.
public sealed partial class FolderCyclesTests
{
    [Fact]
    public void AcrossTiers_IgnoresACycleInsideOneTier()
        => Assert.Empty(AcrossTiers(
        [
            new("Algorithms/Walking", "Algorithms/Reducing"),
            new("Algorithms/Reducing", "Algorithms/Walking"),
        ]));

    [Fact]
    public void AcrossTiers_IgnoresReadsAcrossTiersThatNeverComeBack()
        => Assert.Empty(AcrossTiers(
        [
            new("Domain/Games/Life", "DataStructures/Graph"),
            new("Algorithms/Searching", "DataStructures/Graph"),
            new("DataStructures/Graph", "Domain/Mathematics/Geometry"),
        ]));

    [Fact]
    public void AcrossTiers_FindsTwoFoldersReadingEachOtherAcrossTiers()
        => Assert.Contains(
            "Domain/Games/Life -> DataStructures/Graph -> Domain/Games/Life",
            AcrossTiers(
            [
                new("Domain/Games/Life", "DataStructures/Graph"),
                new("DataStructures/Graph", "Domain/Games/Life"),
            ]));

    // The way back runs through a second folder of the read tier, so no single pair of folders
    // reads each other.
    [Fact]
    public void AcrossTiers_FindsACycleThatReturnsThroughAnotherFolder()
        => Assert.Equal(
            ["Domain/Games/Life -> DataStructures/Graph -> DataStructures/Graph/Grids -> Domain/Games/Life"],
            AcrossTiers(
            [
                new("Domain/Games/Life", "DataStructures/Graph"),
                new("DataStructures/Graph", "DataStructures/Graph/Grids"),
                new("DataStructures/Graph/Grids", "Domain/Games/Life"),
            ]).Where(cycle => cycle.StartsWith("Domain/", StringComparison.Ordinal)));
}
