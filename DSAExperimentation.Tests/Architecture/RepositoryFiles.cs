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

        while (directory is not null)
        {
            var marker = Path.Combine(directory.FullName, "ARCHITECTURE.md");

            if (File.Exists(marker))
            {
                break;
            }

            directory = directory.Parent;
        }

        return directory?.FullName
            ?? throw new InvalidOperationException(
                "No ancestor of the test output directory holds ARCHITECTURE.md, so the repository root is unknown.");
    }

    public static IEnumerable<SourceFile> SourceFilesIn(string directory)
    {
        if (!Directory.Exists(directory))
        {
            return [];
        }

        var obj = $"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}";
        var bin = $"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}";

        return Directory.EnumerateFiles(directory, "*.cs", SearchOption.AllDirectories)
            .Where(file => !file.Contains(obj) && !file.Contains(bin))
            .Select(file => new SourceFile(file));
    }

    // Repository-relative and forward-slashed, so a failure message reads the same
    // way on every platform and can be pasted straight into an allow-list. The base is
    // a plain directory rather than necessarily the repository root - the coverage and
    // solution scans below ask for a path relative to the folder they walked.
    public static string PathFromRoot(string baseDirectory, SourceFile file)
        => Path.GetRelativePath(baseDirectory, file).Replace(Path.DirectorySeparatorChar, '/');

    // A source file under the repository, as SourceFilesIn hands it out. Its own type so
    // that PathFromRoot's two positions say which role each one plays - the base to be
    // relative to, and the file - and a transposed call stops compiling instead of
    // quietly answering with a path from the wrong end, which is exactly the mistake
    // the coverage scan's `coverage` base invites. The conversion back to `string` is
    // one-way on purpose: it keeps File.ReadAllText and Path.GetFileName reading as they
    // did, while a `string` arriving into the file position would restore the ambiguity.
    internal readonly record struct SourceFile(string Path)
    {
        public static implicit operator string(SourceFile file) => file.Path;
    }
}
