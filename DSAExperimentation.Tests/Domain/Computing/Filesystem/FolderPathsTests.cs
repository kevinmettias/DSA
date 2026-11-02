using DSAExperimentation.Domain.Computing.Filesystem;

namespace DSAExperimentation.Tests.Domain.Computing.Filesystem;

public sealed partial class FolderPathsTests
{
    [Fact]
    public void Separator_IsTheSlash() => Assert.Equal('/', FolderPaths.Separator);
}
