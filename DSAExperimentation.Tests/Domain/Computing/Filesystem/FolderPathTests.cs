using DSAExperimentation.Domain.Computing.Filesystem;

namespace DSAExperimentation.Tests.Domain.Computing.Filesystem;

public sealed partial class FolderPathTests
{
    // A folder, and a path inside it, one level down and deeper.
    public static TheoryData<string, string> Inside => new()
    {
        { "/a", "/a/b" },
        { "/a", "/a/b/c" },
    };

    // Sharing characters is not lying inside, since the next one must be the separator; a folder is
    // not inside itself, and a folder's parent is not inside it.
    public static TheoryData<string, string> NotInside => new()
    {
        { "/a/b", "/a/bc" },
        { "/a", "/ab" },
        { "/a", "/a" },
        { "/a/b", "/a" },
    };

    [Theory]
    [MemberData(nameof(Inside))]
    public void HasInside_PathPastTheFolderAndASeparator_IsTrue(string folder, string path) =>
        Assert.True(new FolderPath(folder).HasInside(path));

    [Theory]
    [MemberData(nameof(NotInside))]
    public void HasInside_PathNotPastTheFolderAndASeparator_IsFalse(string folder, string path) =>
        Assert.False(new FolderPath(folder).HasInside(path));
}
