using static DSAExperimentation.Tests.Architecture.FolderGraph;

namespace DSAExperimentation.Tests.Architecture;

// What FolderGraph reads as a folder's reads, stated as cases. The layering test builds the graph it
// checks for cycles from these.
public sealed partial class FolderGraphTests
{
    // A path resolves to the deepest folder it reaches, not the first: Algorithms holding a file of
    // its own does not make a search's name Algorithms'.
    [Fact]
    public void DeepestFolderOf_FindsTheDeepestFolderThePathReaches()
        => Assert.Equal(
            "Algorithms/Searching",
            DeepestFolderOf("Algorithms/Searching/BinarySearch/LowerBound", new HashSet<string> { "Algorithms", "Algorithms/Searching", "Algorithms/Sorting" }));

    [Fact]
    public void DeepestFolderOf_IsNullForAPathThatReachesNoFolder()
        => Assert.Null(DeepestFolderOf("Domain/Games", new HashSet<string> { "Domain/Games/Life" }));

    [Fact]
    public void EnclosingFolders_RunOutwardToTheTierFolder()
        => Assert.Equal(["Domain/Games", "Domain"], EnclosingFolders("Domain/Games/Life"));

    // A folder reads what its names reach and each enclosing folder that holds a file, never
    // itself, and each once.
    [Fact]
    public void ReadsFrom_AddsTheEnclosingFoldersAndDropsTheFolderItself()
        => Assert.Equal(
            [new("Domain/Games/Life", "DataStructures/Graph"), new("Domain/Games/Life", "Domain")],
            ReadsFrom(
                "Domain/Games/Life",
                ["DataStructures/Graph/GridGraph", "DataStructures/Graph/GridGraph/Neighbors", "Domain/Games/Life/ConwayLife"],
                new HashSet<string> { "DataStructures/Graph", "Domain/Games/Life", "Domain" }));
}
