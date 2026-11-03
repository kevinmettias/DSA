using System.Collections.Immutable;
using DSAExperimentation.Domain.Mathematics.Lattices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DSAExperimentation.Tests.Architecture;

// One source per fact for the grid's steps (ARCHITECTURE.md 17.5, and the owner's ruling of
// 2026-11-03 that they are the square lattice's facts): a solution reads GridDirections' tables
// rather than writing its own. LibraryValueCopies sees only constants, and a table is a collection of
// literal pairs, so this reads the solutions' syntax instead. A copy is a collection holding exactly
// one table's steps, in any order:
// - four or more literal (row, column) pairs, written as tuples or as two-number arrays;
// - or the five-number run {-1, 0, 1, 0, -1}, whose neighbouring numbers are the four orthogonal
//   steps.
// A problem's own table, such as a turning cycle or its notation's direction codes, is built from the
// named steps instead, so it never reads as a literal copy.
internal static class LatticeTableCopies
{
    // The fewest steps a table holds: the four orthogonal ones.
    private const int FewestSteps = 4;

    // A run of numbers whose neighbours pair into the four orthogonal steps is one longer than them.
    private const int OrthogonalRunLength = FewestSteps + 1;

    // The two numbers of a pair: how far down the rows, and how far across the columns.
    private const int PairLength = 2;

    private static readonly (string Name, ImmutableArray<GridStep> Steps)[] Tables =
    [
        (nameof(GridDirections.Orthogonal), GridDirections.Orthogonal),
        (nameof(GridDirections.Diagonal), GridDirections.Diagonal),
        (nameof(GridDirections.Octilinear), GridDirections.Octilinear),
        (nameof(GridDirections.LShaped), GridDirections.LShaped),
    ];

    // Each literal copy of a table in the solutions, by its repository-relative file and line.
    public static IReadOnlyList<Copy> In(Compilation solutions)
    {
        var root = RepositoryFiles.Root();

        return [.. solutions.SyntaxTrees
            .Where(tree => File.Exists(tree.FilePath))
            .SelectMany(tree => CopiesIn(tree, RepositoryFiles.PathFromRoot(root, new RepositoryFiles.SourceFile(tree.FilePath))))
            .OrderBy(copy => copy.Location, StringComparer.Ordinal)];
    }

    // Each literal copy of a table in one file, located by the file name given.
    public static IEnumerable<Copy> CopiesIn(SyntaxTree tree, string fileName) =>
        tree.GetRoot().DescendantNodes()
            .Select(node => (Node: node, Steps: StepsWritten(node)))
            .Where(written => written.Steps.Count >= FewestSteps)
            .SelectMany(written => Tables
                .Where(table => IsSameSteps(written.Steps, table.Steps))
                .Select(table => new Copy($"{fileName}:{LineOf(written.Node)}", table.Name)));

    private static bool IsSameSteps(IReadOnlyList<(int DeltaRow, int DeltaCol)> written, ImmutableArray<GridStep> table) =>
        written.Count == table.Length
        && written.ToHashSet().SetEquals(table.Select(step => (step.DeltaRow, step.DeltaCol)));

    private static int LineOf(SyntaxNode node) => node.GetLocation().GetLineSpan().StartLinePosition.Line + 1;

    // The steps a collection literal writes, or none when it is not one made only of literal pairs.
    private static IReadOnlyList<(int DeltaRow, int DeltaCol)> StepsWritten(SyntaxNode node)
    {
        var elements = ElementsOf(node);
        var pairs = new List<(int DeltaRow, int DeltaCol)>(elements.Count);

        foreach (var element in elements)
        {
            if (!TryReadPair(element, out var pair))
            {
                return OrthogonalRunIn(elements);
            }

            pairs.Add(pair);
        }

        return pairs;
    }

    // The five-number run's neighbouring numbers paired, or none when the elements are not such a run.
    private static IReadOnlyList<(int DeltaRow, int DeltaCol)> OrthogonalRunIn(IReadOnlyList<ExpressionSyntax> elements)
    {
        var numbers = NumbersIn(elements);

        if (numbers.Count != OrthogonalRunLength)
        {
            return [];
        }

        return [.. numbers.Zip(numbers.Skip(1))];
    }

    private static IReadOnlyList<ExpressionSyntax> ElementsOf(SyntaxNode node) => node switch
    {
        CollectionExpressionSyntax collection => [.. collection.Elements.OfType<ExpressionElementSyntax>().Select(element => element.Expression)],
        InitializerExpressionSyntax initializer => initializer.Expressions,
        _ => [],
    };

    // A pair written as (row, column), [row, column] or new[] { row, column }.
    private static bool TryReadPair(ExpressionSyntax element, out (int DeltaRow, int DeltaCol) pair)
    {
        var numbers = NumbersIn(PairElementsOf(element));
        pair = default;

        if (numbers.Count != PairLength)
        {
            return false;
        }

        pair = (numbers[0], numbers[1]);
        return true;
    }

    private static IReadOnlyList<ExpressionSyntax> PairElementsOf(ExpressionSyntax element) => element switch
    {
        TupleExpressionSyntax tuple => [.. tuple.Arguments.Select(argument => argument.Expression)],
        ImplicitArrayCreationExpressionSyntax array => array.Initializer.Expressions,
        ArrayCreationExpressionSyntax { Initializer: { } initializer } => initializer.Expressions,
        _ => ElementsOf(element),
    };

    // Every element's number, or none when any element is not a literal whole number.
    private static List<int> NumbersIn(IReadOnlyList<ExpressionSyntax> elements)
    {
        var numbers = new List<int>(elements.Count);

        foreach (var element in elements)
        {
            if (!TryReadNumber(element, out var number))
            {
                return [];
            }

            numbers.Add(number);
        }

        return numbers;
    }

    // A whole number written as a literal, negated or not.
    private static bool TryReadNumber(ExpressionSyntax expression, out int number)
    {
        switch (expression)
        {
            case LiteralExpressionSyntax { Token.Value: int value }:
                number = value;
                return true;
            case PrefixUnaryExpressionSyntax { RawKind: (int)SyntaxKind.UnaryMinusExpression, Operand: LiteralExpressionSyntax { Token.Value: int value } }:
                number = -value;
                return true;
            default:
                number = 0;
                return false;
        }
    }

    // A literal copy of a table: where it is written, and which table it repeats.
    internal readonly record struct Copy(string Location, string Table);
}
