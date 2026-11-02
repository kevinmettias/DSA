using DSAExperimentation.Domain.Computing.Filesystem;
using DSAExperimentation.Domain.Computing.Paths;

namespace DSAExperimentation.Tests.Domain.Computing.Paths;

// Folder paths, root first, and domain names, root last: one rule written in opposite directions.
// The domain-name notation is test-only, since no problem here reads one (ARCHITECTURE §17.10).
public sealed partial class NamePathTests
{
    // A folder, and a path inside it, one level down and deeper.
    public static TheoryData<string, string> FoldersAndInside => new()
    {
        { "/a", "/a/b" },
        { "/a", "/a/b/c" },
    };

    // Sharing characters is not lying inside, since the next one must be the separator; a folder is
    // not inside itself, and a folder's parent is not inside it.
    public static TheoryData<string, string> FoldersAndNotInside => new()
    {
        { "/a/b", "/a/bc" },
        { "/a", "/ab" },
        { "/a", "/a" },
        { "/a/b", "/a" },
    };

    // The same cases in a root-last notation: a subdomain lies inside its domain.
    public static TheoryData<string, string> DomainsAndInside => new()
    {
        { "example.com", "mail.example.com" },
        { "com", "a.b.com" },
    };

    public static TheoryData<string, string> DomainsAndNotInside => new()
    {
        { "example.com", "notexample.com" },
        { "example.com", "example.com" },
        { "mail.example.com", "example.com" },
    };

    [Theory]
    [MemberData(nameof(FoldersAndInside))]
    public void HasInside_FolderPathPastTheFolderAndASeparator_IsTrue(string folder, string path) =>
        Assert.True(new NamePath<FolderPaths>(folder).HasInside(path));

    [Theory]
    [MemberData(nameof(FoldersAndNotInside))]
    public void HasInside_FolderPathNotPastTheFolderAndASeparator_IsFalse(string folder, string path) =>
        Assert.False(new NamePath<FolderPaths>(folder).HasInside(path));

    [Theory]
    [MemberData(nameof(DomainsAndInside))]
    public void HasInside_DomainNameBeforeTheDomainAndASeparator_IsTrue(string domain, string name) =>
        Assert.True(new NamePath<DomainNames>(domain).HasInside(name));

    [Theory]
    [MemberData(nameof(DomainsAndNotInside))]
    public void HasInside_DomainNameNotBeforeTheDomainAndASeparator_IsFalse(string domain, string name) =>
        Assert.False(new NamePath<DomainNames>(domain).HasInside(name));

    private readonly struct DomainNames : IPathNotation
    {
        public static char Separator => '.';

        public static bool IsRootFirst => false;
    }
}
