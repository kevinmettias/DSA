using DSAExperimentation.Domain.Computing.Paths;

namespace DSAExperimentation.Domain.Computing.Filesystem;

// Folder paths as LC 1233 writes them: folder names joined by '/', with one before the first, the
// root written first. The notation is the problem's *what* (ARCHITECTURE §17.6), and so is what it
// means for one path to lie inside another, which NamePath answers. How a strategy finds the
// outermost folders stays with it, and the baseline keeps its own copy (§17.5). Separator stays a
// constant for a caller that needs one; the notation reads it.
/// <summary>The notation folder paths are written in: '/' between names, the root first.</summary>
public readonly struct FolderPaths : IPathNotation
{
    /// <summary>The character before each folder name in a path.</summary>
    public const char Separator = '/';

    static char IPathNotation.Separator => Separator;

    /// <inheritdoc/>
    public static bool IsRootFirst => true;
}
