using DSAExperimentation.Domain.Computing.Paths;

namespace DSAExperimentation.Domain.Computing.Filesystem;

// Windows paths: folder names joined by '\', the root - a drive such as "C:" - written first. The
// same tree of names as a folder path, written with the other slash, so NamePath's rule serves it
// unchanged: "C:\a\b" lies inside "C:\a", and "C:\ab" does not.
/// <summary>The notation Windows paths are written in: '\' between names, the root first.</summary>
public readonly struct WindowsPaths : IPathNotation
{
    /// <summary>The character between two names in a Windows path.</summary>
    public const char Separator = '\\';

    static char IPathNotation.Separator => Separator;

    /// <inheritdoc/>
    public static bool IsRootFirst => true;
}
