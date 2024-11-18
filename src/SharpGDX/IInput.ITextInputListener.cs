namespace SharpGDX;

public partial interface IInput
{
    /// <summary>
    ///     Callback interface for <see cref="GetTextInput(ITextInputListener, string, string, string)" />.
    /// </summary>
    public interface ITextInputListener
    {
        public void Canceled();
        public void Input(string text);
    }
}