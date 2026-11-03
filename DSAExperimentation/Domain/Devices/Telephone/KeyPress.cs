namespace DSAExperimentation.Domain.Devices.Telephone;

// A letter typed by multi-tap: the key it is on, and how many presses of that key reach it, so 'e' is
// key 3 pressed twice.
/// <summary>A letter typed on a keypad: its key and how many presses reach it.</summary>
/// <param name="Key">The key the letter is on.</param>
/// <param name="Presses">How many presses of the key reach the letter, from 1 for its first letter.</param>
public readonly record struct KeyPress(PhoneKey Key, int Presses);
