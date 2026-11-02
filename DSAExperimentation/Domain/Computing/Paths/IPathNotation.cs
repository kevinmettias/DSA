namespace DSAExperimentation.Domain.Computing.Paths;

// How a notation writes a path from the root of a tree of names: the separator between names, and
// which end the root is written at. A filesystem writes it first, "/a/b/c"; a domain name writes it
// last, "c.b.a". The two are one structure written in opposite directions, so one rule of what lies
// inside what serves both (NamePath). The members are static abstract members of a struct, so the
// direction folds to a constant in a caller.
/// <summary>A notation for paths through a tree of names.</summary>
public interface IPathNotation
{
    /// <summary>The character written between two names.</summary>
    static abstract char Separator { get; }

    /// <summary>Whether the root's name is written first, as a filesystem path writes it, rather than last.</summary>
    static abstract bool IsRootFirst { get; }
}
