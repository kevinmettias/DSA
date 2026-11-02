namespace DSAExperimentation.Domain.Computing.Filesystem;

// Folder paths as LC 1233 writes them: folder names joined by '/', with one before the first. The
// separator is the problem's *what* (ARCHITECTURE §17.6), and so is what it means for one path to lie
// inside another, which FolderPath answers. How a strategy finds the outermost folders stays with it,
// and the baseline keeps its own copy (§17.5).
/// <summary>The separator folder paths are written with.</summary>
public static class FolderPaths
{
    /// <summary>The character before each folder name in a path.</summary>
    public const char Separator = '/';
}
