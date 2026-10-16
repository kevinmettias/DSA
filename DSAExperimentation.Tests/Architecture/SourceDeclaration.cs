namespace DSAExperimentation.Tests.Architecture;

// What a ProblemFolderSource declaration is: a method, a field or property, or a type.
internal enum DeclarationKind
{
    Method,
    Member,
    Type,
}

// One declaration in a problem folder: its kind and name, the file and character span it covers,
// its text, its signature - a method's header through its parameter list, a type's primary-
// constructor parameters, nothing for a field or property - the type it is declared in (null at
// file level), and whether it is static. Compared by reference: two declarations are the same
// only when they are the same place in the source.
internal sealed class SourceDeclaration
{
    public required DeclarationKind Kind { get; init; }

    public required string Name { get; init; }

    public required string File { get; init; }

    public required int Start { get; init; }

    public required int End { get; init; }

    public required string Text { get; init; }

    public required string Signature { get; init; }

    public required SourceDeclaration? Owner { get; init; }

    public required bool IsStatic { get; init; }
}
