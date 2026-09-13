namespace DSAExperimentation.Tests.Architecture;

// The architecture tests read the repository as TEXT rather than as types,
// because the rules they enforce span projects that cannot see each other: the
// benchmark project is not on the test project's reference list, so a reflection
// test would be blind in exactly the place the duplication these rules exist to
// catch actually happens. Both tests therefore need the same two primitives -
// where the repository root is, and which files under it are source - and this
// is where they live rather than in whichever test asked for them first.
internal static class RepositoryFiles
{
    public static string Root()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "ARCHITECTURE.md")))
        {
            directory = directory.Parent;
        }

        return directory?.FullName
            ?? throw new DirectoryNotFoundException(
                "No ancestor of the test output directory holds ARCHITECTURE.md, so the repository root is unknown.");
    }

    public static IEnumerable<string> SourceFilesIn(string directory)
    {
        if (!Directory.Exists(directory))
        {
            return [];
        }

        var obj = Path.DirectorySeparatorChar + "obj" + Path.DirectorySeparatorChar;
        var bin = Path.DirectorySeparatorChar + "bin" + Path.DirectorySeparatorChar;

        return Directory.EnumerateFiles(directory, "*.cs", SearchOption.AllDirectories)
            .Where(file => !file.Contains(obj) && !file.Contains(bin));
    }

    // Repository-relative and forward-slashed, so a failure message reads the same
    // way on every platform and can be pasted straight into an allow-list.
    public static string PathFromRoot(string root, string file)
        => Path.GetRelativePath(root, file).Replace(Path.DirectorySeparatorChar, '/');
}
