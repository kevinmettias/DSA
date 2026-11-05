using System.Runtime.CompilerServices;
using System.Text.Json;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.LeetCode.Harness;

// LeetCode's notation read into the type a strategy takes, chosen by that type at run time. The
// catalog's own LeetCodeNotation.Parse<TValue> needs the type at compile time, and a generated harness
// holds a strategy's parameter types as Type values. The rules are the ones the hand-written harnesses
// applied case by case:
// - JSON numbers, strings and booleans as themselves, and a one-character string as a char;
// - an array as an array, element by element;
// - an array as a tuple of the same arity, the way LeetCode writes an interval as [start, end];
// - a level-order array as a tree and a flat array as a linked list, through LeetCodeWireFormat;
// - an interface-typed sequence as an array, or as a List<T> when the interface lets a strategy add;
// - a role type standing for one value, such as SourceText(string Text), as that value: a value type
//   whose one public constructor takes one value and keeps it as the property of the same name.
// CatalogBenchmarkGenerator states the same rules over compile-time types, so a parameter it accepts is
// one this reads. A type outside them is refused by name, never guessed at, and a value that does not fit
// its type is a defect in the checked-in snapshot, which stops [GlobalSetup] rather than being skipped.
internal static class NotationReader
{
    // A ValueTuple holds seven items before its eighth slot nests the rest, which no LeetCode input needs.
    private const int MostTupleItems = 7;

    private const string BareNullText = "A bare null is not a string LeetCode passes.";

    private static readonly Dictionary<Type, Func<JsonElement, object>> ScalarReaders = new()
    {
        [typeof(int)] = element => element.GetInt32(),
        [typeof(long)] = element => element.GetInt64(),
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

    // Whether Read has a reading for the type: the dispatch Read follows, asked of the type alone, which is
    // how a strategy's parameters are checked before any case is read.
    public static bool CanRead(Type type)
    {
        if (IsReadAsAWhole(type))
        {
            return true;
        }

        if (type.IsArray)
        {
            return type.GetArrayRank() == 1 && CanRead(ElementTypeOf(type));
        }

        if (IsTuple(type))
        {
            return type.GetGenericArguments().All(CanRead);
        }

        if (IsSequence(type))
        {
            return CanRead(type.GetGenericArguments()[0]);
        }

        return OneValueOf(type) is { } value && CanRead(value);
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

    // The one value a role type stands for, as a positional record of one value declares it; null for any
    // other type. A value type alone, so a class's copy constructor never counts as a second way to build it.
    private static Type? OneValueOf(Type type)
    {
        if (!type.IsValueType)
        {
            return null;
        }

        if (type.GetConstructors() is not [var constructor] || constructor.GetParameters() is not [var parameter])
        {
            return null;
        }

        var property = parameter.Name is { } name ? type.GetProperty(name) : null;
        var keepsTheValue = property?.PropertyType == parameter.ParameterType;
        return keepsTheValue ? parameter.ParameterType : null;
    }

    private static object? ValueOf(JsonElement element, Type type)
    {
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

    private static object StructuredValueOf(JsonElement element, Type type)
    {
        if (type.IsArray)
        {
            return ArrayOf(element, ElementTypeOf(type));
        }

        if (IsTuple(type))
        {
            return TupleOf(element, type);
        }

        if (OneValueOf(type) is { } valueType)
        {
            var value = ValueOf(element, valueType);
            return Activator.CreateInstance(type, value) ?? throw new InvalidOperationException($"{type} could not be constructed.");
        }

        return SequenceOf(element, type);
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
