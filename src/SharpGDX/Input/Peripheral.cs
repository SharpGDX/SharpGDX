namespace SharpGDX.Input;

/// <summary>
///     Enumeration of potentially available peripherals.
/// </summary>
/// <remarks>
///     Use with <see cref="IInput.isPeripheralAvailable(Peripheral)" />.
/// </remarks>
public enum Peripheral
{
    HardwareKeyboard,
    OnscreenKeyboard,
    MultitouchScreen,
    Accelerometer,
    Compass,
    Vibrator,
    HapticFeedback,
    Gyroscope,
    RotationVector,
    Pressure
}