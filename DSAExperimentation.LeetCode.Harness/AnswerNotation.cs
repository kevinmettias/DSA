using System.Collections;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.LeetCode.Harness;

// A strategy's answer written back in LeetCode's notation, so CatalogJudge can compare it with the
// catalog's recorded text the way LeetCode compares a submission's printed answer: NotationReader's rules
// run backwards. A tree or a list is its LeetCodeWireFormat array, and no tree or list at all is
// LeetCode's [].
//
// A Try strategy answers with its return value and its out parameters together. It stands for the outputs
// when it found them, and for LeetCode's empty answer, [], when it found none.
internal static class AnswerNotation
{
    public static JsonNode Of(object? answer) => answer switch
    {
        null => new JsonArray(),
        string or bool or int or uint or long or double => JsonSerializer.SerializeToNode(answer) ?? new JsonArray(),
        char character => JsonValue.Create(character.ToString()),
        BinaryTreeNode<int> root => ArrayOf(LeetCodeWireFormat.FromBinaryTree(root).Cast<object?>()),
        SinglyLinkedListNode<int> head => ArrayOf(LeetCodeWireFormat.FromLinkedList(head).Cast<object?>()),
        ITuple tuple => ArrayOf(ItemsOf(tuple)),
        IEnumerable sequence when IsSequenceOfTreesOrLists(sequence.GetType()) => TreesOrListsArrayOf(sequence.Cast<object?>()),
        IEnumerable sequence => ArrayOf(sequence.Cast<object?>()),
        _ => throw new NotSupportedException($"No notation is written for a {answer.GetType()} answer."),
    };

    // A sequence of trees or lists writes a missing one as LeetCode's [], as it writes a whole answer that
    // is null: LC 725 prints the empty parts of a split list so.
    private static bool IsSequenceOfTreesOrLists(Type sequence)
        => ElementTypeOf(sequence) is { } element && (element == typeof(BinaryTreeNode<int>) || element == typeof(SinglyLinkedListNode<int>));

    // An array's element type, or the T of the IEnumerable<T> a sequence implements.
    private static Type? ElementTypeOf(Type sequence)
    {
        if (sequence.IsArray)
        {
            return sequence.GetElementType();
        }

        return sequence.GetInterfaces()
            .Append(sequence)
            .FirstOrDefault(contract => contract.IsGenericType && contract.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            ?.GetGenericArguments()[0];
    }

    // Each tree or list as its own array, and a missing one as [].
    private static JsonArray TreesOrListsArrayOf(IEnumerable<object?> items) => [.. items.Select(Of)];

    // A generated arm's answer to a Try strategy: the one tuple it gathered the strategy's return value and
    // out parameters into.
    public static JsonNode OfFoundOutputs(object? answer)
    {
        if (answer is not ITuple { Length: > 0 } gathered)
        {
            throw new InvalidOperationException($"{answer} is not a Try strategy's (found, outputs...) answer.");
        }

        if (gathered[0] is not true)
        {
            return NoneFound();
        }

        return OfFound(ItemsOf(gathered).Skip(1));
    }

    // The outputs a Try strategy found: one output is its own answer, as Find Substring With Given Hash
    // Value's substring is, and several are LeetCode's array of them, as Two Sum's two indices are.
    public static JsonNode OfFound(IEnumerable<object?> outputs)
    {
        var found = outputs.ToList();
        return found.Count == 1 ? Of(found[0]) : ArrayOf(found);
    }

    // What a Try strategy that found nothing stands for: LeetCode's empty answer.
    public static JsonNode NoneFound() => new JsonArray();

    private static JsonNode? ItemOf(object? item)
    {
        if (item is null)
        {
            return null;
        }

        return Of(item);
    }

    // Only a whole answer that is null means an empty tree or list, and so does a missing one in a sequence of
    // trees or lists; any other sequence's null is LeetCode's null, as a level-order tree writes a missing child.
    private static JsonArray ArrayOf(IEnumerable<object?> items) => [.. items.Select(ItemOf)];

    private static IEnumerable<object?> ItemsOf(ITuple tuple) => Enumerable.Range(0, tuple.Length).Select(index => tuple[index]);
}
