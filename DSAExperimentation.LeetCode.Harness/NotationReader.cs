using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.LeetCode.Harness;

// LeetCode's notation read into the type a strategy takes, chosen by that type at run time. The
// catalog's own LeetCodeNotation.Parse<TValue> needs the type at compile time, and a generated harness
// holds a strategy's parameter types as Type values. The rules are the ones the hand-written harnesses
// applied case by case:
// - JSON numbers, strings and booleans as themselves (a non-negative integer as a uint too), and a
//   one-character string as a char; and a value that may be absent, a T?, as null for LeetCode's null and
//   as a T otherwise, as Copy List with Random Pointer writes a node with no random pointer [7,null];
// - an array as an array, element by element, and an array of rows of one length as a rectangular array,
//   as Cherry Pickup reads its grid into an int[,];
// - an array as a tuple of the same arity, the way LeetCode writes an interval as [start, end];
// - a level-order array as a tree and a flat array as a linked list, through LeetCodeWireFormat;
// - an interface-typed sequence as an array, or as a List<T> when the interface lets a strategy add;
// - a positional record of value type - one public constructor that keeps each value as the property of
//   the same name - as its values: a role type of one value, such as SourceText(string Text), as that
//   value, and a record of several as an array of them, the way a tuple is read;
// - and, for a case that lists more inputs than the strategy takes, a tuple or record of several values
//   over as many consecutive inputs (ReadSpread), as Rectangle Area's eight coordinates are two
//   Rectangle(X1, Y1, X2, Y2).
// CatalogBenchmarkGenerator states the same rules over compile-time types, so a parameter it accepts is
// one this reads. A type outside them is refused by name, never guessed at, and a value that does not fit
// its type is a defect in the checked-in snapshot, which stops [GlobalSetup] rather than being skipped.
internal static class NotationReader
{
    // A ValueTuple holds seven items before its eighth slot nests the rest, which no LeetCode input needs.
    private const int MostTupleItems = 7;

    // A grid's rank, the one rank past a list's that LeetCode's rows of rows can fill.
    private const int GridRank = 2;

    private const string BareNullText = "A bare null is not a string LeetCode passes.";

    private static readonly Dictionary<Type, Func<JsonElement, object>> ScalarReaders = new()
    {
        [typeof(int)] = element => element.GetInt32(),
        [typeof(long)] = element => element.GetInt64(),
        [typeof(uint)] = element => element.GetUInt32(),
        [typeof(double)] = element => element.GetDouble(),
        [typeof(bool)] = element => element.GetBoolean(),
        [typeof(string)] = TextOf,
        [typeof(char)] = SingleCharacterOf,
    };

    // The interfaces an array implements, which a strategy can only read through.
    private static readonly Type[] ArrayBackedSequences =
        [typeof(IEnumerable<>), typeof(IReadOnlyList<>), typeof(IReadOnlyCollection<>)];

    // The sequences a strategy may add to, so they need a List<T> behind them.
    private static readonly Type[] ListBackedSequences = [typeof(List<>), typeof(IList<>), typeof(ICollection<>)];

    private static readonly Type[] SequenceDefinitions = [.. ArrayBackedSequences, .. ListBackedSequences];

    public static object? Read(string text, Type type)
    {
        using var document = JsonDocument.Parse(text);

        return ValueOf(document.RootElement, type);
    }

    // How many consecutive case inputs a parameter of this type can stand for: a tuple's or a record's
    // values, when it has several, and one for any other type.
    public static int WidthOf(Type type)
    {
        if (IsTuple(type))
        {
            return type.GetGenericArguments().Length;
        }

        return RecordValuesOf(type)?.Length ?? 1;
    }

    // A tuple or a record built from one value per text, in order.
    public static object ReadSpread(IReadOnlyList<string> texts, Type type)
    {
        // A tuple keeps its items in fields, so it is no positional record, and its type arguments are its values.
        var valueTypes = RecordValuesOf(type) ?? type.GetGenericArguments();
        var values = texts.Zip(valueTypes, Read).ToArray();
        return Activator.CreateInstance(type, values) ?? throw new InvalidOperationException($"{type} could not be constructed.");
    }

