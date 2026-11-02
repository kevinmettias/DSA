using DSAExperimentation.Domain.Computing.Paths;

namespace DSAExperimentation.Domain.Computing.Networking;

// Domain names: labels joined by '.', the root written last, so "mail.example.com" names mail inside
// example inside com. The same tree of names a folder path walks, written in the other direction, so
// NamePath's rule serves it with the ends swapped: "mail.example.com" lies inside "example.com", and
// "notexample.com" does not.
/// <summary>The notation domain names are written in: '.' between labels, the root last.</summary>
public readonly struct DomainNames : IPathNotation
{
    /// <summary>The character between two labels in a domain name.</summary>
    public const char Separator = '.';

    static char IPathNotation.Separator => Separator;

    /// <inheritdoc/>
    public static bool IsRootFirst => false;
}
