using DSAExperimentation.Benchmarks.Generators;

namespace DSAExperimentation.Benchmarks.Tests.Generators;

// The compile-time half of NotationReader's rules: each row names a field of Holder below, and the field's
// type is the parameter type in question.
public sealed partial class NotationTypesTests
{
    private const string Holder = """
        using System.Collections.Generic;
        using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

        internal sealed class Holder
        {
            public int Integer;
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
        }
        """;

    public static TheoryData<string> ReadableFields => ["Integer", "Text", "Character", "Grid", "Intervals", "Words", "Values", "Tree"];

    // A tree LeetCodeWireFormat cannot build, a map LeetCode has no notation for, and a rectangular array.
    public static TheoryData<string> UnreadableFields => ["TextTree", "Counts", "Rectangle"];

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
}
