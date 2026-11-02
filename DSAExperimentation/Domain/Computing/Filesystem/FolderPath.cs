namespace DSAExperimentation.Domain.Computing.Filesystem;

// A folder, named by its path, asked which other paths lie inside it. A path lies inside a folder when
// it is the folder's path, then the separator, then more: "/a/b/c" is inside "/a/b", but "/a/bc" is
// not, though its first characters are the same. The test reads the separator in place rather than
// building "<folder>/" to compare against, so asking it allocates nothing. The folder is its own type,
// so the two paths in the question cannot be handed over the wrong way round.
/// <summary>A folder, named by its path, below the root.</summary>
/// <param name="Path">The folder's path, such as "/a/b": below the root, with no separator at its end.</param>
public readonly record struct FolderPath(string Path)
{
    /// <summary>Whether a path lies inside this folder, at any depth.</summary>
    /// <param name="path">The path asked about.</param>
    /// <returns><see langword="true"/> when the path continues past this folder's path with the separator; otherwise <see langword="false"/>.</returns>
    public bool HasInside(string path) =>
        path.Length > Path.Length
        && path[Path.Length] == FolderPaths.Separator
        && path.StartsWith(Path, StringComparison.Ordinal);
}
