using System.Collections;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.Solutions.Conventions;

namespace DSAExperimentation.Benchmarks.Tests.Catalog;

// An arm's answer written back in LeetCode's notation, so it is judged against the catalog's expected
// text the way LeetCode judges a submission's printed answer: NotationReader's rules run backwards. A
// tree or a list is its LeetCodeWireFormat array, and no tree or list at all is LeetCode's [].
//
// A Try strategy's answer is (found, outputs...) - its return value gathered with its out parameters -
// and stands for the outputs when one was found and for LeetCode's empty answer, [], when none was.
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

    public static JsonNode OfFoundOutputs(object? answer)
    {
        if (answer is not ITuple { Length: > 0 } outputs)
        {
            throw new InvalidOperationException($"{answer} is not a Try strategy's (found, outputs...) answer.");
        }

        if (outputs[0] is not true)
        {
            return new JsonArray();
        }

        return ArrayOf(ItemsOf(outputs).Skip(1));
    }

    // Exactly as LeetCode compares, or with the answer's top-level elements in any order; each element is
    // compared as written either way.
    public static bool IsAccepted(JsonNode actual, JsonNode expected, LeetCodeAnswerOrder order)
    {
        var written = JsonNode.Parse(actual.ToJsonString());

        if (order != LeetCodeAnswerOrder.AnyOrder)
        {
            return JsonNode.DeepEquals(written, expected);
        }

        // Only a sequence answer has an order to free: a number or a string is compared as written whatever
        // the question says.
        if (written is not JsonArray answered || expected is not JsonArray wanted)
        {
            return JsonNode.DeepEquals(written, expected);
        }

        return SortedTextOf(answered).SequenceEqual(SortedTextOf(wanted), StringComparer.Ordinal);
    }

    private static JsonArray ArrayOf(IEnumerable<object?> items) => [.. items.Select(item => (JsonNode?)Of(item))];

    private static IEnumerable<object?> ItemsOf(ITuple tuple) => Enumerable.Range(0, tuple.Length).Select(index => tuple[index]);

    private static IEnumerable<string> SortedTextOf(JsonArray items) =>
        items.Select(item => item?.ToJsonString() ?? "null").Order(StringComparer.Ordinal);
}
