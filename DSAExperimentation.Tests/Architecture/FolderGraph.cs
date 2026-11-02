namespace DSAExperimentation.Tests.Architecture;

// Which folder of the framework project reads which. A folder is named by its path inside the
// project, written with '/', so its first segment is its tier. FolderCycles reads the graph for
// cycles across tiers.
internal static class FolderGraph
{
    // The folders one folder's files read: each name they write, resolved to the deepest folder
    // holding a file that its path reaches, and every enclosing folder that holds a file. A file may
    // name a type of an enclosing folder with no using and no qualifier, which no text shows, so it
    // is taken to read them all. That can add a cycle, never hide one.
    public static IEnumerable<FolderRead> ReadsFrom(string folder, IEnumerable<string> namedPaths, IReadOnlySet<string> folders)
        => namedPaths
            .Select(path => DeepestFolderOf(path, folders))
            .OfType<string>()
            .Concat(EnclosingFolders(folder).Where(folders.Contains))
            .Where(read => read != folder)
            .Distinct()
            .Select(read => new FolderRead(folder, read));

    // The longest prefix of a path that is a folder holding a file, or null when none is, as for a
    // namespace that only groups others.
    public static string? DeepestFolderOf(string path, IReadOnlySet<string> folders)
    {
        while (!folders.Contains(path))
        {
            var slash = path.LastIndexOf('/');

            if (slash < 0)
            {
                return null;
            }

            path = path[..slash];
        }

        return path;
    }

    // The folders that enclose one, nearest first, down to its tier's own folder.
    public static IEnumerable<string> EnclosingFolders(string folder)
    {
        for (var slash = folder.LastIndexOf('/'); slash > 0; slash = folder.LastIndexOf('/', slash - 1))
        {
            yield return folder[..slash];
        }
    }

    // One folder reading another: a file in the reader names a type in the read folder.
    public readonly record struct FolderRead(string Reader, string Read);
}