    // Whether Read has a reading for the type: the dispatch Read follows, asked of the type alone, which is
    // how a strategy's parameters are checked before any case is read.
    public static bool CanRead(Type type)
    {
        if (Nullable.GetUnderlyingType(type) is { } present)
        {
            return CanRead(present);
        }

        if (IsReadAsAWhole(type))
        {
            return true;
        }

        if (type.IsArray)
        {
            return type.GetArrayRank() <= GridRank && CanRead(ElementTypeOf(type));
        }

        if (IsTuple(type))
        {
            return type.GetGenericArguments().All(CanRead);
        }

        if (IsSequence(type))
        {
            return CanRead(type.GetGenericArguments()[0]);
        }

        return RecordValuesOf(type) is { } values && values.All(CanRead);
    }

    // A scalar, or a tree or list LeetCodeWireFormat builds from one array: read in one step, not item by item.
    private static bool IsReadAsAWhole(Type type) =>
        ScalarReaders.ContainsKey(type) || type == typeof(BinaryTreeNode<int>) || type == typeof(SinglyLinkedListNode<int>);

    private static bool IsSequence(Type type) =>
        type.IsGenericType && SequenceDefinitions.Contains(type.GetGenericTypeDefinition());

    private static object SingleCharacterOf(JsonElement element)
    {
        var text = (string)TextOf(element);

        if (text.Length != 1)
        {
            throw new InvalidOperationException($"\"{text}\" is not a single character.");
        }

        return text[0];
    }

    private static object TextOf(JsonElement element) =>
        element.GetString() ?? throw new InvalidOperationException(BareNullText);

    // The values a positional record of value type is built from, in order: the parameters of its one public
    // constructor, each kept as the property of the same name. Null for any other type. A value type alone,
    // so a class's copy constructor never counts as a second way to build it.
    private static Type[]? RecordValuesOf(Type type)
    {
        if (!type.IsValueType || type.GetConstructors() is not [var constructor])
        {
            return null;
        }

        var parameters = constructor.GetParameters();

        if (parameters.Length == 0 || !parameters.All(parameter => IsKeptAsProperty(type, parameter)))
        {
            return null;
        }

        return [.. parameters.Select(parameter => parameter.ParameterType)];
    }

    private static bool IsKeptAsProperty(Type type, ParameterInfo parameter) =>
        parameter.Name is { } name && type.GetProperty(name)?.PropertyType == parameter.ParameterType;

    private static object? ValueOf(JsonElement element, Type type)
    {
        if (Nullable.GetUnderlyingType(type) is { } present)
        {
            return MaybeValueOf(element, present);
        }

        if (ScalarReaders.TryGetValue(type, out var readScalar))
        {
            return readScalar(element);
        }

        if (type == typeof(BinaryTreeNode<int>))
        {
            return LeetCodeWireFormat.ToBinaryTree(element.Deserialize<int?[]>() ?? []);
        }

        if (type == typeof(SinglyLinkedListNode<int>))
        {
            return LeetCodeWireFormat.ToLinkedList(element.Deserialize<int[]>() ?? []);
        }

        return StructuredValueOf(element, type);
    }

    // LeetCode's null as no value, and any other element as the value it holds.
    private static object? MaybeValueOf(JsonElement element, Type present)
    {
        if (element.ValueKind == JsonValueKind.Null)
        {
            return null;
        }

        return ValueOf(element, present);
    }

    private static object StructuredValueOf(JsonElement element, Type type)
    {
        if (type.IsArray && type.GetArrayRank() == GridRank)
        {
            return GridOf(element, ElementTypeOf(type));
        }

        if (type.IsArray)
        {
            return ArrayOf(element, ElementTypeOf(type));
        }

