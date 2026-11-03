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
        string or bool or int or long or double => JsonSerializer.SerializeToNode(answer) ?? new JsonArray(),
        char character => JsonValue.Create(character.ToString()),
        BinaryTreeNode<int> root => ArrayOf(LeetCodeWireFormat.FromBinaryTree(root).Cast<object?>()),
        SinglyLinkedListNode<int> head => ArrayOf(LeetCodeWireFormat.FromLinkedList(head).Cast<object?>()),
        ITuple tuple => ArrayOf(ItemsOf(tuple)),
        IEnumerable sequence => ArrayOf(sequence.Cast<object?>()),
        _ => throw new NotSupportedException($"No notation is written for a {answer.GetType()} answer."),
    };

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

    // The outputs a Try strategy found.
    public static JsonNode OfFound(IEnumerable<object?> outputs) => ArrayOf(outputs);

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

    // Only a whole answer that is null means an empty tree or list; inside a sequence, null is LeetCode's
    // null, as a level-order tree writes a missing child.
    private static JsonArray ArrayOf(IEnumerable<object?> items) => [.. items.Select(ItemOf)];

    private static IEnumerable<object?> ItemsOf(ITuple tuple) => Enumerable.Range(0, tuple.Length).Select(index => tuple[index]);
}
