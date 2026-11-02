namespace DSAExperimentation.Domain.Computing.Paths;

// A path through a tree of names, asked which other paths lie inside it. A path lies inside another
// when it continues past it with the separator and more: "/a/b/c" lies inside "/a/b", and in a
// root-last notation "c.b.a" lies inside "b.a", but "/a/bc" does not lie inside "/a/b" though its
// first characters are the same. That is a name's descendants in the tree, the relation a trie of
// names holds. The test reads the separator in place rather than building "<path>/" to compare
// against, so asking it allocates nothing, and the notation's direction is a constant the JIT folds.
// The path is its own type, so the two paths in the question cannot be handed over the wrong way
// round.
/// <summary>A path through a tree of names, written in a notation.</summary>
/// <typeparam name="TNotation">The notation the path is written in, such as a filesystem's.</typeparam>
/// <param name="text">The path, such as "/a/b": below the root, with no separator at its open end.</param>
public readonly struct NamePath<TNotation>(string text)
    where TNotation : struct, IPathNotation
{
    /// <summary>The path, such as "/a/b".</summary>
    public string Text { get; } = text;

    /// <summary>Whether a path lies inside this one, at any depth.</summary>
    /// <param name="path">The path asked about.</param>
    /// <returns><see langword="true"/> when the path continues past this one with the separator; otherwise <see langword="false"/>.</returns>
    public bool HasInside(string path) =>
        path.Length > Text.Length && (TNotation.IsRootFirst ? HasInsideAfter(path) : HasInsideBefore(path));

    // Root first: this path, then the separator, then more.
    private bool HasInsideAfter(string path) =>
        path[Text.Length] == TNotation.Separator && path.StartsWith(Text, StringComparison.Ordinal);

    // Root last: more, then the separator, then this path.
    private bool HasInsideBefore(string path) =>
        path[path.Length - Text.Length - 1] == TNotation.Separator && path.EndsWith(Text, StringComparison.Ordinal);
}
