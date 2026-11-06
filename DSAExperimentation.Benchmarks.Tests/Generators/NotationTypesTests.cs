using DSAExperimentation.Benchmarks.Generators;

namespace DSAExperimentation.Benchmarks.Tests.Generators;

// The compile-time half of NotationReader's rules: each row names a field of Holder below, and the field's
// type is the parameter type in question.
public sealed partial class NotationTypesTests
{
    // Holder's array of values that may be absent, and how a generated class writes its type.
    private const string NullableArrayField = "RandomNodes";
    private const string NullableArrayType = "int?[][]";

    private const string Holder = """
        using System.Collections.Generic;
        using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

        internal sealed class Holder
        {
            public int Integer;
            public uint Unsigned;
            public string Text;
            public char Character;
            public int[][] Grid;
            public (int Start, int End)[] Intervals;
            public IEnumerable<string> Words;
            public IList<int> Values;
            public BinaryTreeNode<int> Tree;
            public BinaryTreeNode<string> TextTree;
            public Dictionary<int, int> Counts;
            public int[,] Rectangle;
            public int?[][] RandomNodes;
            public int[,,] Cube;
            public RoleText Role;
            public Endpoints TwoValues;
            public BoxedText BoxedRole;
            public Capacity Sized;
        }

        internal readonly record struct RoleText(string Text);

        internal readonly record struct Endpoints(int Start, int End);

        internal sealed record BoxedText(string Text);

        internal readonly struct Capacity
        {
            public Capacity(int count) { }
        }
        """;

    public static TheoryData<string> ReadableFields => ["Integer", "Unsigned", "Text", "Character", "Grid", "Rectangle", "RandomNodes", "Intervals", "Words", "Values", "Tree", "Role", "TwoValues"];

    // A tree LeetCodeWireFormat cannot build, a map LeetCode has no notation for, an array of a rank past a
    // grid's, a role type that is a class, and a struct that keeps no value it is built from.
    public static TheoryData<string> UnreadableFields => ["TextTree", "Counts", "Cube", "BoxedRole", "Sized"];

    [Theory]
    [MemberData(nameof(ReadableFields))]
    public void IsReadable_FieldNotationReaderReads_IsTrue(string field) =>
        Assert.True(NotationTypes.IsReadable(GeneratorProbe.FieldType(Holder, field)));

    [Theory]
    [MemberData(nameof(UnreadableFields))]
    public void IsReadable_FieldNotationReaderRefuses_IsFalse(string field) =>
        Assert.False(NotationTypes.IsReadable(GeneratorProbe.FieldType(Holder, field)));

    // typeof refuses tuple element names, so a rendered type drops them.
    [Fact]
    public void Render_NamedTupleArray_DropsTheElementNames() =>
        Assert.Equal("(int, int)[]", NotationTypes.Render(GeneratorProbe.FieldType(Holder, "Intervals")));

    [Fact]
    public void Render_GenericType_QualifiesItFully() =>
        Assert.Equal("global::System.Collections.Generic.IEnumerable<string>", NotationTypes.Render(GeneratorProbe.FieldType(Holder, "Words")));

    // A value that may be absent writes as its type and a question mark, never as Nullable<T> with its argument twice.
    [Fact]
    public void Render_NullableArray_MarksTheValueType()
    {
        var rendered = NotationTypes.Render(GeneratorProbe.FieldType(Holder, NullableArrayField));

        Assert.Equal(NullableArrayType, rendered);
    }

    // C# reads rank specifiers outermost first, so an array of a grid puts its own brackets before the grid's.
    public static TheoryData<string, int, string> NestedArrays =>
        new()
        {
            { "int", 1, "int[]" },
            { "int[,]", 1, "int[][,]" },
            { "int[,]", 2, "int[][][,]" },
            { "int[][]", 1, "int[][][]" },
            { "(int, int)[]", 1, "(int, int)[][]" },
            { "global::System.Collections.Generic.IList<int[]>", 1, "global::System.Collections.Generic.IList<int[]>[]" },
        };

    [Theory]
    [MemberData(nameof(NestedArrays))]
    public void ArrayOf_RenderedType_PutsTheNewRanksOutermost(string element, int depth, string expected) =>
        Assert.Equal(expected, NotationTypes.ArrayOf(element, depth));
}
