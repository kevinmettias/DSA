using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace DSAExperimentation.Benchmarks.Tests;

// AnswerText renders an answer that is a value or a collection of values. A check run over every
// benchmark also meets answers that are object graphs - a reversed list's head node, a built tree's
// root, a quad tree, a copied list whose random pointers point back into itself - and harnesses
// whose void arm answers by rewriting their own workload. This renders those by walking fields, so
// two answers match only when the structures have the same shape and the same values. Each object
// is written once; meeting it again writes a back-reference to the order it was first met in, which
// keeps a cycle finite and still tells a tail that rejoins the head from one that rejoins the middle.
// The walk keeps its own stack rather than recursing, because a long list would overflow the thread.
internal static class AnswerGraphText
{
    // Written around a sequence, around an object's fields, around an unordered answer's elements,
    // between elements, for a null reference, before a back-reference and between a field's name and
    // its value; named once here rather than repeated as bare literals at each use.
    private const string SequenceStart = "[";
    private const string SequenceEnd = "]";
    private const string FieldsStart = "{";
    private const string FieldsEnd = "}";
    private const string SetStart = "{";
    private const string SetEnd = "}";
    private const string ElementSeparator = ",";
    private const string NullText = "null";
    private const string BackReferenceMarker = "#";
    private const string FieldValueSeparator = "=";

    // A runaway answer - an endless enumerable, a workload far past the smallest parameters - fails
    // with a message here instead of exhausting the test host's memory and killing every other test.
    private const int MostRenderedItems = 20_000_000;

    private const BindingFlags InstanceFields =
        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

    // Floating-point arms that order the same operations differently legitimately differ in the last
    // few bits, so a double is compared to twelve significant digits and a float to six.
    private const string DoublePrecision = "G12";
    private const string FloatPrecision = "G6";

    public static string Of(object? answer)
    {
        var walk = new Walk();
        walk.Pending.Push(answer);

        while (walk.Pending.Count > 0)
        {
            walk.Step();
        }

        return walk.Text.ToString();
    }

    // For an answer whose outer order the problem leaves unspecified: each element is rendered on its
    // own and the renderings are sorted, so two answers match when they hold the same elements.
    public static string OfUnordered(object? answer)
    {
        if (answer is not IEnumerable sequence)
        {
            return Of(answer);
        }

        var elements = sequence.Cast<object?>().Select(Of).Order(StringComparer.Ordinal);

        return $"{SetStart}{string.Join(ElementSeparator, elements)}{SetEnd}";
    }

    private static IEnumerable<FieldInfo> FieldsOf(Type type)
    {
        var lineage = new Stack<Type>();

        for (var current = type; current is not null && current != typeof(object); current = current.BaseType)
        {
            lineage.Push(current);
        }

        return lineage.SelectMany(current => current.GetFields(InstanceFields).OrderBy(field => field.MetadataToken));
    }

    // A record's or an auto-property's compiler-generated backing field is named <Name>k__BackingField.
    private static string DisplayNameOf(FieldInfo field) =>
        field.Name.StartsWith('<') ? BackedMemberNameOf(field.Name) : field.Name;

    private static string BackedMemberNameOf(string backingFieldName) =>
        backingFieldName[1..backingFieldName.IndexOf('>')];

    // Pushed text is written as-is; anything else pushed is a value still to be rendered.
    private sealed record Literal(string Text);

    private sealed class Walk
    {
        private readonly Dictionary<object, int> _firstMet = new(ReferenceEqualityComparer.Instance);
        private int _renderedItems;

        public Stack<object?> Pending { get; } = new();

        public StringBuilder Text { get; } = new();

        public void Step()
        {
            var item = Pending.Pop();

            if (++_renderedItems > MostRenderedItems)
            {
                throw new InvalidOperationException($"The answer is larger than {MostRenderedItems} rendered items.");
            }

            if (item is Literal literal)
            {
                Text.Append(literal.Text);
            }
            else if (item is null)
            {
                Text.Append(NullText);
            }
            else if (!TryAppendScalar(item))
            {
                ExpandComposite(item);
            }
        }

        private bool TryAppendScalar(object value)
        {
            var scalar = value switch
            {
                string item => $"\"{item}\"",
                double item => item.ToString(DoublePrecision, CultureInfo.InvariantCulture),
                float item => item.ToString(FloatPrecision, CultureInfo.InvariantCulture),
                IConvertible item => item.ToString(CultureInfo.InvariantCulture),
                IFormattable item => item.ToString(null, CultureInfo.InvariantCulture),
                Delegate item => item.Method.Name,
                MemberInfo item => item.Name,
                StringBuilder item => $"\"{item}\"",
                _ => null,
            };

            Text.Append(scalar);

            return scalar is not null;
        }

        private void ExpandComposite(object value)
        {
            if (value is IEnumerable sequence)
            {
                PushSequence(sequence);
            }
            else if (!value.GetType().IsValueType && _firstMet.TryGetValue(value, out var order))
            {
                Text.Append(BackReferenceMarker).Append(order);
            }
            else
            {
                PushFields(value);
            }
        }

        private void PushSequence(IEnumerable sequence)
        {
            var elements = new List<object?>();

            foreach (var element in sequence)
            {
                elements.Add(element);

                if (elements.Count > MostRenderedItems)
                {
                    throw new InvalidOperationException($"A sequence in the answer is longer than {MostRenderedItems} items.");
                }
            }

            Pending.Push(new Literal(SequenceEnd));

            for (var i = elements.Count - 1; i >= 0; i--)
            {
                Pending.Push(elements[i]);

                if (i > 0)
                {
                    Pending.Push(new Literal(ElementSeparator));
                }
            }

            Pending.Push(new Literal(SequenceStart));
        }

        private void PushFields(object value)
        {
            var type = value.GetType();

            if (!type.IsValueType)
            {
                _firstMet.Add(value, _firstMet.Count);
            }

            var fields = FieldsOf(type).ToList();
            Pending.Push(new Literal(FieldsEnd));

            for (var i = fields.Count - 1; i >= 0; i--)
            {
                Pending.Push(fields[i].GetValue(value));
                Pending.Push(new Literal($"{DisplayNameOf(fields[i])}{FieldValueSeparator}"));

                if (i > 0)
                {
                    Pending.Push(new Literal(ElementSeparator));
                }
            }

            Pending.Push(new Literal($"{type.Name}{FieldsStart}"));
        }
    }
}
