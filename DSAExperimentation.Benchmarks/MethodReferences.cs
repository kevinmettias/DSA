using System.Reflection;
using System.Reflection.Emit;

namespace DSAExperimentation.Benchmarks;

// The methods and types one method body names, read from its IL: every call, constructor,
// delegate target, field owner and type token. This is what lets a benchmark be categorised by
// the library code its arms actually reach rather than by a tag someone has to remember to write -
// the IL is the only place that relationship is recorded once the source is compiled.
internal static class MethodReferences
{
    private const int TwoByteOpCodePrefix = 0xFE;
    private const int OpCodeTableSize = 0x100;
    private const int TokenSize = sizeof(int);

    private static readonly OpCode[] OneByteOpCodes = new OpCode[OpCodeTableSize];
    private static readonly OpCode[] TwoByteOpCodes = new OpCode[OpCodeTableSize];

    static MethodReferences()
    {
        foreach (var field in typeof(OpCodes).GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            if (field.GetValue(null) is OpCode opCode)
            {
                var value = (ushort)opCode.Value;

                if (value < OpCodeTableSize)
                {
                    OneByteOpCodes[value] = opCode;
                }
                else
                {
                    TwoByteOpCodes[value & (OpCodeTableSize - 1)] = opCode;
                }
            }
        }
    }

    // A method with no body - abstract, extern, an interface member - names nothing itself.
    //
    // A misread operand does not announce itself: the reader carries on from the wrong byte, and
    // small operands decode as harmless one-byte instructions until it happens to fall back into
    // step. So the read is strict instead - an undefined opcode, or a last instruction that does
    // not end exactly where the body does, throws rather than returning references read from the
    // wrong bytes.
    public static MethodBodyReferences Of(MethodBase method)
    {
        var il = method.GetMethodBody()?.GetILAsByteArray() ?? [];
        var methods = new List<MethodBase>();
        var types = new List<Type>();
        var offset = 0;

        while (offset < il.Length)
        {
            var opCode = ReadOpCode(il, ref offset)
                ?? throw new InvalidOperationException($"{method.DeclaringType?.Name}.{method.Name}: no opcode at IL offset {offset - 1}.");

            if (opCode.OperandType is OperandType.InlineMethod or OperandType.InlineTok or OperandType.InlineType
                or OperandType.InlineField)
            {
                Resolve(method, BitConverter.ToInt32(il, offset), new MethodBodyReferences(methods, types));
            }

            offset += OperandSize(opCode.OperandType, il, offset);
        }

        return offset == il.Length
            ? new MethodBodyReferences(methods, types)
            : throw new InvalidOperationException($"{method.DeclaringType?.Name}.{method.Name}: the last instruction overran the body.");
    }

    // Null for a byte no opcode is defined at, which the default OpCode marks with a size of zero.
    private static OpCode? ReadOpCode(byte[] il, ref int offset)
    {
        var first = il[offset++];
        var opCode = first == TwoByteOpCodePrefix ? TwoByteOpCodes[il[offset++]] : OneByteOpCodes[first];

        return opCode.Size == 0 ? null : opCode;
    }

    private static int OperandSize(OperandType operandType, byte[] il, int offset) => operandType switch
    {
        OperandType.InlineNone => 0,
        OperandType.ShortInlineBrTarget or OperandType.ShortInlineI or OperandType.ShortInlineVar => sizeof(byte),
        OperandType.InlineVar => sizeof(short),
        OperandType.InlineI8 or OperandType.InlineR => sizeof(long),
        OperandType.InlineSwitch => TokenSize + (BitConverter.ToInt32(il, offset) * TokenSize),
        _ => TokenSize,
    };

    // A token is resolved in the generic context of the method that holds it, and a token that
    // cannot be resolved there - a vararg call site, a member of an assembly that failed to load -
    // contributes nothing rather than failing the whole categorisation.
    private static void Resolve(MethodBase method, int token, MethodBodyReferences into)
    {
        var typeArguments = method.DeclaringType is { IsGenericType: true } owner ? owner.GetGenericArguments() : null;
        var methodArguments = method.IsGenericMethod ? method.GetGenericArguments() : null;

        try
        {
            switch (method.Module.ResolveMember(token, typeArguments, methodArguments))
            {
                case MethodBase target:
                    into.Methods.Add(target);
                    break;
                case Type type:
                    into.Types.Add(type);
                    break;
                case FieldInfo { DeclaringType: { } fieldOwner }:
                    into.Types.Add(fieldOwner);
                    break;
            }
        }
        catch (Exception exception) when (exception is ArgumentException or BadImageFormatException or TypeLoadException
            or MemberAccessException or IOException)
        {
            // An unresolvable token names nothing this categorisation can use.
        }
    }
}

// What one method body names: the methods it calls, constructs or takes a delegate to, and the
// types it mentions directly.
internal readonly record struct MethodBodyReferences(List<MethodBase> Methods, List<Type> Types);