        if (IsTuple(type))
        {
            return TupleOf(element, type);
        }

        if (RecordValuesOf(type) is { } valueTypes)
        {
            return RecordOf(element, type, valueTypes);
        }

        return SequenceOf(element, type);
    }

    // LeetCode's rows as a rectangular array, each row read as a list is; rows of different lengths fill no
    // rectangle, and are a defect in the snapshot.
    private static Array GridOf(JsonElement element, Type itemType)
    {
        var rows = element.EnumerateArray().Select(row => ArrayOf(row, itemType)).ToList();
        var columnCount = rows.FirstOrDefault()?.Length ?? 0;

        if (rows.Any(row => row.Length != columnCount))
        {
            throw new InvalidOperationException($"Rows of {columnCount} and of other lengths fill no rectangular {itemType.Name}[,].");
        }

        var grid = Array.CreateInstance(itemType, rows.Count, columnCount);

        for (var row = 0; row < rows.Count; row++)
        {
            CopyRow(rows[row], grid, row);
        }

        return grid;
    }

    private static void CopyRow(Array cells, Array grid, int row)
    {
        for (var column = 0; column < cells.Length; column++)
        {
            var cell = cells.GetValue(column);
            grid.SetValue(cell, row, column);
        }
    }

    private static object SequenceOf(JsonElement element, Type type)
    {
        var definition = type.IsGenericType ? type.GetGenericTypeDefinition() : type;

        if (ArrayBackedSequences.Contains(definition))
        {
            return ArrayOf(element, type.GetGenericArguments()[0]);
        }

        if (ListBackedSequences.Contains(definition))
        {
            var itemType = type.GetGenericArguments()[0];

            var listType = typeof(List<>).MakeGenericType(itemType);
            var items = ArrayOf(element, itemType);

            return Activator.CreateInstance(listType, items)
                ?? throw new InvalidOperationException($"A List<{itemType.Name}> could not be constructed.");
        }

        throw new NotSupportedException($"LeetCode's notation has no reading for {type}.");
    }

    private static Array ArrayOf(JsonElement element, Type itemType)
    {
        var items = Array.CreateInstance(itemType, element.GetArrayLength());
        var index = 0;

        foreach (var item in element.EnumerateArray())
        {
            var value = ValueOf(item, itemType);
            items.SetValue(value, index++);
        }

        return items;
    }

    // A record of one value from that value, and of several from an array of them.
    private static object RecordOf(JsonElement element, Type type, Type[] valueTypes)
    {
        var values = RecordArgumentsOf(element, valueTypes);
        return Activator.CreateInstance(type, values) ?? throw new InvalidOperationException($"{type} could not be constructed.");
    }

    private static object?[] RecordArgumentsOf(JsonElement element, Type[] valueTypes)
    {
        if (valueTypes.Length == 1)
        {
            return [ValueOf(element, valueTypes[0])];
        }

        return [.. element.EnumerateArray().Zip(valueTypes, ValueOf)];
    }

    private static object TupleOf(JsonElement element, Type type)
    {
        var itemTypes = type.GetGenericArguments();

        if (element.GetArrayLength() != itemTypes.Length)
        {
            throw new InvalidOperationException($"{element.GetRawText()} has {element.GetArrayLength()} items, and {type} has {itemTypes.Length}.");
        }

        var items = element.EnumerateArray().Zip(itemTypes, ValueOf).ToArray();

        return Activator.CreateInstance(type, items)
            ?? throw new InvalidOperationException($"{type} could not be constructed.");
    }

    private static bool IsTuple(Type type) =>
        type is { IsValueType: true, IsGenericType: true }
            && typeof(ITuple).IsAssignableFrom(type)
            && type.GetGenericArguments().Length <= MostTupleItems;

    private static Type ElementTypeOf(Type arrayType) =>
        arrayType.GetElementType() ?? throw new InvalidOperationException($"{arrayType} is an array with no element type.");
}
