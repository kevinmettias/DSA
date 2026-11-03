using System.Collections.Immutable;

namespace DSAExperimentation.Domain.Devices.Telephone;

// A telephone keypad's layout: its ten keys, each with the letters printed on it. ITU-T E.161 is
// today's standard; North American keypads before it left out Q and Z. Keypad reads a layout's keys
// and answers which letters a key carries and which key and press type a letter.
/// <summary>A telephone keypad's layout: its keys, 0 through 9, and their letters.</summary>
public interface IKeypadLayout
{
    /// <summary>The ten keys, in digit order from 0 to 9.</summary>
    static abstract ImmutableArray<PhoneKey> Keys { get; }
}
